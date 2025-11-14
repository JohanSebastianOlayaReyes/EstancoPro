export interface LoginDto {
  Email: string;
  Password: string;
}

export interface LoginResponseDto {
  token: string;
  refreshToken: string;
  expiresIn: number;
  email: string;
  roleName: string;
  userId: number;
}

export interface RefreshTokenRequestDto {
  refreshToken: string;
}

export interface UserDto {
  id: number;
  username: string;
  email: string;
  personId: number;
  state: boolean;
}
