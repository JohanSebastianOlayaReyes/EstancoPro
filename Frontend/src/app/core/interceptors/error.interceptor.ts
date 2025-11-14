import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Interceptor de errores HTTP
 * Maneja errores 401 (no autorizado) y otros errores globales
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Token expirado o inválido - redirigir al login
        authService.logout();
        router.navigate(['/auth/login'], {
          queryParams: { returnUrl: router.url }
        });
      }

      // Devolver el error para que los componentes lo manejen
      return throwError(() => error);
    })
  );
};
