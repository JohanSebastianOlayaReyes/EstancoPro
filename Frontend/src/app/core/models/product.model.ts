export interface ProductDto {
  id?: number;
  name: string;
  unitCost: number;
  unitPrice: number;
  taxRate?: number | null;
  stockOnHand: number;
  reorderPoint: number;
  categoryId: number;
  unitMeasureId: number;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
  categoryName?: string;
  unitMeasureName?: string;
}
