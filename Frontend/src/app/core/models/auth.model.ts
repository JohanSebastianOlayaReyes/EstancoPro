// Modelos de autenticación - coinciden con DTOs del backend

export interface LoginDto {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  email: string;
  roleName: string;
  userId: number;
  expiresAt: string;
  refreshTokenExpiresAt: string;
}

export interface AuthUser {
  userId: number;
  email: string;
  roleName: string;
  token: string;
  refreshToken: string;
  expiresAt: Date;
}

export interface RefreshTokenRequest {
  token: string;
  refreshToken: string;
}
