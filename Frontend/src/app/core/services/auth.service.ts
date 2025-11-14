import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { LoginDto, LoginResponse, AuthUser, RefreshTokenRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = `${environment.apiUrl}/Auth`;
  private currentUserSubject: BehaviorSubject<AuthUser | null>;
  public currentUser$: Observable<AuthUser | null>;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    const storedUser = this.getUserFromStorage();
    this.currentUserSubject = new BehaviorSubject<AuthUser | null>(storedUser);
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  /**
   * Obtiene el usuario actual
   */
  public get currentUserValue(): AuthUser | null {
    return this.currentUserSubject.value;
  }

  /**
   * Verifica si el usuario está autenticado
   */
  public get isAuthenticated(): boolean {
    const user = this.currentUserValue;
    if (!user) return false;

    // Verificar si el token expiró
    const expiresAt = new Date(user.expiresAt);
    return expiresAt > new Date();
  }

  /**
   * Login
   */
  login(credentials: LoginDto): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.API_URL}/login`, credentials).pipe(
      tap(response => {
        const user: AuthUser = {
          userId: response.userId,
          email: response.email,
          roleName: response.roleName,
          token: response.token,
          refreshToken: response.refreshToken,
          expiresAt: new Date(response.expiresAt)
        };

        this.saveUserToStorage(user);
        this.currentUserSubject.next(user);
      })
    );
  }

  /**
   * Logout
   */
  logout(): void {
    const user = this.currentUserValue;

    if (user) {
      const request: RefreshTokenRequest = {
        token: user.token,
        refreshToken: user.refreshToken
      };

      // Llamar al backend para revocar el token (no esperamos respuesta)
      this.http.post(`${this.API_URL}/logout`, request).subscribe();
    }

    this.clearUserFromStorage();
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  /**
   * Refrescar token
   */
  refreshToken(): Observable<LoginResponse> {
    const user = this.currentUserValue;

    if (!user) {
      throw new Error('No user logged in');
    }

    const request: RefreshTokenRequest = {
      token: user.token,
      refreshToken: user.refreshToken
    };

    return this.http.post<LoginResponse>(`${this.API_URL}/refresh`, request).pipe(
      tap(response => {
        const updatedUser: AuthUser = {
          userId: response.userId,
          email: response.email,
          roleName: response.roleName,
          token: response.token,
          refreshToken: response.refreshToken,
          expiresAt: new Date(response.expiresAt)
        };

        this.saveUserToStorage(updatedUser);
        this.currentUserSubject.next(updatedUser);
      })
    );
  }

  /**
   * Validar token actual
   */
  validateToken(): Observable<any> {
    return this.http.get(`${this.API_URL}/validate`);
  }

  /**
   * Guardar usuario en localStorage
   */
  private saveUserToStorage(user: AuthUser): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
  }

  /**
   * Obtener usuario de localStorage
   */
  private getUserFromStorage(): AuthUser | null {
    const userStr = localStorage.getItem('currentUser');
    if (!userStr) return null;

    try {
      const user = JSON.parse(userStr);
      // Verificar si expiró
      const expiresAt = new Date(user.expiresAt);
      if (expiresAt <= new Date()) {
        this.clearUserFromStorage();
        return null;
      }
      return user;
    } catch {
      return null;
    }
  }

  /**
   * Limpiar usuario de localStorage
   */
  private clearUserFromStorage(): void {
    localStorage.removeItem('currentUser');
  }

  /**
   * Obtener token de autorización
   */
  getAuthToken(): string | null {
    const user = this.currentUserValue;
    return user ? user.token : null;
  }

  /**
   * Verificar si tiene un rol específico
   */
  hasRole(roleName: string): boolean {
    const user = this.currentUserValue;
    return user?.roleName === roleName;
  }

  /**
   * Verificar si tiene uno de varios roles
   */
  hasAnyRole(roleNames: string[]): boolean {
    const user = this.currentUserValue;
    return user ? roleNames.includes(user.roleName) : false;
  }
}
