export interface UserDto {
  id?: number;
  email: string;
  fullName?: string;
  password?: string;
  personId: number;
  rolId: number;
  active?: boolean;
}

export interface CreateUserDto {
  email: string;
  password: string;
  username: string;
  rolId: number;
}

export interface UserListDto {
  id: number;
  email: string;
  username: string;
  roleName: string;
  active: boolean;
}
