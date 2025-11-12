export interface CashSessionDto {
  id?: number;
  openedAt: string;
  closedAt?: string | null;
  openingAmount: number;
  closingAmount?: number;
  difference?: number;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
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
