// Modelos de caja

export interface CashSession {
  id?: number;
  openedAt: string;  // Nombre del backend
  closedAt?: string | null;  // Nombre del backend
  openingAmount: number;
  closingAmount?: number;
  active: boolean;  // Nombre del backend
  createAt?: string;
  updateAt?: string;
  deleteAt?: string | null;
}

export interface CashMovement {
  cashSessionId: number;
  at: string;
  typeMovement: 'Income' | 'Expense';
  amount: number;
  reason: string;
  referenceType: 'Sale' | 'Purchase' | 'Adjustment' | 'Other';
  referenceId?: number;
}

export interface CashSessionBalance {
  cashSessionId: number;
  openingAmount: number;
  totalIncome: number;
  totalExpense: number;
  expectedClosing: number;
  movements: CashMovement[];
}

export interface OpenSessionRequest {
  openingAmount: number;
}

export interface CloseSessionRequest {
  closingAmount: number;
}
