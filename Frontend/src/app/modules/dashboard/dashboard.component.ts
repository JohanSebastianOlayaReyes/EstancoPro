import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CashService } from '../../core/services/cash.service';
import { DashboardService, DashboardStats } from '../../core/services/dashboard.service';
import { AuthUser, CashSession } from '../../core/models';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ModalComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: AuthUser | null = null;
  currentSession: CashSession | null = null;
  stats: DashboardStats = {
    totalSalesToday: 0,
    salesCountToday: 0,
    totalProfit: 0,
    totalProducts: 0,
    lowStockProducts: 0,
    totalExpenses: 0
  };
  loading = true;

  // Modal states
  showCashSessionModal = false;
  showNoCashModal = false;

  constructor(
    private authService: AuthService,
    private cashService: CashService,
    private dashboardService: DashboardService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;

    // Suscribirse a la sesión actual de caja
    this.cashService.currentSession$.subscribe(session => {
      this.currentSession = session;

      // Mostrar modales automáticamente según el estado de la sesión
      if (session) {
        this.showCashSessionModal = true;
        this.showNoCashModal = false;
      } else {
        this.showCashSessionModal = false;
        this.showNoCashModal = true;
      }
    });

    // Cargar estadísticas del dashboard
    this.loadDashboardStats();
  }

  loadDashboardStats(): void {
    this.loading = true;
    this.dashboardService.getDashboardStats().subscribe({
      next: (stats) => {
        this.stats = stats;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error cargando estadísticas:', error);
        this.loading = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }

  goToPOS(): void {
    this.router.navigate(['/pos']);
  }

  goToProducts(): void {
    this.router.navigate(['/products']);
  }

  goToCash(): void {
    this.router.navigate(['/cash']);
  }
}
