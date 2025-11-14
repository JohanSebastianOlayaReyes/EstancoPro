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
  paymentMethod?: string;
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

/**
 * CreateSaleDto representa los datos necesarios para crear una venta
 */
export interface CreateSaleDto {
  cashSessionId?: number;  // Opcional para vendedores sin caja
  subtotal: number;
  taxTotal: number;
  grandTotal: number;
  paymentMethod: string;
  status: string;  // 'Pending', 'Finalized', 'Cancelled'
  saleDetails: CreateSaleDetailDto[];
}

/**
 * CreateSaleDetailDto representa un detalle de producto en la venta
 */
export interface CreateSaleDetailDto {
  productId: number;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}
