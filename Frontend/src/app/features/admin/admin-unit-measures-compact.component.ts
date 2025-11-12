import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { UnitMeasureService } from '../../core/services/unit-measure.service';
import { ButtonComponent } from '../../shared/components/button.component';
import { InputComponent } from '../../shared/components/input.component';
import { EstancoCardComponent } from '../../shared/components/estanco-card.component';
// AppHeaderComponent removed - not present in shared/components
import { UnitMeasureDto } from '../../core/models/unit-measure.model';

@Component({
  selector: 'app-admin-unit-measures',
  standalone: true,
  imports: [CommonModule, ButtonComponent, InputComponent, EstancoCardComponent],
  template: `
    <div class="page-container">
      <div class="page-content">
        <div class="section-title">
          <h1>Gestión de Unidades de Medida</h1>
          <app-button variant="primary" (clicked)="toggleForm()">{{ showForm() ? 'Cancelar' : '+ Nueva Unidad' }}</app-button>
        </div>

        @if (showForm()) {
          <app-estanco-card variant="accent">
            <h3>{{ editingId() ? 'Editar Unidad' : 'Crear Unidad' }}</h3>
            <form (submit)="saveUnitMeasure($event)" class="compact-form">
              <app-input id="name" label="Nombre/Abreviación" placeholder="Ej: kg, L, und" [value]="form().name || ''" (valueChange)="updateForm('name', $event)" [required]="true" />
              <app-input id="description" label="Descripción" placeholder="Ej: Kilogramo, Litro, Unidad" [value]="form().description || ''" (valueChange)="updateForm('description', $event)" />

              <div class="form-actions">
                <app-button type="button" variant="ghost" (clicked)="cancelForm()">Cancelar</app-button>
                <app-button type="submit" variant="primary" [loading]="loading()">{{ editingId() ? 'Actualizar' : 'Crear' }}</app-button>
              </div>
            </form>
          </app-estanco-card>
        }

        @if (error()) { <div class="alert error">{{ error() }}</div> }
        @if (success()) { <div class="alert success">{{ success() }}</div> }

        <div class="units-grid">
          @if (unitMeasures().length === 0) {
            <p class="empty-state">No hay unidades de medida. Crea una para empezar.</p>
          } @else {
            @for (unit of unitMeasures(); track unit.id) {
              <app-estanco-card class="unit-item" [variant]="unit.active ? 'primary' : 'neutral'">
                <div class="unit-header">
                  <h4>{{ unit.name }}</h4>
                  <span [class.badge]="true" [class.active]="unit.active">{{ unit.active ? 'Activo' : 'Inactivo' }}</span>
                </div>
                <p class="unit-desc">{{ unit.description }}</p>
                <div class="unit-actions">
                  <app-button variant="ghost" size="sm" (clicked)="editUnitMeasure(unit)">Editar</app-button>
                  <app-button variant="destructive" size="sm" (clicked)="deleteUnitMeasure(unit.id!)">Eliminar</app-button>
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
    .units-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 16px; }
    .unit-item { cursor: pointer; }
    .unit-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
    .unit-header h4 { margin: 0; color: var(--color-primary); font-size: 1.2rem; font-weight: 700; }
    .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 0.8rem; background: #fee2e2; color: #991b1b; }
    .badge.active { background: #dcfce7; color: #166534; }
    .unit-desc { margin: 8px 0 12px 0; font-size: 0.9rem; color: var(--color-text-secondary); }
    .unit-actions { display: flex; gap: 8px; }
    .alert { padding: 12px 16px; border-radius: 8px; margin-bottom: 16px; border-left: 4px solid; }
    .alert.error { background: #fef2f2; border-color: #dc2626; color: #7f1d1d; }
    .alert.success { background: #f0fdf4; border-color: #16a34a; color: #166534; }
    .empty-state { text-align: center; padding: 40px 20px; color: var(--color-text-secondary); }
  `]
})
export class AdminUnitMeasuresCompactComponent implements OnInit {
  private authService = inject(AuthService);
  private unitMeasureService = inject(UnitMeasureService);
  private router = inject(Router);

  unitMeasures = signal<UnitMeasureDto[]>([]);
  showForm = signal(false);
  editingId = signal(0);
  loading = signal(false);
  error = signal('');
  success = signal('');
  form = signal<Partial<UnitMeasureDto>>({ name: '', description: '', active: true });

  ngOnInit() {
    this.loadUnitMeasures();
  }

  private loadUnitMeasures() {
    this.unitMeasureService.getAll().subscribe({
      next: (data) => this.unitMeasures.set(data),
      error: (e) => this.error.set('Error al cargar unidades de medida')
    });
  }

  updateForm(key: string, value: any) {
    this.form.update(f => ({ ...f, [key]: value }));
  }

  toggleForm() { this.showForm.update(v => !v); this.editingId.set(0); }
  cancelForm() { this.showForm.set(false); this.editingId.set(0); this.form.set({ name: '', description: '', active: true }); }

  saveUnitMeasure(e: Event) {
    e.preventDefault();
    const f = this.form();
    if (!f.name) {
      this.error.set('Todos los campos requeridos');
      return;
    }
    this.loading.set(true);
    const action = this.editingId() ? this.unitMeasureService.update(f as UnitMeasureDto) : this.unitMeasureService.create(f as UnitMeasureDto);
    action.subscribe({
      next: () => {
        this.success.set('Unidad ' + (this.editingId() ? 'actualizada' : 'creada') + ' ✓');
        this.cancelForm();
        this.loadUnitMeasures();
        setTimeout(() => { this.success.set(''); this.error.set(''); }, 3000);
        this.loading.set(false);
      },
      error: (e) => {
        this.error.set(e.error?.message || 'Error en la operación');
        this.loading.set(false);
      }
    });
  }

  editUnitMeasure(unit: UnitMeasureDto) {
    this.form.set(unit);
    this.editingId.set(unit.id!);
    this.showForm.set(true);
  }

  deleteUnitMeasure(id: number) {
    if (confirm('¿Eliminar unidad de medida?')) {
      this.unitMeasureService.delete(id).subscribe({
        next: () => { this.success.set('Unidad eliminada ✓'); this.loadUnitMeasures(); },
        error: (e) => this.error.set(e.error?.message || 'Error al eliminar')
      });
    }
  }

  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}
