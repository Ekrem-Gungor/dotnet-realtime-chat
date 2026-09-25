import { useState } from "react";
import { useNavigate } from "react-router";
import { ApiError } from "../../../shared/api/ApiError";
import { useAuth } from "../../auth/context/useAuth";

export function ChatPage() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [logoutError, setLogoutError] = useState<string | null>(null);

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

      <section className="chat-empty-state" aria-labelledby="chat-title">
        <div className="chat-status-dot" />

        <h1 id="chat-title">Hoş geldin, {user?.userName}</h1>

        <p>
          Mesaj geçmişi ve gerçek zamanlı iletişim özellikleri sonraki aşamada
          burada yer alacak.
        </p>
      </section>
    </main>
  );
}
