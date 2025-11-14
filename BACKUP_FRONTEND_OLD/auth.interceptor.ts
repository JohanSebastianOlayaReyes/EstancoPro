import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * HTTP Interceptor to add JWT token to requests and handle auth errors
 *
 * This interceptor:
 * 1. Adds the JWT token to the Authorization header of all HTTP requests
 * 2. Handles 401 (Unauthorized) errors by redirecting to login
 * 3. Handles 403 (Forbidden) errors by showing an error message
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Get the auth token
  const token = authService.getToken();

  // Clone the request and add the authorization header if token exists
  let authReq = req;
  if (token && !authService.isTokenExpired()) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  // Handle the request and catch errors
  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Unauthorized - token expired or invalid
        console.error('Unauthorized request - redirecting to login');

        // Clear auth data
        authService.logout();

        // Redirect to login
        router.navigate(['/login'], {
          queryParams: { returnUrl: router.url, reason: 'session-expired' }
        });

        // Show error message
        alert('Tu sesión ha expirado. Por favor, inicia sesión nuevamente.');
      } else if (error.status === 403) {
        // Forbidden - user doesn't have permission
        console.error('Forbidden - insufficient permissions');

        // Show error message
        alert('No tienes permisos para realizar esta acción.');

        // Optionally redirect to dashboard
        // router.navigate(['/dashboard']);
      }

      // Re-throw the error so it can be handled by the calling code
      return throwError(() => error);
    })
  );
};
