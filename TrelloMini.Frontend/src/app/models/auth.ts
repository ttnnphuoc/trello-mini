export interface User {
  id: number;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  createdAt: Date;
  isEmailVerified: boolean;
}

export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: Date;
  user: User;
}

export interface AuthState {
  isLoggedIn: boolean;
  user: User | null;
  token: string | null;
}