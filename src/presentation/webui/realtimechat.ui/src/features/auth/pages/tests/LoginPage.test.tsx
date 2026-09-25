import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router";
import { describe, expect, it, vi } from "vitest";
import { ApiError } from "../../../../shared/api/ApiError";
import { AuthContext, type AuthContextValue } from "../../context/authContext";
import type { LoginCredentials } from "../../types/auth";
import { LoginPage } from "../LoginPage";

function createLoginMock() {
  return vi.fn<(credentials: LoginCredentials) => Promise<void>>();
}

function renderLoginPage(login: AuthContextValue["login"]) {
  const contextValue: AuthContextValue = {
    status: "anonymous",
    user: null,
    sessionError: null,
    login,
    logout: vi.fn(),
  };

  render(
    <AuthContext.Provider value={contextValue}>
      <MemoryRouter initialEntries={["/login"]}>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/chat" element={<h1>Sohbet ekranı</h1>} />
        </Routes>
      </MemoryRouter>
    </AuthContext.Provider>,
  );
}

describe("LoginPage", () => {
  it("bilgileri gönderir ve başarılı girişte sohbet ekranına yönlendirir", async () => {
    const user = userEvent.setup();
    const login = createLoginMock();

    login.mockResolvedValue(undefined);
    renderLoginPage(login);

    await user.type(
      screen.getByRole("textbox", { name: /kullanıcı adı/i }),
      "realtime.demo",
    );

    await user.type(screen.getByLabelText(/şifre|parola/i), "DemoPassword!");

    await user.click(screen.getByRole("button", { name: /giriş yap/i }));

    expect(login).toHaveBeenCalledWith({
      userName: "realtime.demo",
      password: "DemoPassword!",
    });

    expect(
      await screen.findByRole("heading", { name: "Sohbet ekranı" }),
    ).toBeInTheDocument();
  });

  it("401 yanıtında kullanıcıya anlaşılır hata gösterir", async () => {
    const user = userEvent.setup();
    const login = createLoginMock();

    login.mockRejectedValue(
      new ApiError(401, {
        title: "Unauthorized",
        status: 401,
      }),
    );

    renderLoginPage(login);

    await user.type(
      screen.getByRole("textbox", { name: /kullanıcı adı/i }),
      "realtime.demo",
    );

    await user.type(screen.getByLabelText(/şifre|parola/i), "WrongPassword!");

    await user.click(screen.getByRole("button", { name: /giriş yap/i }));

    expect(
      await screen.findByText(/kullanıcı adı.*(şifre|parola).*hatalı/i),
    ).toBeInTheDocument();
  });
});
