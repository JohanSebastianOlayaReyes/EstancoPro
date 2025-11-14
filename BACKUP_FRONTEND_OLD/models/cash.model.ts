export interface CashSessionDto {
  id?: number;
  openedAt: string;
  closedAt?: string | null;
  openingAmount: number;
  closingAmount?: number;
  difference?: number;
  status?: string; // "open" | "closed"
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
  expectedAmount?: number;
  notes?: string;
}

export interface OpenCashSessionRequest {
  openingAmount: number;
}

export interface CloseCashSessionRequest {
  closingAmount: number;
}

export interface CloseCashSessionResponse {
  message: string;
  difference: number;
  status: 'balanced' | 'surplus' | 'shortage';
}

export interface CashSessionBalance {
  sessionId: number;
  openingAmount: number;
  totalSales: number;
  totalExpenses: number;
  expectedAmount: number;
  actualAmount?: number;
  difference?: number;
  movements: number;
}

export interface CashMovementDto {
  id?: number;
  transactionType: string;
  amount: number;
  description?: string | null;
  cashSessionId: number;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
}
