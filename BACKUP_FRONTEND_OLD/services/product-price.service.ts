import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProductPriceDto, CreateProductPriceDto, UpdateProductPriceDto } from '../models/product-price.model';

@Injectable({
  providedIn: 'root'
})
export class ProductPriceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/ProductPrice`;

  getAll(): Observable<ProductPriceDto[]> {
    return this.http.get<ProductPriceDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<ProductPriceDto> {
    return this.http.get<ProductPriceDto>(`${this.apiUrl}/${id}`);
  }

  getByProductId(productId: number): Observable<ProductPriceDto[]> {
    return this.http.get<ProductPriceDto[]>(`${this.apiUrl}/by-product/${productId}`);
  }

  create(productPrice: CreateProductPriceDto): Observable<ProductPriceDto> {
    return this.http.post<ProductPriceDto>(this.apiUrl, productPrice);
  }

  update(id: number, productPrice: UpdateProductPriceDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, productPrice);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
