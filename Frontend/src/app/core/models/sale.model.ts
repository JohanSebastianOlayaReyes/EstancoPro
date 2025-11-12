export interface SaleDto {
  id?: number;
  soldAt: string;
  status: string;
  subtotal: number;
  taxTotal: number;
  grandTotal: number;
  cashSessionId: number;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
}

export interface SaleProductDetailDto {
  id?: number;
  quantity: number;
  unitPrice: number;
  subtotal: number;
  saleId: number;
  productId: number;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
  productName?: string;
}
