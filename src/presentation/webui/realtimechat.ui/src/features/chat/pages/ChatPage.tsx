import {
  useCallback,
  useEffect,
  useState,
  type FormEvent,
  type KeyboardEvent,
} from "react";
import { useNavigate } from "react-router";
import { ApiError } from "../../../shared/api/ApiError";
import { useAuth } from "../../auth/context/useAuth";
import { chatService } from "../api/chatService";
import type { ChatMessage } from "../types/chat";

type MessageHistoryStatus = "loading" | "ready" | "error";

function getHistoryErrorMessage(error: unknown): string {
  if (!(error instanceof ApiError)) {
    return "API bağlantısı kurulamadı. Lütfen tekrar deneyin.";
  }

  if (error.status === 401) {
    return "Oturumunuz sona ermiş olabilir. Lütfen yeniden giriş yapın.";
  }

  return (
    error.problemDetails?.detail ??
    "Mesaj geçmişi yüklenemedi. Lütfen tekrar deneyin."
  );
}

function formatMessageTime(value: string): string {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "";
  }

  return new Intl.DateTimeFormat("tr-TR", {
    hour: "2-digit",
    minute: "2-digit",
  }).format(date);
}

export function ChatPage() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [historyStatus, setHistoryStatus] =
    useState<MessageHistoryStatus>("loading");
  const [historyError, setHistoryError] = useState<string | null>(null);

  const [messageDraft, setMessageDraft] = useState("");
  const [messageFieldError, setMessageFieldError] = useState<string | null>(
    null,
  );
  const [sendError, setSendError] = useState<string | null>(null);
  const [isSending, setIsSending] = useState(false);

  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [logoutError, setLogoutError] = useState<string | null>(null);

  const loadMessages = useCallback(async () => {
    setHistoryStatus("loading");
    setHistoryError(null);

    try {
      const messageHistory = await chatService.getMessages();

      setMessages(messageHistory);
      setHistoryStatus("ready");
    } catch (error) {
      setHistoryError(getHistoryErrorMessage(error));
      setHistoryStatus("error");
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    void loadMessages();
  }, [loadMessages]);

  function handleMessageKeyDown(event: KeyboardEvent<HTMLTextAreaElement>) {
    const shouldSubmit =
      event.key === "Enter" &&
      !event.shiftKey &&
      !event.nativeEvent.isComposing;

    if (!shouldSubmit) {
      return;
    }

    event.preventDefault();

    if (isSending || !messageDraft.trim()) {
      return;
    }

    event.currentTarget.form?.requestSubmit();
  }

  async function handleSendMessage(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const normalizedMessage = messageDraft.trim();

    setMessageFieldError(null);
    setSendError(null);

    if (!normalizedMessage) {
      setMessageFieldError("Mesaj alanı boş bırakılamaz.");
      return;
    }

    setIsSending(true);

    try {
      const createdMessage = await chatService.createMessage({
        message: normalizedMessage,
      });

      setMessages((currentMessages) => [...currentMessages, createdMessage]);

      setMessageDraft("");
      setHistoryStatus("ready");
      setHistoryError(null);
    } catch (error) {
      if (error instanceof ApiError) {
        const validationError = getMessageValidationError(error);

        if (validationError) {
          setMessageFieldError(validationError);
        } else {
          setSendError(getSendErrorMessage(error));
        }
      } else {
        setSendError(getSendErrorMessage(error));
      }
    } finally {
      setIsSending(false);
    }
  }

  async function handleLogout() {
    setIsLoggingOut(true);
    setLogoutError(null);

    try {
      await logout();
      navigate("/login", { replace: true });
    } catch (error) {
      if (error instanceof ApiError) {
        setLogoutError(
          error.problemDetails?.detail ?? "Çıkış işlemi tamamlanamadı.",
        );
      } else {
        setLogoutError("API bağlantısı kurulamadı. Lütfen tekrar deneyin.");
      }

      setIsLoggingOut(false);
    }
  }

  return (
    <main className="chat-shell">
      <header className="chat-header">
        <div>
          <span className="chat-brand">Siglora</span>
          <p className="chat-product-description">Gerçek zamanlı mesajlaşma</p>
        </div>

        <div className="user-panel">
          <div className="user-summary">
            <strong>{user?.userName}</strong>
            <span>{user?.roles.join(", ") || "Member"}</span>
          </div>

          <button
            className="logout-button"
            type="button"
            onClick={handleLogout}
            disabled={isLoggingOut}
          >
            {isLoggingOut ? "Çıkış yapılıyor..." : "Çıkış yap"}
          </button>
        </div>
      </header>

      {logoutError && (
        <p className="chat-error" role="alert">
          {logoutError}
        </p>
      )}

      <section
        className="message-panel"
        aria-labelledby="message-history-title"
      >
        <header className="message-panel-header">
          <div>
            <h1 id="message-history-title">Sohbet</h1>
            <p>Son 30 dakikadaki mesajlar</p>
          </div>

          <span className="message-count">{messages.length} mesaj</span>
        </header>

        <div className="message-history" aria-live="polite">
          {historyStatus === "loading" && (
            <div className="message-state" role="status">
              <span className="loading-indicator" aria-hidden="true" />
              <p>Mesajlar yükleniyor...</p>
            </div>
          )}

          {historyStatus === "error" && (
            <div className="message-state message-state--error">
              <p role="alert">{historyError}</p>

              <button
                className="retry-button"
                type="button"
                onClick={() => void loadMessages()}
              >
                Tekrar dene
              </button>
            </div>
          )}

          {historyStatus === "ready" && messages.length === 0 && (
            <div className="message-state">
              <p>Son 30 dakika içinde henüz mesaj bulunmuyor.</p>
            </div>
          )}

          {historyStatus === "ready" && messages.length > 0 && (
            <ul className="message-list">
              {messages.map((message) => {
                const isOwnMessage =
                  message.senderUserId === user?.userId ||
                  (message.senderUserId === null &&
                    message.senderUserName === user?.userName);

                return (
                  <li
                    className={
                      isOwnMessage
                        ? "message-item message-item--own"
                        : "message-item"
                    }
                    key={message.id}
                  >
                    <article className="message-bubble">
                      <header className="message-metadata">
                        <strong>{message.senderUserName}</strong>
                        <time dateTime={message.sendAt}>
                          {formatMessageTime(message.sendAt)}
                        </time>
                      </header>

                      <p>{message.message}</p>
                    </article>
                  </li>
                );
              })}
            </ul>
          )}
        </div>

        <form className="message-composer" onSubmit={handleSendMessage}>
          {sendError && (
            <p className="composer-error" role="alert">
              {sendError}
            </p>
          )}

          <label className="visually-hidden" htmlFor="chat-message">
            Mesaj
          </label>

          <div className="composer-input-row">
            <textarea
              id="chat-message"
              className="message-input"
              value={messageDraft}
              onChange={(event) => {
                setMessageDraft(event.target.value);
                setMessageFieldError(null);
                setSendError(null);
              }}
              onKeyDown={handleMessageKeyDown}
              placeholder="Bir mesaj yaz..."
              rows={2}
              maxLength={1000}
              required
              disabled={isSending}
              aria-invalid={Boolean(messageFieldError)}
              aria-describedby={
                messageFieldError ? "chat-message-error" : undefined
              }
            />

            <button
              className="send-button"
              type="submit"
              disabled={isSending || !messageDraft.trim()}
            >
              {isSending ? "Gönderiliyor..." : "Gönder"}
            </button>
          </div>

          <div className="composer-footer">
            {messageFieldError ? (
              <span className="composer-field-error" id="chat-message-error">
                {messageFieldError}
              </span>
            ) : (
              <span className="composer-hint">
                Enter gönderir · Shift + Enter yeni satır ekler
              </span>
            )}

            <span
              className={
                messageDraft.length >= 900
                  ? "message-character-count message-character-count--warning"
                  : "message-character-count"
              }
            >
              {messageDraft.length}/1000
            </span>
          </div>
        </form>
      </section>
    </main>
  );
}

function getMessageValidationError(error: ApiError): string | undefined {
  const errors = error.problemDetails?.errors;

  if (!errors) {
    return undefined;
  }

  const matchingEntry = Object.entries(errors).find(
    ([propertyName]) => propertyName.toLowerCase() === "message",
  );

  return matchingEntry?.[1]?.[0];
}

function getSendErrorMessage(error: unknown): string {
  if (!(error instanceof ApiError)) {
    return "API bağlantısı kurulamadı. Lütfen tekrar deneyin.";
  }

  if (error.status === 401) {
    return "Oturumunuz sona erdi. Lütfen yeniden giriş yapın.";
  }

  if (error.status === 429) {
    return "Mesaj gönderme limitine ulaştınız. Lütfen daha sonra tekrar deneyin.";
  }

  return (
    error.problemDetails?.detail ??
    "Mesaj gönderilemedi. Lütfen tekrar deneyin."
  );
}
