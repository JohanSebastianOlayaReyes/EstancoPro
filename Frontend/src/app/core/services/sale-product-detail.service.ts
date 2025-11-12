import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SaleProductDetailDto } from '../models/sale.model';

@Injectable({
  providedIn: 'root'
})
export class SaleProductDetailService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/SaleProductDetail`;

  getAll(): Observable<SaleProductDetailDto[]> {
    return this.http.get<SaleProductDetailDto[]>(this.apiUrl);
  }

  getById(saleId: number, productId: number, unitMeasureId: number): Observable<SaleProductDetailDto> {
    return this.http.get<SaleProductDetailDto>(`${this.apiUrl}/${saleId}/${productId}/${unitMeasureId}`);
  }

  create(saleProductDetail: SaleProductDetailDto): Observable<SaleProductDetailDto> {
    return this.http.post<SaleProductDetailDto>(this.apiUrl, saleProductDetail);
  }

  update(saleId: number, productId: number, unitMeasureId: number, saleProductDetail: SaleProductDetailDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${saleId}/${productId}/${unitMeasureId}`, saleProductDetail);
  }

  delete(saleId: number, productId: number, unitMeasureId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${saleId}/${productId}/${unitMeasureId}`);
  }
}
