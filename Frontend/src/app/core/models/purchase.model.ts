export interface PurchaseDto {
  id?: number;
  orderedAt?: string | null;
  receivedAt?: string | null;
  status: boolean;
  totalCost: number;
  supplierId: number;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
  supplierName?: string;
}

export interface PurchaseProductDetailDto {
  id?: number;
  quantity: number;
  unitCost: number;
  subtotal: number;
  purchaseId: number;
  productId: number;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
  productName?: string;
}
