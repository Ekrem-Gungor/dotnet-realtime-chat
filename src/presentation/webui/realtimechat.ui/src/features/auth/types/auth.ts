export interface LoginCredentials {
  userName: string;
  password: string;
}

export interface AuthenticatedUser {
  userId: number;
  userName: string;
  email: string | null;
  roles: string[];
}
