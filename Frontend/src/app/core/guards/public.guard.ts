import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Guard para rutas públicas
 * Redirige al dashboard si ya está autenticado
 */
export const publicGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated) {
    // Ya está autenticado - redirigir al dashboard
    router.navigate(['/dashboard']);
    return false;
  }

  return true;
};
