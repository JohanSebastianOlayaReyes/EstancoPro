// Modelos de compras

export interface Purchase {
  id: number;
  invoiceNumber: string;
  orderedAt: string;
  receivedAt?: string;
  status: 'Ordered' | 'Received' | 'Cancelled';
  subtotal: number;
  taxTotal: number;
  grandTotal: number;
  supplierId: number;
  active: boolean;
  supplier?: Supplier;
  purchaseProductDetails?: PurchaseProductDetail[];
}

export interface PurchaseProductDetail {
  id: number;
  purchaseId: number;
  productId: number;
  quantity: number;
  unitCost: number;
  subtotal: number;
  taxAmount: number;
  active: boolean;
  productName?: string;
}

export interface Supplier {
  id: number;
  name: string;
  contactName: string;
  phone: string;
  email: string;
  address: string;
  active: boolean;
}

export interface ReceivePurchaseRequest {
  payInCash: boolean;
  cashSessionId?: number;
}

export interface CancelPurchaseRequest {
  reason: string;
}
