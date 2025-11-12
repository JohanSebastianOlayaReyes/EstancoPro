import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SaleService, SalesReportDto } from '../../core/services/sale.service';
import { SaleDto } from '../../core/models/sale.model';

@Component({
  selector: 'app-sales-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container">
      <h1>Historial de Ventas</h1>

      <div class="filters">
        <div class="filter-group">
          <label>Desde:</label>
          <input type="date" [(ngModel)]="fromDate" />
        </div>
        <div class="filter-group">
          <label>Hasta:</label>
          <input type="date" [(ngModel)]="toDate" />
        </div>
        <button (click)="filterByDate()">Filtrar</button>
        <button (click)="loadAll()">Ver Todas</button>
        <button (click)="generateReport()">Generar Reporte</button>
      </div>

      @if (report()) {
        <div class="report-card">
          <h3>Reporte de Ventas</h3>
          <div class="report-stats">
            <div class="stat">
              <span class="label">Total Ventas:</span>
              <span class="value">{{ report()!.totalSales }}</span>
            </div>
            <div class="stat">
              <span class="label">Ingresos Totales:</span>
              <span class="value">{{ report()!.totalRevenue | currency }}</span>
            </div>
            <div class="stat">
              <span class="label">Ticket Promedio:</span>
              <span class="value">{{ report()!.averageTicket | currency }}</span>
            </div>
          </div>
        </div>
      }

      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th>Fecha</th>
              <th>Estado</th>
              <th>Subtotal</th>
              <th>Impuestos</th>
              <th>Total</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            @for (sale of sales(); track sale.id) {
              <tr [attr.data-sale-id]="sale.id">
                <td>{{ sale.soldAt | date:'short' }}</td>
                <td>
                  <span class="status" [class]="'status-' + sale.status.toLowerCase()">
                    {{ sale.status }}
                  </span>
                </td>
                <td>{{ sale.subtotal | currency }}</td>
                <td>{{ sale.taxTotal | currency }}</td>
                <td class="total">{{ sale.grandTotal | currency }}</td>
                <td>
                  <button (click)="viewDetails(sale)">Ver</button>
                  @if (sale.status === 'Draft') {
                    <button class="btn-danger" (click)="cancelSale(sale.id!)">Cancelar</button>
                  }
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>

      @if (loading()) {
        <div class="loading">Cargando...</div>
      }
      @if (error()) {
        <div class="alert alert-error">{{ error() }}</div>
      }
    </div>
  `,
  styles: [`
    .container { padding: 20px; max-width: 1200px; margin: 0 auto; }
    h1 { margin-bottom: 20px; }
    .filters { display: flex; gap: 10px; margin-bottom: 20px; align-items: flex-end; }
    .filter-group { display: flex; flex-direction: column; }
    .filter-group label { font-size: 12px; margin-bottom: 5px; color: #666; }
    .filter-group input { padding: 8px; border: 1px solid #ddd; border-radius: 4px; }
    button { padding: 8px 16px; border: none; border-radius: 4px; background: #2196F3; color: white; cursor: pointer; }
    button:hover { background: #1976D2; }
    .btn-danger { background: #f44336; }
    .btn-danger:hover { background: #d32f2f; }
    .report-card { background: #f5f5f5; padding: 20px; border-radius: 8px; margin-bottom: 20px; }
    .report-stats { display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px; }
    .stat { text-align: center; }
    .stat .label { display: block; font-size: 12px; color: #666; margin-bottom: 5px; }
    .stat .value { display: block; font-size: 24px; font-weight: bold; color: #2196F3; }
    .table-container { overflow-x: auto; }
    table { width: 100%; border-collapse: collapse; background: white; }
    th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }
    th { background: #f5f5f5; font-weight: 600; }
    .total { font-weight: bold; color: #4CAF50; }
    .status { padding: 4px 8px; border-radius: 4px; font-size: 12px; }
    .status-completed { background: #c8e6c9; color: #2e7d32; }
    .status-draft { background: #fff3e0; color: #e65100; }
    .status-cancelled { background: #ffcdd2; color: #c62828; }
    .loading { text-align: center; padding: 40px; }
    .alert { padding: 12px; border-radius: 4px; margin-top: 15px; }
    .alert-error { background: #ffebee; color: #c62828; border: 1px solid #ef5350; }
  `]
})
export class SalesListComponent implements OnInit {
  sales = signal<SaleDto[]>([]);
  report = signal<SalesReportDto | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  fromDate: string = '';
  toDate: string = '';

  constructor(private saleService: SaleService) {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    this.fromDate = firstDay.toISOString().split('T')[0];
    this.toDate = today.toISOString().split('T')[0];
  }

  ngOnInit() {
    this.loadAll();
  }

  loadAll() {
    this.loading.set(true);
    this.saleService.getAll().subscribe({
      next: (sales) => {
        this.sales.set(sales);
        this.loading.set(false);
        this.error.set(null);
      },
      error: (err) => {
        this.error.set('Error al cargar ventas');
        this.loading.set(false);
      }
    });
  }

  filterByDate() {
    if (!this.fromDate || !this.toDate) {
      this.error.set('Debe seleccionar ambas fechas');
      return;
    }

    this.loading.set(true);
    this.saleService.getByDateRange(new Date(this.fromDate), new Date(this.toDate)).subscribe({
      next: (sales) => {
        this.sales.set(sales);
        this.loading.set(false);
        this.error.set(null);
      },
      error: (err) => {
        this.error.set('Error al filtrar ventas');
        this.loading.set(false);
      }
    });
  }

  generateReport() {
    if (!this.fromDate || !this.toDate) {
      this.error.set('Debe seleccionar ambas fechas para generar el reporte');
      return;
    }

    this.saleService.getSalesReport(new Date(this.fromDate), new Date(this.toDate)).subscribe({
      next: (report) => {
        this.report.set(report);
        this.error.set(null);
      },
      error: (err) => {
        this.error.set('Error al generar reporte');
      }
    });
  }

  viewDetails(sale: SaleDto) {
    // TODO: Navigate to sale details or show modal
    console.log('View details:', sale);
  }

  cancelSale(id: number) {
    if (!confirm('¿Está seguro de cancelar esta venta?')) return;

    this.saleService.cancelSale(id).subscribe({
      next: () => {
        this.loadAll();
      },
      error: (err) => {
        this.error.set('Error al cancelar venta');
      }
    });
  }
}
