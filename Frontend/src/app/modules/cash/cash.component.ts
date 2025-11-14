import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CashService } from '../../core/services/cash.service';
import { SaleService } from '../../core/services/sale.service';
import { AuthUser, CashSession, Sale } from '../../core/models';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Component({
  selector: 'app-cash',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent],
  templateUrl: './cash.component.html',
  styleUrls: ['./cash.component.scss']
})
export class CashComponent implements OnInit {
  Math = Math;  // Para usar Math.abs en el template
  currentUser: AuthUser | null = null;
  currentSession: CashSession | null = null;
  sessions: CashSession[] = [];
  sessionSales: Sale[] = [];  // Corregido typo: sessionsales -> sessionSales

  // Loading states
  loading = true;
  loadingSales = false;
  processingAction = false;

  // Open session modal
  showOpenModal = false;
  openingAmount = 0;
  openingObservations = '';

  // Close session modal
  showCloseModal = false;
  closingAmount = 0;
  closingObservations = '';

  // Session details modal
  showDetailsModal = false;
  selectedSession: CashSession | null = null;

  // Success modal
  showSuccessModal = false;
  successMessage = '';

  constructor(
    private router: Router,
    private authService: AuthService,
    private cashService: CashService,
    private saleService: SaleService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;

    // Subscribe to current session
    this.cashService.currentSession$.subscribe(session => {
      this.currentSession = session;
    });

    this.loadSessions();
  }

  loadSessions(): void {
    this.loading = true;
    this.cashService.getAll().subscribe({
      next: (sessions) => {
        this.sessions = sessions.sort((a, b) =>
          new Date(b.openedAt).getTime() - new Date(a.openedAt).getTime()
        );
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading sessions:', error);
        this.loading = false;
      }
    });
  }

  openOpenSessionModal(): void {
    this.openingAmount = 0;
    this.openingObservations = '';
    this.showOpenModal = true;
  }

  openSession(): void {
    if (this.openingAmount < 0) {
      alert('El monto de apertura debe ser mayor o igual a cero');
      return;
    }

    this.processingAction = true;

    // Usar el endpoint /open directamente que ya crea y abre la sesión
    this.cashService.openSessionSimple(this.openingAmount).subscribe({
      next: (session) => {
        this.processingAction = false;
        this.showOpenModal = false;
        this.successMessage = 'Sesión de caja abierta exitosamente';
        this.showSuccessModal = true;
        this.currentSession = session;
        this.loadSessions();
      },
      error: (error) => {
        console.error('Error opening session:', error);
        alert('Error al abrir sesión: ' + (error.error?.message || 'Error desconocido'));
        this.processingAction = false;
      }
    });
  }

  openCloseSessionModal(): void {
    if (!this.currentSession) {
      alert('No hay una sesión abierta para cerrar');
      return;
    }

    this.closingAmount = 0;
    this.closingObservations = '';
    this.loadSessionSales();
    this.showCloseModal = true;
  }

  loadSessionSales(): void {
    if (!this.currentSession || !this.currentSession.id) return;

    this.loadingSales = true;
    this.saleService.getByCashSession(this.currentSession.id).subscribe({
      next: (sales) => {
        this.sessionSales = sales.filter(s => s.status === 'Finalizada');
        this.loadingSales = false;
      },
      error: (error) => {
        console.error('Error loading sales:', error);
        this.loadingSales = false;
      }
    });
  }

  closeSession(): void {
    if (!this.currentSession) return;

    if (this.closingAmount < 0) {
      alert('El monto de cierre debe ser mayor o igual a cero');
      return;
    }

    this.processingAction = true;

    // Usar directamente el endpoint /close que ya actualiza todo
    const closeData = {
      closingAmount: this.closingAmount
    };

    this.cashService.close(this.currentSession.id!, closeData).subscribe({
      next: () => {
        this.processingAction = false;
        this.showCloseModal = false;
        this.successMessage = 'Sesión de caja cerrada exitosamente';
        this.showSuccessModal = true;
        this.currentSession = null;
        this.loadSessions();
      },
      error: (error) => {
        console.error('Error closing session:', error);
        alert('Error al cerrar sesión: ' + (error.error?.message || 'Error desconocido'));
        this.processingAction = false;
      }
    });
  }

  viewSessionDetails(session: CashSession): void {
    if (!session.id) return;

    this.selectedSession = session;
    this.showDetailsModal = true;

    // Load sales for this session
    this.loadingSales = true;
    this.saleService.getByCashSession(session.id).subscribe({
      next: (sales) => {
        this.sessionSales = sales.filter(s => s.status === 'Finalizada');
        this.loadingSales = false;
      },
      error: (error) => {
        console.error('Error loading sales:', error);
        this.loadingSales = false;
      }
    });
  }

  calculateTotalSales(): number {
    return this.sessionSales.reduce((sum, sale) => sum + sale.totalAmount, 0);
  }

  calculateExpectedClosing(): number {
    if (!this.currentSession) return 0;
    return this.currentSession.openingAmount + this.calculateTotalSales();
  }

  calculateDifference(): number {
    return this.closingAmount - this.calculateExpectedClosing();
  }

  getSessionStatus(session: CashSession): string {
    if (session.id === this.currentSession?.id) return 'Abierta';
    if (session.closedAt) return 'Cerrada';
    return 'Inactiva';
  }

  getSessionStatusClass(session: CashSession): string {
    const status = this.getSessionStatus(session);
    if (status === 'Abierta') return 'bg-success/10 text-success';
    if (status === 'Cerrada') return 'bg-gray-100 text-gray-600';
    return 'bg-warning/10 text-warning';
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}
