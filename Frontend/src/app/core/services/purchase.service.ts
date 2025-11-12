import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PurchaseDto } from '../models/purchase.model';

export interface ReceivePurchaseRequest {
  payInCash: boolean;
  cashSessionId?: number;
}

export interface CancelPurchaseRequest {
  reason: string;
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Purchase`;

  // CRUD básico
  getAll(): Observable<PurchaseDto[]> {
    return this.http.get<PurchaseDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<PurchaseDto> {
    return this.http.get<PurchaseDto>(`${this.apiUrl}/${id}`);
  }

  create(purchase: PurchaseDto): Observable<PurchaseDto> {
    return this.http.post<PurchaseDto>(this.apiUrl, purchase);
  }

  update(id: number, purchase: PurchaseDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, purchase);
  }

  patch(id: number, purchase: Partial<PurchaseDto>): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, purchase);
  }

  deleteLogic(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/logic/${id}`);
  }

  deletePermanent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/permanent/${id}`);
  }

  // Métodos específicos
  receivePurchase(id: number, request: ReceivePurchaseRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/receive`, request);
  }

  cancelPurchase(id: number, request: CancelPurchaseRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/cancel`, request);
  }

  getBySupplierName(supplierName: string): Observable<PurchaseDto[]> {
    return this.http.get<PurchaseDto[]>(`${this.apiUrl}/by-supplier/${supplierName}`);
  }

  getByDateRange(from: Date, to: Date): Observable<PurchaseDto[]> {
    const params = new HttpParams()
      .set('from', from.toISOString())
      .set('to', to.toISOString());
    return this.http.get<PurchaseDto[]>(`${this.apiUrl}/by-date-range`, { params });
  }

  getByStatus(status: boolean): Observable<PurchaseDto[]> {
    const params = new HttpParams().set('status', status.toString());
    return this.http.get<PurchaseDto[]>(`${this.apiUrl}/by-status`, { params });
  }
}
