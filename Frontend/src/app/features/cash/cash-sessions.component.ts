import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CashSessionService } from '../../core/services/cash-session.service';
import { CashSessionDto } from '../../core/models/cash.model';

@Component({
  selector: 'app-cash-sessions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container">
      <h1>Gestión de Sesiones de Caja</h1>

      @if (openSession()) {
        <div class="open-session-card">
          <h2>Sesión Abierta</h2>
          <div class="session-info">
            <div class="info-item">
              <span class="label">ID:</span>
              <span class="value">#{{ openSession()!.id }}</span>
            </div>
            <div class="info-item">
              <span class="label">Apertura:</span>
              <span class="value">{{ openSession()!.openingAmount | currency }}</span>
            </div>
            <div class="info-item">
              <span class="label">Fecha Apertura:</span>
              <span class="value">{{ openSession()!.openedAt | date:'short' }}</span>
            </div>
          </div>
          @if (balance()) {
            <div class="balance-info">
              <h3>Balance Actual</h3>
              <div class="balance-grid">
                <div class="balance-item">
                  <span class="label">Esperado:</span>
                  <span class="value">{{ balance()!.expectedAmount | currency }}</span>
                </div>
                <div class="balance-item">
                  <span class="label">Ventas:</span>
                  <span class="value">{{ balance()!.totalSales | currency }}</span>
                </div>
                <div class="balance-item">
                  <span class="label">Egresos:</span>
                  <span class="value">{{ balance()!.totalExpenses | currency }}</span>
                </div>
              </div>
            </div>
          }
          <div class="close-session-form">
            <h3>Cerrar Sesión</h3>
            <div class="form-group">
              <label>Monto de Cierre:</label>
              <input
                type="number"
                [(ngModel)]="closingAmount"
                placeholder="Monto contado en caja"
                min="0"
                step="0.01"
              />
            </div>
            <button class="btn-close" (click)="closeSession()">Cerrar Sesión</button>
          </div>
        </div>
      } @else {
        <div class="open-session-form">
          <h2>Abrir Nueva Sesión</h2>
          <div class="form-group">
            <label>Monto de Apertura:</label>
            <input
              type="number"
              [(ngModel)]="openingAmount"
              placeholder="Monto inicial en caja"
              min="0"
              step="0.01"
            />
          </div>
          <button (click)="openNewSession()">Abrir Sesión</button>
        </div>
      }

      <div class="sessions-list">
        <h2>Historial de Sesiones</h2>
        <table>
          <thead>
            <tr>
              <th>Apertura</th>
              <th>Cierre</th>
              <th>Monto Inicial</th>
              <th>Monto Final</th>
              <th>Diferencia</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            @for (session of sessions(); track session.id) {
              <tr [attr.data-session-id]="session.id">
                <td>{{ session.openedAt | date:'short' }}</td>
                <td>{{ session.closedAt ? (session.closedAt | date:'short') : 'Abierta' }}</td>
                <td>{{ session.openingAmount | currency }}</td>
                <td>{{ session.closingAmount ? (session.closingAmount | currency) : '-' }}</td>
                <td [class.positive]="session.difference && session.difference > 0"
                    [class.negative]="session.difference && session.difference < 0">
                  {{ session.difference ? (session.difference | currency) : '-' }}
                </td>
                <td>
                  <button (click)="viewSessionDetails(session.id!)">Ver Balance</button>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>

      @if (error()) {
        <div class="alert alert-error">{{ error() }}</div>
      }
      @if (success()) {
        <div class="alert alert-success">{{ success() }}</div>
      }
    </div>
  `,
  styles: [`
    .container { padding: 20px; max-width: 1200px; margin: 0 auto; }
    .open-session-card, .open-session-form {
      background: white;
      padding: 20px;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      margin-bottom: 20px;
    }
    .session-info { display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px; margin: 20px 0; }
    .info-item { text-align: center; padding: 15px; background: #f5f5f5; border-radius: 8px; }
    .info-item .label { display: block; font-size: 12px; color: #666; margin-bottom: 5px; }
    .info-item .value { display: block; font-size: 20px; font-weight: bold; color: #2196F3; }
    .balance-info { margin: 20px 0; padding: 15px; background: #e3f2fd; border-radius: 8px; }
    .balance-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 15px; margin-top: 15px; }
    .balance-item { text-align: center; }
    .balance-item .label { display: block; font-size: 12px; color: #666; }
    .balance-item .value { display: block; font-size: 18px; font-weight: bold; color: #1976D2; }
    .close-session-form, .open-session-form { border-top: 1px solid #eee; padding-top: 20px; }
    .form-group { margin-bottom: 15px; }
    .form-group label { display: block; margin-bottom: 5px; font-weight: 500; }
    .form-group input { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 4px; font-size: 16px; }
    button { padding: 12px 24px; border: none; border-radius: 4px; background: #2196F3; color: white; cursor: pointer; font-size: 14px; }
    button:hover { background: #1976D2; }
    .btn-close { background: #f44336; }
    .btn-close:hover { background: #d32f2f; }
    .sessions-list { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
    table { width: 100%; border-collapse: collapse; margin-top: 20px; }
    th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }
    th { background: #f5f5f5; font-weight: 600; }
    .positive { color: #4CAF50; font-weight: bold; }
    .negative { color: #f44336; font-weight: bold; }
    .alert { padding: 12px; border-radius: 4px; margin-top: 15px; }
    .alert-error { background: #ffebee; color: #c62828; border: 1px solid #ef5350; }
    .alert-success { background: #e8f5e9; color: #2e7d32; border: 1px solid #66bb6a; }
  `]
})
export class CashSessionsComponent implements OnInit {
  openSession = signal<CashSessionDto | null>(null);
  sessions = signal<CashSessionDto[]>([]);
  balance = signal<any>(null);
  error = signal<string | null>(null);
  success = signal<string | null>(null);

  openingAmount = 0;
  closingAmount = 0;

  constructor(private cashSessionService: CashSessionService) {}

  ngOnInit() {
    this.loadOpenSession();
    this.loadSessions();
  }

  loadOpenSession() {
    this.cashSessionService.getOpenSession().subscribe({
      next: (session) => {
        this.openSession.set(session);
        if (session) {
          this.loadBalance(session.id!);
        }
      },
      error: (err) => console.error(err)
    });
  }

  loadSessions() {
    this.cashSessionService.getAll().subscribe({
      next: (sessions) => this.sessions.set(sessions),
      error: (err) => this.error.set('Error al cargar sesiones')
    });
  }

  loadBalance(sessionId: number) {
    this.cashSessionService.getSessionBalance(sessionId).subscribe({
      next: (balance) => this.balance.set(balance),
      error: (err) => console.error(err)
    });
  }

  openNewSession() {
    if (this.openingAmount <= 0) {
      this.error.set('El monto de apertura debe ser mayor a 0');
      return;
    }

    this.cashSessionService.openSession({ openingAmount: this.openingAmount }).subscribe({
      next: (session) => {
        this.success.set(`Sesión #${session.id} abierta exitosamente`);
        this.openSession.set(session);
        this.openingAmount = 0;
        this.loadSessions();
      },
      error: (err) => {
        this.error.set('Error al abrir sesión: ' + (err.error?.message || err.message));
      }
    });
  }

  closeSession() {
    const session = this.openSession();
    if (!session || this.closingAmount < 0) {
      this.error.set('Datos inválidos');
      return;
    }

    if (!confirm('¿Está seguro de cerrar esta sesión?')) return;

    this.cashSessionService.closeSession(session.id!, { closingAmount: this.closingAmount }).subscribe({
      next: (response) => {
        this.success.set(response.message + ` - ${response.status}: ${response.difference}`);
        this.openSession.set(null);
        this.balance.set(null);
        this.closingAmount = 0;
        this.loadSessions();
      },
      error: (err) => {
        this.error.set('Error al cerrar sesión: ' + (err.error?.message || err.message));
      }
    });
  }

  viewSessionDetails(sessionId: number) {
    this.cashSessionService.getSessionBalance(sessionId).subscribe({
      next: (balance) => {
        alert(`Balance de Sesión #${sessionId}\n\nEsperado: ${balance.expectedAmount}\nActual: ${balance.actualAmount}\nDiferencia: ${balance.difference}\nVentas: ${balance.totalSales}\nEgresos: ${balance.totalExpenses}`);
      },
      error: (err) => this.error.set('Error al obtener balance')
    });
  }
}
