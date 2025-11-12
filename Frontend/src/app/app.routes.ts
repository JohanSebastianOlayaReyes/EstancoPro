import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/pages/login.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { AdminDashboardComponent } from './features/admin/dashboard/admin-dashboard.component';
import { AdminUsersCompactComponent } from './features/admin/users/components/admin-users-compact.component';
import { AdminRolesCompactComponent } from './features/admin/roles/admin-roles-compact.component';
import { AdminProductsComponent } from './features/admin/admin-products/admin-products.component';
import { AdminCategoriesCompactComponent } from './features/admin/categories/admin-categories-compact.component';
import { AdminSuppliersCompactComponent } from './features/admin/suppliers/admin-suppliers-compact.component';
import { AdminUnitMeasuresCompactComponent } from './features/admin/unit-measures/admin-unit-measures-compact.component';
import { PosComponent } from './features/sales/pos.component';
import { SalesListComponent } from './features/sales/sales-list.component';
import { PurchasesComponent } from './features/purchases/purchases.component';
import { CashSessionsComponent } from './features/cash/cash-sessions.component';
import { authGuard, loginGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [loginGuard]
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]
  },
  {
    path: 'pos',
    component: PosComponent,
    canActivate: [authGuard]
  },
  {
    path: 'sales',
    component: SalesListComponent,
    canActivate: [authGuard]
  },
  {
    path: 'purchases',
    component: PurchasesComponent,
    canActivate: [authGuard]
  },
  {
    path: 'cash-sessions',
    component: CashSessionsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    component: AdminDashboardComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin/users',
    component: AdminUsersCompactComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin/roles',
    component: AdminRolesCompactComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin/products',
    component: AdminProductsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin/categories',
    component: AdminCategoriesCompactComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin/suppliers',
    component: AdminSuppliersCompactComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin/unit-measures',
    component: AdminUnitMeasuresCompactComponent,
    canActivate: [authGuard]
  }
];
