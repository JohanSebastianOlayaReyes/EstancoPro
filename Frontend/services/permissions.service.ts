import { Injectable, inject } from '@angular/core';
import { AuthService } from './auth.service';
import {
  Permission,
  PermissionAction,
  ModuleName,
  RoleName,
  ROLE_PERMISSIONS,
  hasPermission
} from '../models/permission.model';

/**
 * Centralized permissions service
 * Provides methods to check user permissions for different modules and actions
 */
@Injectable({
  providedIn: 'root'
})
export class PermissionsService {
  private authService = inject(AuthService);

  /**
   * Get current user role
   */
  private getCurrentRole(): RoleName | null {
    const role = this.authService.getUserRole();
    return role as RoleName | null;
  }

  /**
   * Get all permissions for current user
   */
  getUserPermissions(): Permission[] {
    const role = this.getCurrentRole();
    if (!role) return [];
    return ROLE_PERMISSIONS[role] || [];
  }

  /**
   * Check if user can perform an action on a module
   */
  can(module: ModuleName, action: PermissionAction): boolean {
    const role = this.getCurrentRole();
    if (!role) return false;
    return hasPermission(role, module, action);
  }

  /**
   * Check if user can create in a module
   */
  canCreate(module: ModuleName): boolean {
    return this.can(module, 'create');
  }

  /**
   * Check if user can read in a module
   */
  canRead(module: ModuleName): boolean {
    return this.can(module, 'read');
  }

  /**
   * Check if user can update in a module
   */
  canUpdate(module: ModuleName): boolean {
    return this.can(module, 'update');
  }

  /**
   * Check if user can delete in a module
   */
  canDelete(module: ModuleName): boolean {
    return this.can(module, 'delete');
  }

  /**
   * Check if user has any permission on a module
   */
  hasAccessToModule(module: ModuleName): boolean {
    const permissions = this.getUserPermissions();
    return permissions.some(p => p.module === module && p.actions.length > 0);
  }

  /**
   * Get allowed actions for a specific module
   */
  getModuleActions(module: ModuleName): PermissionAction[] {
    const permissions = this.getUserPermissions();
    const modulePermission = permissions.find(p => p.module === module);
    return modulePermission ? modulePermission.actions : [];
  }

  /**
   * Role-specific permission checks
   */

  /**
   * Can user operate cash (open/close sessions)?
   * Only Cajero can operate cash
   */
  canOperateCash(): boolean {
    return this.authService.isCajero();
  }

  /**
   * Can user make sales?
   * Cajero and Vendedor can make sales, but NOT Administrador
   */
  canMakeSales(): boolean {
    const role = this.getCurrentRole();
    return role === 'Cajero' || role === 'Vendedor';
  }

  /**
   * Can user access POS?
   * Cajero and Vendedor can access POS
   */
  canAccessPOS(): boolean {
    return this.canMakeSales();
  }

  /**
   * Can user manage users?
   * Only Administrador can manage users
   */
  canManageUsers(): boolean {
    return this.authService.isAdmin();
  }

  /**
   * Can user manage products?
   * Only Administrador can manage (create/edit/delete) products
   */
  canManageProducts(): boolean {
    return this.authService.isAdmin();
  }

  /**
   * Can user manage purchases?
   * Only Administrador can manage purchases
   */
  canManagePurchases(): boolean {
    return this.authService.isAdmin();
  }

  /**
   * Can user view reports?
   * All roles can view reports
   */
  canViewReports(): boolean {
    return this.canRead('reports');
  }

  /**
   * Is user read-only?
   * Supervisor is read-only across all modules
   */
  isReadOnly(): boolean {
    return this.authService.isSupervisor();
  }
}
