export interface ProductPriceDto {
  id?: number;
  productId: number;
  unitMeasureId: number;
  conversionFactor: number;
  unitPrice: number;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
  productName?: string;
  unitMeasureName?: string;
}

export interface CreateProductPriceDto {
  productId: number;
  unitMeasureId: number;
  conversionFactor: number;
  unitPrice: number;
}

export interface UpdateProductPriceDto extends CreateProductPriceDto {
  id: number;
  active?: boolean;
}
