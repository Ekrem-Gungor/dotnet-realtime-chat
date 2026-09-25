import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type PropsWithChildren,
} from "react";
import { ApiError } from "../../../shared/api/ApiError";
import { authService } from "../api/authService";
import type { AuthenticatedUser, LoginCredentials } from "../types/auth";
import {
  AuthContext,
  type AuthContextValue,
  type AuthStatus,
} from "./authContext";

export function AuthProvider({ children }: PropsWithChildren) {
  const [status, setStatus] = useState<AuthStatus>("checking");
  const [user, setUser] = useState<AuthenticatedUser | null>(null);
  const [sessionError, setSessionError] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    async function restoreSession() {
      try {
        const authenticatedUser = await authService.getSession();

        if (!isActive) {
          return;
        }

        setUser(authenticatedUser);
        setStatus("authenticated");
        setSessionError(null);
      } catch (error) {
        if (!isActive) {
          return;
        }

        setUser(null);
        setStatus("anonymous");

        if (error instanceof ApiError && error.status === 401) {
          setSessionError(null);
          return;
        }

        setSessionError("Oturum bilgisi doğrulanamadı. Lütfen tekrar deneyin.");
      }
    }

    void restoreSession();

    return () => {
      isActive = false;
    };
  }, []);

  const login = useCallback(async (credentials: LoginCredentials) => {
    const authenticatedUser = await authService.login(credentials);

    setUser(authenticatedUser);
    setStatus("authenticated");
    setSessionError(null);
  }, []);

  const logout = useCallback(async () => {
    await authService.logout();

    setUser(null);
    setStatus("anonymous");
    setSessionError(null);
  }, []);

  const contextValue = useMemo<AuthContextValue>(
    () => ({
      status,
      user,
      sessionError,
      login,
      logout,
    }),
    [status, user, sessionError, login, logout],
  );

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
}
