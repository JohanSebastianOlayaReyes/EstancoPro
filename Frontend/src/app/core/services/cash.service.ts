import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CashSession, CashMovement, CashSessionBalance, OpenSessionRequest, CloseSessionRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class CashService {
  private readonly API_URL = `${environment.apiUrl}/CashSession`;
  private readonly MOVEMENT_URL = `${environment.apiUrl}/CashMovement`;

  // Observable para la sesión activa actual
  private currentSessionSubject = new BehaviorSubject<CashSession | null>(null);
  public currentSession$ = this.currentSessionSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadOpenSession();
  }

  /**
   * Cargar sesión abierta al iniciar
   */
  private loadOpenSession(): void {
    this.getOpenSession().subscribe({
      next: (session) => this.currentSessionSubject.next(session),
      error: () => this.currentSessionSubject.next(null)
    });
  }

  /**
   * Obtener sesión actual
   */
  get currentSession(): CashSession | null {
    return this.currentSessionSubject.value;
  }

  /**
   * Verificar si hay sesión abierta
   */
  get hasOpenSession(): boolean {
    return this.currentSession !== null;
  }

  // ============== SESIONES DE CAJA ==============

  /**
   * Abrir nueva sesión - Usa el endpoint /open directamente
   */
  openSession(request: OpenSessionRequest): Observable<CashSession> {
    return this.http.post<CashSession>(`${this.API_URL}/open`, request).pipe(
      tap(session => this.currentSessionSubject.next(session))
    );
  }

  /**
   * Abrir nueva sesión con solo el monto
   */
  openSessionSimple(openingAmount: number): Observable<CashSession> {
    return this.http.post<CashSession>(`${this.API_URL}/open`, { openingAmount }).pipe(
      tap(session => this.currentSessionSubject.next(session))
    );
  }

  /**
   * Cerrar sesión
   */
  closeSession(id: number, request: CloseSessionRequest): Observable<{ message: string; difference: number; status: string }> {
    return this.http.post<{ message: string; difference: number; status: string }>(
      `${this.API_URL}/${id}/close`,
      request
    ).pipe(
      tap(() => this.currentSessionSubject.next(null))
    );
  }

  /**
   * Obtener sesión abierta actual
   */
  getOpenSession(): Observable<CashSession> {
    return this.http.get<CashSession>(`${this.API_URL}/open`);
  }

  /**
   * Obtener balance de una sesión
   */
  getSessionBalance(id: number): Observable<CashSessionBalance> {
    return this.http.get<CashSessionBalance>(`${this.API_URL}/${id}/balance`);
  }

  /**
   * Obtener sesiones por rango de fechas
   */
  getByDateRange(from: string, to: string): Observable<CashSession[]> {
    return this.http.get<CashSession[]>(`${this.API_URL}/by-date-range`, {
      params: { from, to }
    });
  }

  getAllSessions(): Observable<CashSession[]> {
    return this.http.get<CashSession[]>(this.API_URL);
  }

  /**
   * Obtener todas las sesiones
   */
  getAll(): Observable<CashSession[]> {
    return this.http.get<CashSession[]>(this.API_URL);
  }

  /**
   * Crear nueva sesión
   */
  create(session: Partial<CashSession>): Observable<CashSession> {
    return this.http.post<CashSession>(this.API_URL, session);
  }

  /**
   * Abrir sesión por ID
   */
  open(id: number): Observable<CashSession> {
    return this.http.post<CashSession>(`${this.API_URL}/${id}/open`, {}).pipe(
      tap(session => this.currentSessionSubject.next(session))
    );
  }

  /**
   * Cerrar sesión por ID
   */
  close(id: number, data: { closingAmount: number; observations?: string }): Observable<CashSession> {
    return this.http.post<CashSession>(`${this.API_URL}/${id}/close`, data).pipe(
      tap(() => this.currentSessionSubject.next(null))
    );
  }

  /**
   * Actualizar sesión
   */
  update(id: number, session: Partial<CashSession>): Observable<CashSession> {
    return this.http.put<CashSession>(`${this.API_URL}/${id}`, session);
  }

  // ============== MOVIMIENTOS DE CAJA ==============

  getMovements(): Observable<CashMovement[]> {
    return this.http.get<CashMovement[]>(this.MOVEMENT_URL);
  }

  createMovement(movement: Partial<CashMovement>): Observable<CashMovement> {
    return this.http.post<CashMovement>(this.MOVEMENT_URL, movement);
  }
}
