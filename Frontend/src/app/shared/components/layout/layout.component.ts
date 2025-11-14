import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CashService } from '../../../core/services/cash.service';
import { AuthUser, CashSession } from '../../../core/models';

interface MenuItem {
  label: string;
  iconSvg: string; // SVG path del icono
  route: string;
  roles: string[]; // Roles que pueden ver este item
}

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {
  currentUser: AuthUser | null = null;
  currentSession: CashSession | null = null;
  sidebarOpen = true;

  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      iconSvg: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6',
      route: '/dashboard',
      roles: ['Administrador', 'Cajero', 'Vendedor', 'Inventario', 'Gerente']
    },
    {
      label: 'Punto de Venta',
      iconSvg: 'M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z',
      route: '/pos',
      roles: ['Administrador', 'Cajero', 'Vendedor']
    },
    {
      label: 'Productos',
      iconSvg: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
      route: '/products',
      roles: ['Administrador', 'Inventario', 'Gerente']
    },
    {
      label: 'Categorías',
      iconSvg: 'M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10',
      route: '/categories',
      roles: ['Administrador', 'Inventario']
    },
    {
      label: 'Compras',
      iconSvg: 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z',
      route: '/purchases',
      roles: ['Administrador', 'Inventario', 'Gerente']
    },
    {
      label: 'Proveedores',
      iconSvg: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4',
      route: '/suppliers',
      roles: ['Administrador', 'Inventario']
    },
    {
      label: 'Caja',
      iconSvg: 'M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z',
      route: '/cash',
      roles: ['Administrador', 'Cajero', 'Gerente']
    },
    {
      label: 'Clientes',
      iconSvg: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z',
      route: '/customers',
      roles: ['Administrador', 'Cajero', 'Vendedor', 'Gerente']
    },
    {
      label: 'Reportes',
      iconSvg: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z',
      route: '/reports',
      roles: ['Administrador', 'Gerente']
    },
    {
      label: 'Usuarios',
      iconSvg: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z',
      route: '/admin/users',
      roles: ['Administrador']
    },
    {
      label: 'Configuración',
      iconSvg: 'M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z M15 12a3 3 0 11-6 0 3 3 0 016 0z',
      route: '/admin/settings',
      roles: ['Administrador']
    }
  ];

  constructor(
    private authService: AuthService,
    private cashService: CashService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;

    this.cashService.currentSession$.subscribe(session => {
      this.currentSession = session;
    });
  }

  get visibleMenuItems(): MenuItem[] {
    if (!this.currentUser) return [];

    return this.menuItems.filter(item =>
      item.roles.includes(this.currentUser!.roleName)
    );
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  logout(): void {
    this.authService.logout();
  }

  isActive(route: string): boolean {
    return this.router.url === route;
  }
}
