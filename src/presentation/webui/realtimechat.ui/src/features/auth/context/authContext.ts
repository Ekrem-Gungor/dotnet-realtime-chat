import { createContext } from "react";
import type { AuthenticatedUser, LoginCredentials } from "../types/auth";

export type AuthStatus = "checking" | "authenticated" | "anonymous";

export interface AuthContextValue {
  status: AuthStatus;
  user: AuthenticatedUser | null;
  sessionError: string | null;
  login: (credentials: LoginCredentials) => Promise<void>;
  logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined,
);
