import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product, Category, UnitMeasure, ProductAlert } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly API_URL = `${environment.apiUrl}/Product`;
  private readonly CATEGORY_URL = `${environment.apiUrl}/Category`;
  private readonly UNIT_URL = `${environment.apiUrl}/UnitMeasure`;
  private readonly ALERT_URL = `${environment.apiUrl}/ProductAlert`;

  constructor(private http: HttpClient) {}

  // ============== PRODUCTOS ==============

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.API_URL);
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.API_URL}/${id}`);
  }

  create(product: Partial<Product>): Observable<Product> {
    return this.http.post<Product>(this.API_URL, product);
  }

  update(id: number, product: Partial<Product>): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/${id}`, product);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  // ============== CATEGORÍAS ==============

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.CATEGORY_URL);
  }

  createCategory(category: Partial<Category>): Observable<Category> {
    return this.http.post<Category>(this.CATEGORY_URL, category);
  }

  updateCategory(id: number, category: Partial<Category>): Observable<void> {
    return this.http.put<void>(`${this.CATEGORY_URL}/${id}`, category);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.CATEGORY_URL}/${id}`);
  }

  // ============== UNIDADES DE MEDIDA ==============

  getUnitMeasures(): Observable<UnitMeasure[]> {
    return this.http.get<UnitMeasure[]>(this.UNIT_URL);
  }

  // ============== ALERTAS DE STOCK ==============

  getAlerts(): Observable<ProductAlert[]> {
    return this.http.get<ProductAlert[]>(this.ALERT_URL);
  }

  getUnreadAlerts(): Observable<ProductAlert[]> {
    return this.http.get<ProductAlert[]>(`${this.ALERT_URL}/unread`);
  }

  markAlertAsRead(id: number): Observable<void> {
    return this.http.post<void>(`${this.ALERT_URL}/${id}/mark-read`, {});
  }
}
