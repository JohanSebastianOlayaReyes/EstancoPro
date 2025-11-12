import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ButtonComponent } from '../../shared/components/button.component';
import { EstancoCardComponent } from '../../shared/components/estanco-card.component';
// AppHeaderComponent removed - not present in shared/components

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, ButtonComponent, RouterLink, EstancoCardComponent],
  template: `
    <div class="dashboard-container">
  <div class="dashboard-container">

      <main class="dashboard-content">
        <div class="cards-row">
          <app-estanco-card variant="accent">
            <h4>Ventas</h4>
            <div class="big">$12,430</div>
          </app-estanco-card>
          <app-estanco-card variant="info">
            <h4>Gastos</h4>
            <div class="big">$4,210</div>
          </app-estanco-card>
          <app-estanco-card variant="primary">
            <h4>Beneficio</h4>
            <div class="big">$8,220</div>
          </app-estanco-card>
        </div>

        <app-estanco-card class="welcome-card">
          <h2>¡Bienvenido a EstancoPro!</h2>
          <p class="muted">Sistema de gestión de estanco completamente funcional.</p>

          <div class="status-info">
            <div class="status-item">
              <span class="status-label">Estado de autenticación:</span>
              <span class="status-value success">✓ Conectado</span>
            </div>
            <div class="status-item">
              <span class="status-label">Token:</span>
              <span class="status-value mono">{{ getTokenPreview() }}</span>
            </div>
            <div class="status-item">
              <span class="status-label">Backend:</span>
              <span class="status-value success">✓ Conectado a http://localhost:5000/api</span>
            </div>
          </div>

          @if (isAdmin()) {
            <div class="admin-section">
              <h3>Panel de Administración</h3>
              <p>Tienes acceso completo al sistema</p>
              <a routerLink="/admin">
                <app-button variant="primary" [fullWidth]="true">
                  🛠️ Ir al Panel de Admin
                </app-button>
              </a>
            </div>
          }

          <div class="next-steps">
            <h3>Próximos módulos a implementar:</h3>
            <ul>
              <li>📊 Dashboard con KPIs en tiempo real</li>
              <li>🛒 Punto de Venta (POS)</li>
              <li>📦 Gestión de Inventario</li>
              <li>💰 Gestión de Caja</li>
              <li>🛍️ Módulo de Compras</li>
            </ul>
          </div>
        </app-estanco-card>
      </main>
    </div>
  `,
  styles: [`
    .dashboard-container {
      min-height: 100vh;
      background-color: var(--color-background);
    }

    .dashboard-header {
      background-color: var(--color-surface);
      border-bottom: 1px solid var(--color-border);
      padding: var(--space-4) var(--space-6);
      box-shadow: var(--shadow-sm);
    }

    .header-content {
      max-width: 1400px;
      margin: 0 auto;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .header-left {
      h1 {
        font-size: var(--font-size-2xl);
        color: var(--color-primary);
        margin-bottom: var(--space-1);
      }

      p {
        font-size: var(--font-size-sm);
        color: var(--color-text-secondary);
        margin: 0;

        strong {
          color: var(--color-text-primary);
        }
      }
    }

    .dashboard-content {
      max-width: 1400px;
      margin: 0 auto;
      padding: var(--space-6);
    }

    .cards-row { display:flex; gap:16px; margin-bottom: var(--space-6); }
    .cards-row app-estanco-card { flex:1; padding: 18px; }
    .cards-row .big { font-size: 1.6rem; font-weight: 700; margin-top: 8px; }

    .welcome-card {
      background-color: var(--color-surface);
      border-radius: var(--radius-lg);
      padding: var(--space-8);
      box-shadow: var(--shadow-md);

      h2 {
        color: var(--color-primary);
        margin-bottom: var(--space-2);
      }

      > p {
        color: var(--color-text-secondary);
        margin-bottom: var(--space-6);
      }
    }

    .status-info {
      background-color: var(--color-success-light);
      border-radius: var(--radius-md);
      padding: var(--space-4);
      margin-bottom: var(--space-6);
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
    }

    .status-item {
      display: flex;
      align-items: center;
      gap: var(--space-2);
    }

    .status-label {
      font-size: var(--font-size-sm);
      color: var(--color-text-secondary);
      font-weight: var(--font-weight-medium);
    }

    .status-value {
      font-size: var(--font-size-sm);
      color: var(--color-text-primary);

      &.success {
        color: var(--color-success);
        font-weight: var(--font-weight-semibold);
      }

      &.mono {
        font-family: var(--font-mono);
        font-size: var(--font-size-xs);
        background-color: var(--color-surface-secondary);
        padding: var(--space-1) var(--space-2);
        border-radius: var(--radius-sm);
      }
    }

    .next-steps {
      h3 {
        color: var(--color-text-primary);
        margin-bottom: var(--space-3);
        font-size: var(--font-size-lg);
      }

      ul {
        list-style: none;
        padding: 0;
        margin: 0;
        display: grid;
        gap: var(--space-2);

        li {
          padding: var(--space-3);
          background-color: var(--color-surface-secondary);
          border-radius: var(--radius-md);
          color: var(--color-text-secondary);
          font-size: var(--font-size-sm);
        }
      }
    }

    @media (max-width: 768px) {
      .header-content {
        flex-direction: column;
        align-items: flex-start;
        gap: var(--space-4);
      }

      .dashboard-content {
        padding: var(--space-4);
      }

      .welcome-card {
        padding: var(--space-4);
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit() {
    // Si es administrador, redirigir al panel de admin
    if (this.isAdmin()) {
      this.router.navigate(['/admin']);
    }
  }

  isAdmin(): boolean {
    const user = this.authService.currentUser();
    return user?.roleName === 'Administrador';
  }

  getTokenPreview(): string {
    const token = this.authService.getToken();
    if (!token) return 'No disponible';
    return token.substring(0, 20) + '...';
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
