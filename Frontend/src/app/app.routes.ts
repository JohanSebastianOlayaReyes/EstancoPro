import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { publicGuard } from './core/guards/public.guard';
import { LayoutComponent } from './shared/components/layout/layout.component';

export const routes: Routes = [
  {
    path: 'auth/login',
    canActivate: [publicGuard],
    loadComponent: () => import('./modules/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./modules/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'pos',
        loadComponent: () => import('./modules/pos/pos.component').then(m => m.PosComponent)
      },
      {
        path: 'products',
        loadComponent: () => import('./modules/products/products.component').then(m => m.ProductsComponent)
      },
      {
        path: 'cash',
        loadComponent: () => import('./modules/cash/cash.component').then(m => m.CashComponent)
      },
      {
        path: 'categories',
        loadComponent: () => import('./modules/categories/categories.component').then(m => m.CategoriesComponent)
      },
      {
        path: 'purchases',
        loadComponent: () => import('./modules/purchases/purchases.component').then(m => m.PurchasesComponent)
      },
      {
        path: 'suppliers',
        loadComponent: () => import('./modules/suppliers/suppliers.component').then(m => m.SuppliersComponent)
      },
      {
        path: 'customers',
        loadComponent: () => import('./modules/customers/customers.component').then(m => m.CustomersComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
