import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { IconComponent } from '../../../shared/components/icon.component';
import { EstancoCardComponent } from '../../../shared/components/estanco-card.component';
import { SidebarMenuComponent, MenuSection } from '../../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../../shared/config/menu-sections';
import { UserService } from '../../../core/services/user.service';
import { ProductService } from '../../../core/services/product.service';
import { SupplierService } from '../../../core/services/supplier.service';
import { SaleService } from '../../../core/services/sale.service';

interface AdminCard {
  title: string;
  icon: string;
  description: string;
  route: string;
  color: string;
  stats?: string;
}

interface ChartDataPoint {
  label: string;
  value: number;
  percentage: number;
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, IconComponent, EstancoCardComponent, SidebarMenuComponent],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private userService = inject(UserService);
  private productService = inject(ProductService);
  private supplierService = inject(SupplierService);
  private saleService = inject(SaleService);

  currentUser = this.authService.currentUser;

  usersCount = 0;
  productsCount = 0;
  suppliersCount = 0;
  salesTotalRevenue = 0;

  searchQuery = '';
  chartData: ChartDataPoint[] = [];
  users: any[] = [];
  currentDate = new Date().toLocaleDateString('es-ES', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });

  ngOnInit(): void {
    this.menuSections = ADMIN_MENU_SECTIONS;
    this.loadCounts();
    this.generateChartData();
  }

  private generateChartData() {
    this.saleService.getAll().subscribe({
      next: (sales) => {
        const hourMap: { [key: string]: number } = {};
        const hours = ['08:00', '10:00', '12:00', '14:00', '16:00', '18:00', '20:00', '22:00'];
        hours.forEach(h => hourMap[h] = 0);
        sales.forEach(sale => {
          if (sale.soldAt) {
            const saleDate = new Date(sale.soldAt);
            const hour = saleDate.getHours();
            const hourLabel = hour < 10 ? `0${hour}:00` : `${hour}:00`;
            if (hourMap.hasOwnProperty(hourLabel)) {
              hourMap[hourLabel] += sale.grandTotal || 0;
            }
          }
        });
        this.chartData = hours.map(hour => ({ label: hour, value: hourMap[hour], percentage: 0 }));
        this.normalizeChartData();
      },
      error: (err) => {
        console.error('Error cargando datos del gráfico:', err);
        const hours = ['08:00', '10:00', '12:00', '14:00', '16:00', '18:00', '20:00', '22:00'];
        this.chartData = hours.map(hour => ({ label: hour, value: Math.floor(Math.random() * 300) + 100, percentage: 0 }));
        this.normalizeChartData();
      }
    });
  }

  private loadCounts() {
    this.userService.getAll().subscribe({ next: v => { this.usersCount = v?.length ?? 0; this.users = v?.slice(0,5) ?? []; }, error: () => { this.usersCount = 0; this.users = []; } });
    this.productService.getAll().subscribe({ next: v => { this.productsCount = v?.length ?? 0; }, error: () => { this.productsCount = 0; } });
    this.supplierService.getAll().subscribe({ next: v => { this.suppliersCount = v?.length ?? 0; }, error: () => { this.suppliersCount = 0; } });
    this.saleService.getAll().subscribe({ next: v => { const sales = v ?? []; this.salesTotalRevenue = sales.reduce((sum, sale) => sum + (sale.grandTotal || 0), 0); }, error: () => { this.salesTotalRevenue = 0; } });
  }

  private normalizeChartData() {
    const max = Math.max(...this.chartData.map(d => d.value));
    this.chartData = this.chartData.map(d => ({ ...d, percentage: (d.value / max) * 100 }));
  }

  onSearch(q: string) {
    this.searchQuery = q;
    if (q.trim()) {
      this.router.navigate(['/admin/products'], { queryParams: { q } });
    }
  }

  systemCards: AdminCard[] = [
    { title: 'Gestión de Usuarios', icon: 'users', description: 'Crear, editar y administrar usuarios del sistema', route: '/admin/users', color: 'primary', stats: 'Control total de acceso' },
    { title: 'Roles y Permisos', icon: 'lock', description: 'Configurar roles y permisos de usuario', route: '/admin/roles', color: 'info', stats: 'Seguridad del sistema' }
  ];

  inventoryCards: AdminCard[] = [ /* omitted for brevity */ ];
  operationsCards: AdminCard[] = [ /* omitted for brevity */ ];
  reportsCards: AdminCard[] = [ /* omitted for brevity */ ];

  menuSections: MenuSection[] = [];

  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}
