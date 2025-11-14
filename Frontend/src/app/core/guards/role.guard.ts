import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Guard de roles
 * Protege rutas que requieren roles específicos
 * Uso en route: canActivate: [roleGuard], data: { roles: ['Administrador', 'Cajero'] }
 */
export const roleGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const requiredRoles = route.data['roles'] as string[];

  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  if (authService.hasAnyRole(requiredRoles)) {
    return true;
  }

  // No tiene el rol requerido
  router.navigate(['/unauthorized']);
  return false;
};
