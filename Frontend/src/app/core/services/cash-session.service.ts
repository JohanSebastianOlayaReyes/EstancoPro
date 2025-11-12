import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CashSessionDto } from '../models/cash.model';

export interface OpenSessionRequest {
  openingAmount: number;
}

export interface CloseSessionRequest {
  closingAmount: number;
}

export interface CloseSessionResponse {
  message: string;
  difference: number;
  status: string;
}

export interface SessionBalanceDto {
  expectedAmount: number;
  actualAmount: number;
  difference: number;
  totalSales: number;
  totalExpenses: number;
}

@Injectable({
  providedIn: 'root'
})
export class CashSessionService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/CashSession`;

  // CRUD básico
  getAll(): Observable<CashSessionDto[]> {
    return this.http.get<CashSessionDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<CashSessionDto> {
    return this.http.get<CashSessionDto>(`${this.apiUrl}/${id}`);
  }

  create(cashSession: CashSessionDto): Observable<CashSessionDto> {
    return this.http.post<CashSessionDto>(this.apiUrl, cashSession);
  }

  update(id: number, cashSession: CashSessionDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, cashSession);
  }

  patch(id: number, cashSession: Partial<CashSessionDto>): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, cashSession);
  }

  deleteLogic(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/logic/${id}`);
  }

  deletePermanent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/permanent/${id}`);
  }

  // Métodos específicos
  openSession(request: OpenSessionRequest): Observable<CashSessionDto> {
    return this.http.post<CashSessionDto>(`${this.apiUrl}/open`, request);
  }

  closeSession(id: number, request: CloseSessionRequest): Observable<CloseSessionResponse> {
    return this.http.post<CloseSessionResponse>(`${this.apiUrl}/${id}/close`, request);
  }

  getOpenSession(): Observable<CashSessionDto | null> {
    return this.http.get<CashSessionDto | null>(`${this.apiUrl}/open`);
  }

  getByDateRange(from: Date, to: Date): Observable<CashSessionDto[]> {
    const params = new HttpParams()
      .set('from', from.toISOString())
      .set('to', to.toISOString());
    return this.http.get<CashSessionDto[]>(`${this.apiUrl}/by-date-range`, { params });
  }

  getSessionBalance(id: number): Observable<SessionBalanceDto> {
    return this.http.get<SessionBalanceDto>(`${this.apiUrl}/${id}/balance`);
  }
}
