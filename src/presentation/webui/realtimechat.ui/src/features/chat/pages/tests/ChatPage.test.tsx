import { act, render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ApiError } from "../../../../shared/api/ApiError";
import {
  AuthContext,
  type AuthContextValue,
} from "../../../auth/context/authContext";
import type { ChatMessage } from "../../types/chat";
import type { RealtimeConnectionStatus } from "../../types/realtime";
import { ChatPage } from "../ChatPage";

const chatServiceMocks = vi.hoisted(() => ({
  getMessages: vi.fn(),
  createMessage: vi.fn(),
}));

vi.mock("../../api/chatService", () => ({
  chatService: {
    getMessages: chatServiceMocks.getMessages,
    createMessage: chatServiceMocks.createMessage,
  },
}));

const realtimeChatMocks = vi.hoisted(() => ({
  onMessage: null as ((message: ChatMessage) => void) | null,
  connectionStatus: "connected" as RealtimeConnectionStatus,
  connectionError: null as string | null,
  onlineUsers: [] as unknown[],
}));

vi.mock("../../realtime/useRealtimeChat", () => ({
  useRealtimeChat: ({
    onMessage,
  }: {
    onMessage: (message: ChatMessage) => void;
  }) => {
    realtimeChatMocks.onMessage = onMessage;

    return {
      connectionStatus: realtimeChatMocks.connectionStatus,
      connectionError: realtimeChatMocks.connectionError,
      onlineUsers: realtimeChatMocks.onlineUsers,
    };
  },
}));

const currentUser = {
  userId: 4,
  userName: "realtime.demo",
  email: "realtime.demo@example.invalid",
  roles: ["Member"],
};

function createMessage(overrides: Partial<ChatMessage> = {}): ChatMessage {
  return {
    id: "f9f53075-fbba-4f23-a817-0a4e76ea17e1",
    senderUserId: 4,
    senderUserName: "realtime.demo",
    message: "Test mesajı",
    sendAt: "2026-09-25T20:00:00Z",
    ...overrides,
  };
}

function renderChatPage() {
  const logout = vi.fn<() => Promise<void>>();
  logout.mockResolvedValue(undefined);

  const contextValue: AuthContextValue = {
    status: "authenticated",
    user: currentUser,
    sessionError: null,
    login: vi.fn(),
    logout,
  };

  render(
    <AuthContext.Provider value={contextValue}>
      <MemoryRouter initialEntries={["/chat"]}>
        <Routes>
          <Route path="/chat" element={<ChatPage />} />
          <Route path="/login" element={<h1>Giriş ekranı</h1>} />
        </Routes>
      </MemoryRouter>
    </AuthContext.Provider>,
  );
}

describe("ChatPage", () => {
  beforeEach(() => {
    chatServiceMocks.getMessages.mockReset();
    chatServiceMocks.createMessage.mockReset();
    chatServiceMocks.getMessages.mockResolvedValue([]);
    realtimeChatMocks.onMessage = null;
    realtimeChatMocks.connectionStatus = "connected";
    realtimeChatMocks.connectionError = null;
    realtimeChatMocks.onlineUsers = [];
  });

  it("API üzerinden alınan mesaj geçmişini gösterir", async () => {
    chatServiceMocks.getMessages.mockResolvedValue([
      createMessage({
        senderUserId: 8,
        senderUserName: "other.user",
        message: "Geçmişten gelen mesaj",
      }),
    ]);

    renderChatPage();

    expect(
      await screen.findByText("Geçmişten gelen mesaj"),
    ).toBeInTheDocument();

    expect(screen.getByText("other.user")).toBeInTheDocument();
    expect(chatServiceMocks.getMessages).toHaveBeenCalledOnce();
  });

  it("Enter tuşuyla mesaj gönderir ve cevabı listeye ekler", async () => {
    const user = userEvent.setup();

    chatServiceMocks.createMessage.mockResolvedValue(
      createMessage({
        message: "Enter ile gönderilen mesaj",
      }),
    );

    renderChatPage();

    await screen.findByText("Son 30 dakika içinde henüz mesaj bulunmuyor.");

    const messageInput = screen.getByRole("textbox", {
      name: "Mesaj",
    });

    await user.type(messageInput, "Enter ile gönderilen mesaj{Enter}");

    expect(chatServiceMocks.createMessage).toHaveBeenCalledWith({
      message: "Enter ile gönderilen mesaj",
    });

    expect(
      await screen.findByText("Enter ile gönderilen mesaj"),
    ).toBeInTheDocument();

    expect(messageInput).toHaveValue("");
  });

  it("Shift ve Enter ile yeni satır ekler ve mesaj göndermez", async () => {
    const user = userEvent.setup();

    renderChatPage();

    await screen.findByText("Son 30 dakika içinde henüz mesaj bulunmuyor.");

    const messageInput = screen.getByRole("textbox", {
      name: "Mesaj",
    });

    await user.type(messageInput, "İlk satır");
    await user.keyboard("{Shift>}{Enter}{/Shift}");

    expect(chatServiceMocks.createMessage).not.toHaveBeenCalled();
    expect(messageInput).toHaveValue("İlk satır\n");
  });

  it("mesaj kotası dolduğunda kullanıcıya anlaşılır hata gösterir", async () => {
    const user = userEvent.setup();

    chatServiceMocks.createMessage.mockRejectedValue(
      new ApiError(429, {
        title: "Message quota exceeded.",
        status: 429,
      }),
    );

    renderChatPage();

    await screen.findByText("Son 30 dakika içinde henüz mesaj bulunmuyor.");

    const messageInput = screen.getByRole("textbox", {
      name: "Mesaj",
    });

    await user.type(messageInput, "Kota testi{Enter}");

    expect(
      await screen.findByText(
        "Mesaj gönderme limitine ulaştınız. Lütfen daha sonra tekrar deneyin.",
      ),
    ).toBeInTheDocument();

    expect(messageInput).toHaveValue("Kota testi");
  });

  it("mesaj geçmişi hatasından sonra yeniden yükleme yapılabilir", async () => {
    const user = userEvent.setup();

    chatServiceMocks.getMessages
      .mockRejectedValueOnce(
        new ApiError(500, {
          title: "Internal Server Error",
          status: 500,
          detail: "Mesaj servisine erişilemedi.",
        }),
      )
      .mockResolvedValueOnce([
        createMessage({
          message: "Yeniden deneme sonrası mesaj",
        }),
      ]);

    renderChatPage();

    expect(
      await screen.findByText("Mesaj servisine erişilemedi."),
    ).toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: "Tekrar dene" }));

    expect(
      await screen.findByText("Yeniden deneme sonrası mesaj"),
    ).toBeInTheDocument();

    expect(chatServiceMocks.getMessages).toHaveBeenCalledTimes(2);
  });

  it("backend validation hatasını mesaj alanında gösterir", async () => {
    const user = userEvent.setup();

    chatServiceMocks.createMessage.mockRejectedValue(
      new ApiError(400, {
        title: "Request validation failed.",
        status: 400,
        errors: {
          Message: ["Message cannot exceed 1000 characters."],
        },
      }),
    );

    renderChatPage();

    await screen.findByText("Son 30 dakika içinde henüz mesaj bulunmuyor.");

    const messageInput = screen.getByRole("textbox", {
      name: "Mesaj",
    });

    await user.type(messageInput, "Validation testi{Enter}");

    expect(
      await screen.findByText("Message cannot exceed 1000 characters."),
    ).toBeInTheDocument();

    expect(messageInput).toHaveAttribute("aria-invalid", "true");
    expect(messageInput).toHaveValue("Validation testi");
  });

  it("SignalR üzerinden gelen mesajı listeye ekler", async () => {
    renderChatPage();

    await screen.findByText("Son 30 dakika içinde henüz mesaj bulunmuyor.");

    act(() => {
      realtimeChatMocks.onMessage?.(
        createMessage({
          id: "signalr-message-1",
          senderUserId: 8,
          senderUserName: "other.user",
          message: "SignalR üzerinden gelen mesaj",
        }),
      );
    });

    expect(
      screen.getByText("SignalR üzerinden gelen mesaj"),
    ).toBeInTheDocument();
    expect(screen.getByText("1 mesaj")).toBeInTheDocument();
  });

  it("aynı mesaj HTTP ve SignalR üzerinden geldiğinde tekrar eklemez", async () => {
    const message = createMessage({
      id: "shared-message-id",
      message: "Tekil kalması gereken mesaj",
    });

    chatServiceMocks.getMessages.mockResolvedValue([message]);

    renderChatPage();

    expect(
      await screen.findByText("Tekil kalması gereken mesaj"),
    ).toBeInTheDocument();

    act(() => {
      realtimeChatMocks.onMessage?.(message);
    });

    expect(screen.getAllByText("Tekil kalması gereken mesaj")).toHaveLength(1);
    expect(screen.getByText("1 mesaj")).toBeInTheDocument();
  });

  it("bağlantı durumunu ve çevrimiçi kullanıcı sayısını gösterir", async () => {
    realtimeChatMocks.onlineUsers = [{}, {}];

    renderChatPage();

    await screen.findByText("Son 30 dakika içinde henüz mesaj bulunmuyor.");

    expect(screen.getByText("Canlı")).toBeInTheDocument();
    expect(screen.getByText("2 çevrimiçi")).toBeInTheDocument();
  });

  it("SignalR bağlantısı kurulamadığında HTTP kullanım uyarısını gösterir", async () => {
    realtimeChatMocks.connectionStatus = "disconnected";
    realtimeChatMocks.connectionError =
      "Gerçek zamanlı bağlantı kurulamadı.";

    renderChatPage();

    await screen.findByText("Son 30 dakika içinde henüz mesaj bulunmuyor.");

    expect(screen.getByText("Bağlantı kesildi")).toBeInTheDocument();
    expect(
      screen.getByText(/Mesaj geçmişini HTTP üzerinden kullanmaya devam/),
    ).toBeInTheDocument();
  });
});
