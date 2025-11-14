// Modelos de ventas

export interface Sale {
  id?: number;
  cashSessionId: number;
  customerId?: number | null;
  saleDate: string;
  totalAmount: number;
  taxAmount: number;
  observations?: string | null;
  status: 'Pendiente' | 'Finalizada' | 'Cancelada';
  state: boolean;
  customer?: Customer;
  saleProductDetails?: SaleProductDetail[];
  salePayments?: SalePayment[];
}

export interface SaleProductDetail {
  id?: number;
  saleId?: number;
  productId: number;
  quantity: number;
  unitPrice: number;
  subtotal: number;
  state: boolean;
  productName?: string;
}

export interface SalePayment {
  id?: number;
  saleId: number;
  paymentMethodId: number;
  amount: number;
  transactionDate: string;
  reference?: string;
  state: boolean;
  paymentMethodName?: string;
}

export interface Customer {
  id?: number;
  documentType: string;
  documentNumber: string;
  firstName: string;
  lastName: string;
  phone?: string;
  email?: string;
  address?: string;
  loyaltyPoints?: number;
  state: boolean;
  balance?: number; // Saldo de deuda del cliente
  name?: string; // Nombre completo para compatibilidad
  active?: boolean; // Estado activo para compatibilidad
  createdAt?: string;
}

export interface PaymentMethod {
  id: number;
  methodName: string;
  description?: string;
  requiresReference: boolean;
  state: boolean;
}

export interface DebtTransaction {
  id: number;
  customerId: number;
  amount: number;
  type: 'debt' | 'payment'; // 'debt' aumenta la deuda, 'payment' la reduce
  description: string;
  date: string;
  customerName?: string;
}
