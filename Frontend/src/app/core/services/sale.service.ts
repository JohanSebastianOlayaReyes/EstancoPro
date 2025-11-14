import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Sale, SaleProductDetail, PaymentMethod, Customer } from '../models';

@Injectable({
  providedIn: 'root'
})
export class SaleService {
  private readonly API_URL = `${environment.apiUrl}/Sale`;
  private readonly DETAIL_URL = `${environment.apiUrl}/SaleProductDetail`;
  private readonly PAYMENT_URL = `${environment.apiUrl}/PaymentMethod`;
  private readonly CUSTOMER_URL = `${environment.apiUrl}/Customer`;

  constructor(private http: HttpClient) {}

  // ============== VENTAS ==============

  getAll(): Observable<Sale[]> {
    return this.http.get<Sale[]>(this.API_URL);
  }

  getById(id: number): Observable<Sale> {
    return this.http.get<Sale>(`${this.API_URL}/${id}`);
  }

  create(sale: Partial<Sale>): Observable<Sale> {
    return this.http.post<Sale>(this.API_URL, sale);
  }

  update(id: number, sale: Partial<Sale>): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/${id}`, sale);
  }

  /**
   * CRÍTICO: Finalizar venta
   * Valida stock, descuenta inventario, registra en caja
   */
  finalizeSale(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.API_URL}/${id}/finalize`, {});
  }

  cancelSale(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.API_URL}/${id}/cancel`, {});
  }

  recalculateTotals(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.API_URL}/${id}/recalculate-totals`, {});
  }

  getByCashSession(cashSessionId: number): Observable<Sale[]> {
    return this.http.get<Sale[]>(`${this.API_URL}/by-cash-session/${cashSessionId}`);
  }

  getByStatus(status: string): Observable<Sale[]> {
    return this.http.get<Sale[]>(`${this.API_URL}/by-status`, { params: { status } });
  }

  getSalesReport(from: string, to: string): Observable<any> {
    return this.http.get(`${this.API_URL}/report`, { params: { from, to } });
  }

  // ============== DETALLE DE VENTA ==============

  addProduct(detail: Partial<SaleProductDetail>): Observable<SaleProductDetail> {
    return this.http.post<SaleProductDetail>(this.DETAIL_URL, detail);
  }

  updateProduct(id: number, detail: Partial<SaleProductDetail>): Observable<void> {
    return this.http.put<void>(`${this.DETAIL_URL}/${id}`, detail);
  }

  removeProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.DETAIL_URL}/${id}`);
  }

  // ============== MÉTODOS DE PAGO ==============

  getPaymentMethods(): Observable<PaymentMethod[]> {
    return this.http.get<PaymentMethod[]>(this.PAYMENT_URL);
  }

  // ============== CLIENTES ==============

  getCustomers(): Observable<Customer[]> {
    return this.http.get<Customer[]>(this.CUSTOMER_URL);
  }

  getCustomer(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.CUSTOMER_URL}/${id}`);
  }

  createCustomer(customer: Partial<Customer>): Observable<Customer> {
    return this.http.post<Customer>(this.CUSTOMER_URL, customer);
  }
}
