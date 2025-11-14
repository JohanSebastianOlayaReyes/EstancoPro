/**
 * Permission action types
 */
export type PermissionAction = 'create' | 'read' | 'update' | 'delete';

/**
 * Module permission definition
 */
export interface Permission {
  module: string;
  actions: PermissionAction[];
}

/**
 * Role types in the system
 */
export type RoleName = 'Administrador' | 'Cajero' | 'Vendedor' | 'Supervisor';

/**
 * Module names in the system
 */
export type ModuleName =
  | 'users'
  | 'roles'
  | 'products'
  | 'categories'
  | 'suppliers'
  | 'purchases'
  | 'sales'
  | 'cash'
  | 'reports'
  | 'unit-measures'
  | 'product-prices';

/**
 * Complete permissions mapping for each role
 * Based on backend analysis:
 * - Administrador: Manages products, users, purchases, views sales and reports (NO cash operations, NO sales)
 * - Cajero: Opens/closes cash, makes sales, receives money
 * - Vendedor: Only makes sales (NO cash operations)
 * - Supervisor: Views reports and supervises operations (read-only)
 */
export const ROLE_PERMISSIONS: Record<RoleName, Permission[]> = {
  'Administrador': [
    { module: 'users', actions: ['create', 'read', 'update', 'delete'] },
    { module: 'roles', actions: ['create', 'read', 'update', 'delete'] },
    { module: 'products', actions: ['create', 'read', 'update', 'delete'] },
    { module: 'categories', actions: ['create', 'read', 'update', 'delete'] },
    { module: 'suppliers', actions: ['create', 'read', 'update', 'delete'] },
    { module: 'purchases', actions: ['create', 'read', 'update', 'delete'] },
    { module: 'sales', actions: ['read'] }, // Can view but NOT create sales
    { module: 'cash', actions: ['read'] }, // Can view but NOT operate cash
    { module: 'reports', actions: ['read'] },
    { module: 'unit-measures', actions: ['create', 'read', 'update', 'delete'] },
    { module: 'product-prices', actions: ['create', 'read', 'update', 'delete'] }
  ],
  'Cajero': [
    { module: 'products', actions: ['read'] },
    { module: 'categories', actions: ['read'] },
    { module: 'sales', actions: ['create', 'read'] }, // Can create and view sales
    { module: 'cash', actions: ['create', 'read', 'update'] }, // Can open/close cash
    { module: 'reports', actions: ['read'] }
  ],
  'Vendedor': [
    { module: 'products', actions: ['read'] },
    { module: 'categories', actions: ['read'] },
    { module: 'sales', actions: ['create', 'read'] }, // Can create and view sales
    { module: 'cash', actions: ['read'] }, // Can view but NOT operate cash
    { module: 'reports', actions: ['read'] }
  ],
  'Supervisor': [
    { module: 'products', actions: ['read'] },
    { module: 'categories', actions: ['read'] },
    { module: 'suppliers', actions: ['read'] },
    { module: 'purchases', actions: ['read'] },
    { module: 'sales', actions: ['read'] },
    { module: 'cash', actions: ['read'] },
    { module: 'reports', actions: ['read'] },
    { module: 'users', actions: ['read'] }
  ]
};

/**
 * Helper function to get permissions for a specific role
 */
export function getPermissionsForRole(role: RoleName): Permission[] {
  return ROLE_PERMISSIONS[role] || [];
}

/**
 * Helper function to check if a role has a specific permission
 */
export function hasPermission(
  role: RoleName,
  module: ModuleName,
  action: PermissionAction
): boolean {
  const permissions = ROLE_PERMISSIONS[role];
  if (!permissions) return false;

  const modulePermission = permissions.find(p => p.module === module);
  if (!modulePermission) return false;

  return modulePermission.actions.includes(action);
}
