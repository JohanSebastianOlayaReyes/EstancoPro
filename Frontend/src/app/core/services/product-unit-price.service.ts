import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ProductUnitPriceDto {
  productId: number;
  unitMeasureId: number;
  price: number;
  quantityPerBaseUnit: number;
  active?: boolean;
  createdAt?: string;
  updatedAt?: string;
  deletedAt?: string | null;
  productName?: string;
  unitMeasureName?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProductUnitPriceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/ProductUnitPrice`;

  getAll(): Observable<ProductUnitPriceDto[]> {
    return this.http.get<ProductUnitPriceDto[]>(this.apiUrl);
  }

  getById(productId: number, unitMeasureId: number): Observable<ProductUnitPriceDto> {
    return this.http.get<ProductUnitPriceDto>(`${this.apiUrl}/${productId}/${unitMeasureId}`);
  }

  create(productUnitPrice: ProductUnitPriceDto): Observable<ProductUnitPriceDto> {
    return this.http.post<ProductUnitPriceDto>(this.apiUrl, productUnitPrice);
  }

  update(productId: number, unitMeasureId: number, productUnitPrice: ProductUnitPriceDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${productId}/${unitMeasureId}`, productUnitPrice);
  }

  delete(productId: number, unitMeasureId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${productId}/${unitMeasureId}`);
  }

  getByProductName(productName: string): Observable<ProductUnitPriceDto[]> {
    return this.http.get<ProductUnitPriceDto[]>(`${this.apiUrl}/by-product/${productName}`);
  }

  getPriceByNames(productName: string, unitMeasureName: string): Observable<ProductUnitPriceDto | null> {
    const params = new HttpParams()
      .set('productName', productName)
      .set('unitMeasureName', unitMeasureName);
    return this.http.get<ProductUnitPriceDto | null>(`${this.apiUrl}/by-names`, { params });
  }
}
