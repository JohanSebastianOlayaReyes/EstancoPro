import { ProductDto } from './product.model';

/**
 * CartItem representa un producto en el carrito de compras
 */
export interface CartItem {
  product: ProductDto;
  quantity: number;
  unitPrice: number;
  subtotal: number;
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
  status: string;  // 'Finalized' para ventas completadas
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

/**
 * PaymentMethod representa los métodos de pago disponibles
 */
export type PaymentMethod = 'Efectivo' | 'Tarjeta' | 'Transferencia';

/**
 * CashPaymentResult representa el resultado del modal de pago en efectivo
 */
export interface CashPaymentResult {
  confirmed: boolean;
  amountReceived?: number;
  change?: number;
}
