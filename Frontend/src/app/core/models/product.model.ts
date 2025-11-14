// Modelos de productos

export interface Product {
  id: number;
  code: string;
  name: string;
  description: string;
  photo?: string | null;
  purchasePrice: number;
  salePrice: number;
  stockQuantity: number;
  minStock: number;
  maxStock: number;
  expirationDate: string | null;
  categoryId: number;
  unitMeasureId: number;
  state: boolean;
  categoryName?: string;
  unitMeasureName?: string;
  category?: Category;
  unitMeasure?: UnitMeasure;
}

export interface Category {
  id: number;
  name: string;
  description: string;
  state: boolean;
}

export interface UnitMeasure {
  id: number;
  name: string;
  abbreviation: string;
  state: boolean;
}

export interface ProductAlert {
  id: number;
  productId: number;
  alertType: string;
  message: string;
  severity: string;
  isRead: boolean;
  generatedAt: string;
  productName?: string;
  currentStock?: number;
  reorderPoint?: number;
}
