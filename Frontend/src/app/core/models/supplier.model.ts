export interface SupplierDto {
  id?: number;
  name: string;
  phone: string;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
}
