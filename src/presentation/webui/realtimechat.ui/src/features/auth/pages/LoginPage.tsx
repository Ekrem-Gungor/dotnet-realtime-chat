import { useState, type ChangeEvent, type FormEvent } from "react";
import { useNavigate } from "react-router";
import { ApiError } from "../../../shared/api/ApiError";
import { useAuth } from "../context/useAuth";
import type { LoginCredentials } from "../types/auth";

type LoginFieldErrors = Partial<Record<keyof LoginCredentials, string>>;

function getValidationMessage(
  error: ApiError,
  field: keyof LoginCredentials,
): string | undefined {
  const errors = error.problemDetails?.errors;

  if (!errors) {
    return undefined;
  }

  const matchingEntry = Object.entries(errors).find(
    ([propertyName]) => propertyName.toLowerCase() === field.toLowerCase(),
  );

  return matchingEntry?.[1]?.[0];
}

export function LoginPage() {
  const navigate = useNavigate();
  const { login, sessionError } = useAuth();

  const [credentials, setCredentials] = useState<LoginCredentials>({
    userName: "",
    password: "",
  });

  const [fieldErrors, setFieldErrors] = useState<LoginFieldErrors>({});

  const [formError, setFormError] = useState<string | null>(sessionError);

  const [isSubmitting, setIsSubmitting] = useState(false);

  function handleChange(event: ChangeEvent<HTMLInputElement>) {
    const field = event.target.name as keyof LoginCredentials;

    setCredentials((current) => ({
      ...current,
      [field]: event.target.value,
    }));

    setFieldErrors((current) => ({
      ...current,
      [field]: undefined,
    }));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setIsSubmitting(true);
    setFormError(null);
    setFieldErrors({});

    try {
      await login(credentials);
      navigate("/chat", { replace: true });
    } catch (error) {
      if (error instanceof ApiError) {
        const userNameError = getValidationMessage(error, "userName");
        const passwordError = getValidationMessage(error, "password");

        setFieldErrors({
          userName: userNameError,
          password: passwordError,
        });

        if (userNameError || passwordError) {
          setFormError("Lütfen form alanlarını kontrol edin.");
        } else if (error.status === 401) {
          setFormError("Kullanıcı adı veya parola hatalı.");
        } else {
          setFormError(
            error.problemDetails?.detail ?? "Giriş işlemi tamamlanamadı.",
          );
        }
      } else {
        setFormError("API bağlantısı kurulamadı. Lütfen tekrar deneyin.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="app-shell">
      <section
        className="app-placeholder login-card"
        aria-labelledby="login-title"
      >
        <span className="app-brand">Siglora</span>

        <h1 id="login-title">Tekrar hoş geldin</h1>

        <p className="login-subtitle">
          Mesajlarına devam etmek için hesabına giriş yap.
        </p>

        <form className="auth-form" onSubmit={handleSubmit}>
          {formError && (
            <p className="form-error" role="alert">
              {formError}
            </p>
          )}

          <div className="form-field">
            <label className="form-label" htmlFor="userName">
              Kullanıcı adı
            </label>

            <input
              className="form-input"
              id="userName"
              name="userName"
              type="text"
              value={credentials.userName}
              onChange={handleChange}
              autoComplete="username"
              maxLength={256}
              required
              disabled={isSubmitting}
              aria-invalid={Boolean(fieldErrors.userName)}
              aria-describedby={
                fieldErrors.userName ? "userName-error" : undefined
              }
            />

            {fieldErrors.userName && (
              <p className="field-error" id="userName-error">
                {fieldErrors.userName}
              </p>
            )}
          </div>

          <div className="form-field">
            <label className="form-label" htmlFor="password">
              Parola
            </label>

            <input
              className="form-input"
              id="password"
              name="password"
              type="password"
              value={credentials.password}
              onChange={handleChange}
              autoComplete="current-password"
              maxLength={256}
              required
              disabled={isSubmitting}
              aria-invalid={Boolean(fieldErrors.password)}
              aria-describedby={
                fieldErrors.password ? "password-error" : undefined
              }
            />

            {fieldErrors.password && (
              <p className="field-error" id="password-error">
                {fieldErrors.password}
              </p>
            )}
          </div>

          <button
            className="submit-button"
            type="submit"
            disabled={isSubmitting}
          >
            {isSubmitting ? "Giriş yapılıyor..." : "Giriş yap"}
          </button>
        </form>
      </section>
    </main>
  );
}
