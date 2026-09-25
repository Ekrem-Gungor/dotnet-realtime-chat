import { apiRequest } from "../../../shared/api/httpClient";
import type { AuthenticatedUser, LoginCredentials } from "../types/auth";

async function getSession(): Promise<AuthenticatedUser> {
  return apiRequest<AuthenticatedUser>("/api/Auth/session");
}

async function login(
  credentials: LoginCredentials,
): Promise<AuthenticatedUser> {
  await apiRequest<unknown>("/api/Auth/login", {
    method: "POST",
    json: credentials,
  });

  return getSession();
}

async function logout(): Promise<void> {
  await apiRequest<void>("/api/Auth/logout", {
    method: "POST",
  });
}

export const authService = Object.freeze({
  getSession,
  login,
  logout,
});
