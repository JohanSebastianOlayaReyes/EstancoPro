export interface CategoryDto {
  id?: number;
  name: string;
  description: string;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
}
