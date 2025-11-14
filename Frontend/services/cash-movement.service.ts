import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CashMovementDto } from '../models/cash.model';

@Injectable({
  providedIn: 'root'
})
export class CashMovementService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/CashMovement`;

  getAll(): Observable<CashMovementDto[]> {
    return this.http.get<CashMovementDto[]>(this.apiUrl);
  }

  getById(cashSessionId: number, at: string): Observable<CashMovementDto> {
    return this.http.get<CashMovementDto>(`${this.apiUrl}/${cashSessionId}/${at}`);
  }

  create(cashMovement: CashMovementDto): Observable<CashMovementDto> {
    return this.http.post<CashMovementDto>(this.apiUrl, cashMovement);
  }

  update(cashSessionId: number, at: string, cashMovement: CashMovementDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${cashSessionId}/${at}`, cashMovement);
  }

  delete(cashSessionId: number, at: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${cashSessionId}/${at}`);
  }
}
