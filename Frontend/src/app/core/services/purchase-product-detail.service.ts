import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PurchaseProductDetailDto } from '../models/purchase.model';

@Injectable({
  providedIn: 'root'
})
export class PurchaseProductDetailService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/PurchaseProductDetail`;

  getAll(): Observable<PurchaseProductDetailDto[]> {
    return this.http.get<PurchaseProductDetailDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<PurchaseProductDetailDto> {
    return this.http.get<PurchaseProductDetailDto>(`${this.apiUrl}/${id}`);
  }

  create(purchaseProductDetail: PurchaseProductDetailDto): Observable<PurchaseProductDetailDto> {
    return this.http.post<PurchaseProductDetailDto>(this.apiUrl, purchaseProductDetail);
  }

  update(id: number, purchaseProductDetail: PurchaseProductDetailDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, purchaseProductDetail);
  }

  patch(id: number, purchaseProductDetail: Partial<PurchaseProductDetailDto>): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, purchaseProductDetail);
  }

  deleteLogic(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/logic/${id}`);
  }

  deletePermanent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/permanent/${id}`);
  }
}
