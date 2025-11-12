import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { RolService } from '../../core/services/rol.service';
import { ButtonComponent } from '../../shared/components/button.component';
import { InputComponent } from '../../shared/components/input.component';
import { EstancoCardComponent } from '../../shared/components/estanco-card.component';
// AppHeaderComponent removed - not present in shared/components
import { RolDto } from '../../core/models/rol.model';

@Component({
  selector: 'app-admin-roles',
  standalone: true,
  imports: [CommonModule, ButtonComponent, InputComponent, EstancoCardComponent],
  template: `
    <div class="page-container">
      <div class="page-content">
        <div class="section-title">
          <h1>Gestión de Roles</h1>
          <app-button variant="primary" (clicked)="toggleForm()">{{ showForm() ? 'Cancelar' : '+ Nuevo Rol' }}</app-button>
        </div>

        @if (showForm()) {
          <app-estanco-card variant="accent">
            <h3>{{ editingId() ? 'Editar Rol' : 'Crear Rol' }}</h3>
            <form (submit)="saveRol($event)" class="compact-form">
              <app-input id="typeRol" label="Nombre del Rol" placeholder="Ej: Vendedor, Admin" [value]="form().typeRol" (valueChange)="updateForm('typeRol', $event)" [required]="true" />
              <app-input id="description" label="Descripción" placeholder="Descripción del rol" [value]="form().description" (valueChange)="updateForm('description', $event)" [required]="true" />
              <div class="form-actions">
                <app-button type="button" variant="ghost" (clicked)="cancelForm()">Cancelar</app-button>
                <app-button type="submit" variant="primary" [loading]="loading()">{{ editingId() ? 'Actualizar' : 'Crear' }}</app-button>
              </div>
            </form>
          </app-estanco-card>
        }

        @if (error()) { <div class="alert error">{{ error() }}</div> }
        @if (success()) { <div class="alert success">{{ success() }}</div> }

        <div class="roles-grid">
          @if (roles().length === 0) {
            <p class="empty-state">No hay roles. Crea uno para empezar.</p>
          } @else {
            @for (rol of roles(); track rol.id) {
              <app-estanco-card class="rol-item" [variant]="rol.active ? 'primary' : 'neutral'">
                <div class="rol-header">
                  <h4>{{ rol.typeRol }}</h4>
                  <span [class.badge]="true" [class.active]="rol.active">{{ rol.active ? 'Activo' : 'Inactivo' }}</span>
                </div>
                <p class="rol-desc">{{ rol.description }}</p>
                <div class="rol-actions">
                  <app-button variant="ghost" size="sm" (clicked)="editRol(rol)">Editar</app-button>
                  @if (rol.typeRol !== 'Administrador') {
                    <app-button variant="destructive" size="sm" (clicked)="deleteRol(rol.id!)">Eliminar</app-button>
                  }
                </div>
              </app-estanco-card>
            }
          }
        </div>

        <div class="info-row">
          <h3>Roles Estándar del Sistema</h3>
          <div class="info-grid">
            <div class="info-item">
              <strong>Administrador</strong>
              <p>Acceso completo al sistema</p>
            </div>
            <div class="info-item">
              <strong>Vendedor</strong>
              <p>Gestión de ventas y clientes</p>
            </div>
            <div class="info-item">
              <strong>Cajero</strong>
              <p>Operaciones de caja</p>
            </div>
          </div>
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
    .form-actions { display: flex; gap: 12px; justify-content: flex-end; margin-top: 12px; }
    .roles-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 16px; margin-bottom: 32px; }
    .rol-item { cursor: pointer; }
    .rol-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
    .rol-header h4 { margin: 0; color: var(--color-primary); }
    .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 0.8rem; background: #fee2e2; color: #991b1b; }
    .badge.active { background: #dcfce7; color: #166534; }
    .rol-desc { margin: 8px 0 16px 0; font-size: 0.9rem; color: var(--color-text-secondary); line-height: 1.4; }
    .rol-actions { display: flex; gap: 8px; }
    .alert { padding: 12px 16px; border-radius: 8px; margin-bottom: 16px; border-left: 4px solid; }
    .alert.error { background: #fef2f2; border-color: #dc2626; color: #7f1d1d; }
    .alert.success { background: #f0fdf4; border-color: #16a34a; color: #166534; }
    .empty-state { text-align: center; padding: 40px 20px; color: var(--color-text-secondary); }
    .info-row { margin-top: 32px; }
    .info-row h3 { color: var(--color-primary); margin-bottom: 16px; }
    .info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px; }
    .info-item { padding: 16px; background: var(--color-surface); border-radius: 8px; border-left: 4px solid var(--color-accent); }
    .info-item strong { display: block; margin-bottom: 8px; color: var(--color-primary); }
    .info-item p { margin: 0; font-size: 0.9rem; color: var(--color-text-secondary); }
  `]
})
export class AdminRolesCompactComponent implements OnInit {
  private authService = inject(AuthService);
  private rolService = inject(RolService);
  private router = inject(Router);

  roles = signal<RolDto[]>([]);
  showForm = signal(false);
  editingId = signal<number | null>(null);
  loading = signal(false);
  error = signal('');
  success = signal('');
  form = signal<RolDto>({ typeRol: '', description: '', active: true });

  ngOnInit() {
    this.loadRoles();
  }

  private loadRoles() {
    this.rolService.getAll().subscribe({
      next: (data) => this.roles.set(data),
      error: (e) => this.error.set('Error al cargar roles')
    });
  }

  updateForm(key: string, value: any) {
    this.form.update(f => ({ ...f, [key]: value }));
  }

  toggleForm() { this.showForm.update(v => !v); }

  cancelForm() {
    this.showForm.set(false);
    this.editingId.set(null);
    this.form.set({ typeRol: '', description: '', active: true });
  }

  editRol(rol: RolDto) {
    this.form.set(rol);
    this.editingId.set(rol.id || null);
    this.showForm.set(true);
  }

  saveRol(e: Event) {
    e.preventDefault();
    const f = this.form();
    if (!f.typeRol || !f.description) {
      this.error.set('Todos los campos requeridos');
      return;
    }
    this.loading.set(true);

    const save$ = this.editingId() 
      ? this.rolService.update(f) 
      : this.rolService.create(f);

    save$.subscribe({
      next: () => {
        this.success.set(this.editingId() ? 'Rol actualizado ✓' : 'Rol creado ✓');
        this.cancelForm();
        this.loadRoles();
        setTimeout(() => { this.success.set(''); this.error.set(''); }, 3000);
        this.loading.set(false);
      },
      error: (e) => {
        this.error.set(e.error?.message || 'Error al guardar');
        this.loading.set(false);
      }
    });
  }

  deleteRol(id: number) {
    if (confirm('¿Eliminar rol?')) {
      this.rolService.delete(id).subscribe({
        next: () => { this.success.set('Rol eliminado ✓'); this.loadRoles(); },
        error: (e) => this.error.set(e.error?.message || 'Error al eliminar')
      });
    }
  }

  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}
