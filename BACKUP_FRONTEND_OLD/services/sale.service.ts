import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SaleDto, CreateSaleDto } from '../models/sale.model';

export interface SalesReportDto {
  totalSales: number;
  totalRevenue: number;
  averageTicket: number;
  salesByStatus: { status: string; count: number; total: number }[];
}

@Injectable({
  providedIn: 'root'
})
export class SaleService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Sale`;

  // CRUD básico
  getAll(): Observable<SaleDto[]> {
    return this.http.get<SaleDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<SaleDto> {
    return this.http.get<SaleDto>(`${this.apiUrl}/${id}`);
  }

  create(sale: SaleDto): Observable<SaleDto> {
    return this.http.post<SaleDto>(this.apiUrl, sale);
  }

  /**
   * Crear una venta desde el POS
   * Este método es específico para el flujo de venta del punto de venta
   */
  createFromPOS(sale: CreateSaleDto): Observable<SaleDto> {
    return this.http.post<SaleDto>(this.apiUrl, sale);
  }

  update(id: number, sale: SaleDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, sale);
  }

  patch(id: number, sale: Partial<SaleDto>): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, sale);
  }

  deleteLogic(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/logic/${id}`);
  }

  deletePermanent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/permanent/${id}`);
  }

  // Métodos específicos
  finalizeSale(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/finalize`, {});
  }

  cancelSale(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/cancel`, {});
  }

  recalculateTotals(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/recalculate-totals`, {});
  }

  getByCashSession(cashSessionId: number): Observable<SaleDto[]> {
    return this.http.get<SaleDto[]>(`${this.apiUrl}/by-cash-session/${cashSessionId}`);
  }

  getByDateRange(from: Date, to: Date): Observable<SaleDto[]> {
    const params = new HttpParams()
      .set('from', from.toISOString())
      .set('to', to.toISOString());
    return this.http.get<SaleDto[]>(`${this.apiUrl}/by-date-range`, { params });
  }

  getByStatus(status: string): Observable<SaleDto[]> {
    const params = new HttpParams().set('status', status);
    return this.http.get<SaleDto[]>(`${this.apiUrl}/by-status`, { params });
  }

  getSalesReport(from: Date, to: Date): Observable<SalesReportDto> {
    const params = new HttpParams()
      .set('from', from.toISOString())
      .set('to', to.toISOString());
    return this.http.get<SalesReportDto>(`${this.apiUrl}/report`, { params });
  }

  // Métodos específicos para dashboards
  getTodaySales(): Observable<SaleDto[]> {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    return this.getByDateRange(today, tomorrow);
  }

  getMonthSales(): Observable<SaleDto[]> {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    return this.getByDateRange(firstDay, lastDay);
  }

  getSalesByUser(userId: number): Observable<SaleDto[]> {
    return this.http.get<SaleDto[]>(`${this.apiUrl}/by-user/${userId}`);
  }

  getTodayTotalRevenue(): Observable<number> {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    return new Observable(observer => {
      this.getByDateRange(today, tomorrow).subscribe({
        next: (sales) => {
          const total = sales.reduce((sum, sale) => sum + (sale.grandTotal || 0), 0);
          observer.next(total);
          observer.complete();
        },
        error: (err) => observer.error(err)
      });
    });
  }

  getTopProducts(limit: number = 5): Observable<{ productId: number; productName: string; totalQuantity: number; totalRevenue: number }[]> {
    return this.http.get<{ productId: number; productName: string; totalQuantity: number; totalRevenue: number }[]>(
      `${this.apiUrl}/top-products?limit=${limit}`
    );
  }
}
