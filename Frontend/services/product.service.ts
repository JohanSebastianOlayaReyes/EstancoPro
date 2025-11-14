import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProductDto } from '../models/product.model';

export interface AdjustStockRequest {
  productName: string;
  quantityChange: number;
  reason: string;
}

export interface StockByPresentationDto {
  unitMeasureName: string;
  availableQuantity: number;
  price: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Product`;

  // CRUD básico
  getAll(): Observable<ProductDto[]> {
    return this.http.get<ProductDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/${id}`);
  }

  create(product: ProductDto): Observable<ProductDto> {
    return this.http.post<ProductDto>(this.apiUrl, product);
  }

  update(id: number, product: ProductDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, product);
  }

  patch(id: number, product: Partial<ProductDto>): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, product);
  }

  deleteLogic(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/logic/${id}`);
  }

  deletePermanent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/permanent/${id}`);
  }

  // Métodos específicos
  getLowStockProducts(): Observable<ProductDto[]> {
    return this.http.get<ProductDto[]>(`${this.apiUrl}/low-stock`);
  }

  getByCategoryName(categoryName: string): Observable<ProductDto[]> {
    return this.http.get<ProductDto[]>(`${this.apiUrl}/by-category/${categoryName}`);
  }

  adjustStock(request: AdjustStockRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/adjust-stock`, request);
  }

  getStockByPresentations(productName: string): Observable<StockByPresentationDto[]> {
    return this.http.get<StockByPresentationDto[]>(`${this.apiUrl}/stock-by-presentations/${productName}`);
  }
}
