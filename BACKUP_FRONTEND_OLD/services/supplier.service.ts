import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SupplierDto } from '../models/supplier.model';

@Injectable({
  providedIn: 'root'
})
export class SupplierService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Supplier`;

  getAll(): Observable<SupplierDto[]> {
    return this.http.get<SupplierDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<SupplierDto> {
    return this.http.get<SupplierDto>(`${this.apiUrl}/${id}`);
  }

  create(supplier: SupplierDto): Observable<SupplierDto> {
    return this.http.post<SupplierDto>(this.apiUrl, supplier);
  }

  update(supplier: SupplierDto): Observable<SupplierDto> {
    return this.http.put<SupplierDto>(`${this.apiUrl}/${supplier.id}`, supplier);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
