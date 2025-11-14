import { Injectable, inject, signal, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable, tap } from 'rxjs';
import { ApiService } from './api.service';
import { LoginDto, LoginResponseDto, RefreshTokenRequestDto } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiService = inject(ApiService);
  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);

  // Signals for reactive state management
  isAuthenticated = signal<boolean>(this.hasToken());
  currentUser = signal<any>(this.getUserFromStorage());

  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'current_user';

  /**
   * Login user
   */
  login(credentials: LoginDto): Observable<LoginResponseDto> {
    return this.apiService.post<LoginResponseDto>('Auth/login', credentials).pipe(
      tap(response => {
        this.setSession(response);
      })
    );
  }

  /**
   * Logout user
   */
  logout(): void {
    if (this.isBrowser) {
      localStorage.removeItem(this.TOKEN_KEY);
      localStorage.removeItem(this.REFRESH_TOKEN_KEY);
      localStorage.removeItem(this.USER_KEY);
    }
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
  }

  /**
   * Refresh token
   */
  refreshToken(): Observable<LoginResponseDto> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    const request: RefreshTokenRequestDto = { refreshToken };
    return this.apiService.post<LoginResponseDto>('Auth/refresh-token', request).pipe(
      tap(response => {
        this.setSession(response);
      })
    );
  }

  /**
   * Get access token
   */
  getToken(): string | null {
    if (!this.isBrowser) return null;
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Get refresh token
   */
  getRefreshToken(): string | null {
    if (!this.isBrowser) return null;
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Check if user has token
   */
  private hasToken(): boolean {
    if (!this.isBrowser) return false;
    return !!this.getToken();
  }

  /**
   * Set session data
   */
  private setSession(authResult: LoginResponseDto): void {
    if (this.isBrowser) {
      localStorage.setItem(this.TOKEN_KEY, authResult.token);
      localStorage.setItem(this.REFRESH_TOKEN_KEY, authResult.refreshToken);

      const user = {
        userId: authResult.userId,
        email: authResult.email,
        roleName: authResult.roleName
      };

      localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    }

    const user = {
      userId: authResult.userId,
      email: authResult.email,
      roleName: authResult.roleName
    };

    this.isAuthenticated.set(true);
    this.currentUser.set(user);
  }

  /**
   * Get user from storage
   */
  private getUserFromStorage(): any {
    if (!this.isBrowser) return null;
    const userStr = localStorage.getItem(this.USER_KEY);
    return userStr ? JSON.parse(userStr) : null;
  }

  /**
   * Check if token is expired
   */
  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiryTime = payload.exp * 1000; // Convert to milliseconds
      return Date.now() >= expiryTime;
    } catch (e) {
      return true;
    }
  }

  /**
   * Decode JWT token to get payload
   */
  private decodeToken(): any {
    const token = this.getToken();
    if (!token) return null;

    try {
      return JSON.parse(atob(token.split('.')[1]));
    } catch (e) {
      console.error('Error decoding token:', e);
      return null;
    }
  }

  /**
   * Get user role from JWT token
   * @returns Role name: 'Administrador', 'Cajero', 'Vendedor', 'Supervisor'
   */
  getUserRole(): string | null {
    const payload = this.decodeToken();
    if (!payload) return null;

    // The role claim is 'role' in the JWT
    return payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
  }

  /**
   * Check if user has one of the specified roles
   * @param roles Array of allowed roles
   * @returns true if user has any of the specified roles
   */
  hasRole(roles: string[]): boolean {
    const userRole = this.getUserRole();
    if (!userRole) return false;
    return roles.includes(userRole);
  }

  /**
   * Check if user is Administrador
   */
  isAdmin(): boolean {
    return this.getUserRole() === 'Administrador';
  }

  /**
   * Check if user is Cajero
   */
  isCajero(): boolean {
    return this.getUserRole() === 'Cajero';
  }

  /**
   * Check if user is Vendedor
   */
  isVendedor(): boolean {
    return this.getUserRole() === 'Vendedor';
  }

  /**
   * Check if user is Supervisor
   */
  isSupervisor(): boolean {
    return this.getUserRole() === 'Supervisor';
  }

  /**
   * Get user ID from JWT token
   */
  getUserId(): string | null {
    const payload = this.decodeToken();
    if (!payload) return null;
    return payload.sub || payload.userId || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || null;
  }

  /**
   * Get user email from JWT token
   */
  getUserEmail(): string | null {
    const payload = this.decodeToken();
    if (!payload) return null;
    return payload.email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || null;
  }
}
