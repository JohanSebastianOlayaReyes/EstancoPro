import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { SupplierService } from '../../core/services/supplier.service';
import { ButtonComponent } from '../../shared/components/button.component';
import { InputComponent } from '../../shared/components/input.component';
import { EstancoCardComponent } from '../../shared/components/estanco-card.component';
// AppHeaderComponent removed - not present in shared/components
import { SupplierDto } from '../../core/models/supplier.model';

@Component({
  selector: 'app-admin-suppliers',
  standalone: true,
  imports: [CommonModule, ButtonComponent, InputComponent, EstancoCardComponent],
  template: `
    <div class="page-container">
      <div class="page-content">
        <div class="section-title">
          <h1>Gestión de Proveedores</h1>
          <app-button variant="primary" (clicked)="toggleForm()">{{ showForm() ? 'Cancelar' : '+ Nuevo Proveedor' }}</app-button>
        </div>

        @if (showForm()) {
          <app-estanco-card variant="accent">
            <h3>{{ editingId() ? 'Editar Proveedor' : 'Crear Proveedor' }}</h3>
            <form (submit)="saveSupplier($event)" class="compact-form">
              <app-input id="name" label="Nombre" placeholder="Ej: Distribuidora ABC" [value]="form().name || ''" (valueChange)="updateForm('name', $event)" [required]="true" />
              <app-input id="phone" label="Teléfono" placeholder="+57 300 1234567" [value]="form().phone || ''" (valueChange)="updateForm('phone', $event)" [required]="true" />

              <div class="form-actions">
                <app-button type="button" variant="ghost" (clicked)="cancelForm()">Cancelar</app-button>
                <app-button type="submit" variant="primary" [loading]="loading()">{{ editingId() ? 'Actualizar' : 'Crear' }}</app-button>
              </div>
            </form>
          </app-estanco-card>
        }

        @if (error()) { <div class="alert error">{{ error() }}</div> }
        @if (success()) { <div class="alert success">{{ success() }}</div> }

        <div class="suppliers-grid">
          @if (suppliers().length === 0) {
            <p class="empty-state">No hay proveedores. Crea uno para empezar.</p>
          } @else {
            @for (supplier of suppliers(); track supplier.id) {
              <app-estanco-card class="supplier-item" [variant]="supplier.active ? 'primary' : 'neutral'">
                <div class="supplier-header">
                  <h4>{{ supplier.name }}</h4>
                  <span [class.badge]="true" [class.active]="supplier.active">{{ supplier.active ? 'Activo' : 'Inactivo' }}</span>
                </div>
                <div class="supplier-info">
                  <p><strong>Teléfono:</strong> {{ supplier.phone }}</p>
                </div>
                <div class="supplier-actions">
                  <app-button variant="ghost" size="sm" (clicked)="editSupplier(supplier)">Editar</app-button>
                  <app-button variant="destructive" size="sm" (clicked)="deleteSupplier(supplier.id!)">Eliminar</app-button>
                </div>
              </app-estanco-card>
            }
          }
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-container { min-height: 100vh; background: var(--color-background); }
    .page-content { max-width: 1200px; margin: 0 auto; padding: 24px; }
    .section-title { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    .section-title h1 { margin: 0; color: var(--color-primary); }
    .compact-form { display: grid; gap: 16px; }
    .form-group { display: flex; flex-direction: column; gap: 8px; }
    .form-group label { font-weight: 600; font-size: 0.9rem; }
    .form-select { padding: 8px 12px; border: 1px solid var(--color-border); border-radius: 8px; font-family: var(--font-ui); }
    .form-actions { display: flex; gap: 12px; justify-content: flex-end; margin-top: 12px; }
    .suppliers-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 16px; }
    .supplier-item { cursor: pointer; }
    .supplier-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
    .supplier-header h4 { margin: 0; color: var(--color-primary); }
    .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 0.8rem; background: #fee2e2; color: #991b1b; }
    .badge.active { background: #dcfce7; color: #166534; }
    .supplier-info { display: grid; gap: 6px; font-size: 0.9rem; margin: 12px 0; }
    .supplier-info p { margin: 0; color: var(--color-text-secondary); }
    .supplier-info strong { color: var(--color-primary); }
    .supplier-actions { display: flex; gap: 8px; margin-top: 12px; }
    .alert { padding: 12px 16px; border-radius: 8px; margin-bottom: 16px; border-left: 4px solid; }
    .alert.error { background: #fef2f2; border-color: #dc2626; color: #7f1d1d; }
    .alert.success { background: #f0fdf4; border-color: #16a34a; color: #166534; }
    .empty-state { text-align: center; padding: 40px 20px; color: var(--color-text-secondary); }
  `]
})
export class AdminSuppliersCompactComponent implements OnInit {
  private authService = inject(AuthService);
  private supplierService = inject(SupplierService);
  private router = inject(Router);

  suppliers = signal<SupplierDto[]>([]);
  showForm = signal(false);
  editingId = signal(0);
  loading = signal(false);
  error = signal('');
  success = signal('');
  form = signal<Partial<SupplierDto>>({ name: '', phone: '', active: true });

  ngOnInit() {
    this.loadSuppliers();
  }

  private loadSuppliers() {
    this.supplierService.getAll().subscribe({
      next: (data) => this.suppliers.set(data),
      error: (e) => this.error.set('Error al cargar proveedores')
    });
  }

  updateForm(key: string, value: any) {
    this.form.update(f => ({ ...f, [key]: value }));
  }

  toggleForm() { this.showForm.update(v => !v); this.editingId.set(0); }
  cancelForm() { this.showForm.set(false); this.editingId.set(0); this.form.set({ name: '', phone: '', active: true }); }

  saveSupplier(e: Event) {
    e.preventDefault();
    const f = this.form();
    if (!f.name || !f.phone) {
      this.error.set('Todos los campos requeridos');
      return;
    }
    this.loading.set(true);
    const action = this.editingId() ? this.supplierService.update(f as SupplierDto) : this.supplierService.create(f as SupplierDto);
    action.subscribe({
      next: () => {
        this.success.set('Proveedor ' + (this.editingId() ? 'actualizado' : 'creado') + ' ✓');
        this.cancelForm();
        this.loadSuppliers();
        setTimeout(() => { this.success.set(''); this.error.set(''); }, 3000);
        this.loading.set(false);
      },
      error: (e) => {
        this.error.set(e.error?.message || 'Error en la operación');
        this.loading.set(false);
      }
    });
  }

  editSupplier(supplier: SupplierDto) {
    this.form.set(supplier);
    this.editingId.set(supplier.id!);
    this.showForm.set(true);
  }

  deleteSupplier(id: number) {
    if (confirm('¿Eliminar proveedor?')) {
      this.supplierService.delete(id).subscribe({
        next: () => { this.success.set('Proveedor eliminado ✓'); this.loadSuppliers(); },
        error: (e) => this.error.set(e.error?.message || 'Error al eliminar')
      });
    }
  }

  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}
