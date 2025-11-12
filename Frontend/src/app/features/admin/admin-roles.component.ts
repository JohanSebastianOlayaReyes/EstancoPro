import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { RolService } from '../../core/services/rol.service';
import { ButtonComponent } from '../../shared/components/button.component';
import { InputComponent } from '../../shared/components/input.component';
import { IconComponent } from '../../shared/components/icon.component';
import { EstancoCardComponent } from '../../shared/components/estanco-card.component';
import { SidebarMenuComponent } from '../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../shared/config/menu-sections';
import { RolDto } from '../../core/models/rol.model';

@Component({
  selector: 'app-admin-roles',
  imports: [CommonModule, ButtonComponent, InputComponent, IconComponent, EstancoCardComponent, SidebarMenuComponent],
  template: `
    <div class="admin-container">
      <app-sidebar-menu [sections]="menuSections"></app-sidebar-menu>

      <div class="admin-content">
        <app-estanco-card>
          <div class="section-header">
            <h2>
              <app-icon name="clipboard-list" [size]="24" />
              Roles del Sistema
            </h2>
            <app-button
              variant="primary"
              size="sm"
              (clicked)="showCreateRolForm()"
            >
              <app-icon name="plus" [size]="18" />
              Crear Rol
            </app-button>
          </div>

          @if (showRolForm()) {
            <div class="form-card">
              <h3>
                @if (editingRol()) {
                  <app-icon name="pencil" [size]="20" />
                  Editar Rol
                } @else {
                  <app-icon name="sparkles" [size]="20" />
                  Nuevo Rol
                }
              </h3>
              <form (submit)="saveRol($event)">
                <app-input
                  id="typeRol"
                  label="Nombre del Rol"
                  placeholder="Ej: Empleado, Supervisor, Cajero"
                  [value]="rolForm().typeRol"
                  (valueChange)="updateRolField('typeRol', $event)"
                  [required]="true"
                />

                <app-input
                  id="description"
                  label="Descripción"
                  placeholder="Descripción detallada del rol y sus funciones"
                  [value]="rolForm().description"
                  (valueChange)="updateRolField('description', $event)"
                  [required]="true"
                />

                <div class="form-group">
                  <label class="checkbox-label">
                    <input
                      type="checkbox"
                      [checked]="rolForm().active"
                      (change)="toggleActive($event)"
                    />
                    <span>
                      <app-icon name="check" [size]="16" />
                      Rol activo
                    </span>
                  </label>
                </div>

                <div class="form-actions">
                  <app-button
                    type="button"
                    variant="ghost"
                    (clicked)="cancelRolForm()"
                  >
                    <app-icon name="x" [size]="18" />
                    Cancelar
                  </app-button>
                  <app-button
                    type="submit"
                    variant="primary"
                    [loading]="loadingRol()"
                  >
                    @if (editingRol()) {
                      <app-icon name="check" [size]="18" />
                      Actualizar
                    } @else {
                      <app-icon name="plus" [size]="18" />
                      Crear Rol
                    }
                  </app-button>
                </div>
              </form>
            </div>
          }

          @if (errorMessage()) {
            <div class="error-message">
              <app-icon name="alert-circle" [size]="18" />
              {{ errorMessage() }}
            </div>
          }

          @if (successMessage()) {
            <div class="success-message">
              <app-icon name="check" [size]="18" />
              {{ successMessage() }}
            </div>
          }

          <div class="roles-list">
            @if (loadingRoles()) {
              <div class="loading-state">
                <div class="spinner"></div>
                <p>Cargando roles...</p>
              </div>
            } @else if (roles().length === 0) {
              <div class="empty-state">
                <div class="empty-icon">
                  <app-icon name="clipboard-list" [size]="64" [strokeWidth]="1.5" />
                </div>
                <h3>No hay roles registrados</h3>
                <p>Crea el primer rol del sistema</p>
              </div>
            } @else {
              @for (rol of roles(); track rol.id) {
                <div class="role-card" [class.inactive]="!rol.active">
                  <div class="role-icon">
                    <app-icon [name]="getRoleIconName(rol.typeRol)" [size]="32" [strokeWidth]="1.5" />
                  </div>
                  <div class="role-info">
                    <div class="role-header">
                      <h3>{{ rol.typeRol }}</h3>
                      <span class="status" [class.active]="rol.active">
                        <app-icon [name]="rol.active ? 'check' : 'x'" [size]="14" />
                        {{ rol.active ? 'Activo' : 'Inactivo' }}
                      </span>
                    </div>
                    <p class="role-description">{{ rol.description }}</p>
                    @if (rol.id) {
                      <span class="role-id">ID: {{ rol.id }}</span>
                    }
                  </div>
                  <div class="role-actions">
                    <app-button
                      variant="ghost"
                      size="sm"
                      (clicked)="editRol(rol)"
                    >
                      <app-icon name="pencil" [size]="18" />
                      Editar
                    </app-button>
                    @if (rol.typeRol !== 'Administrador') {
                      <app-button
                        variant="destructive"
                        size="sm"
                        (clicked)="deleteRol(rol.id!)"
                      >
                        <app-icon name="trash" [size]="18" />
                        Eliminar
                      </app-button>
                    }
                  </div>
                </div>
              }
            }
          </div>
        </app-estanco-card>

        <!-- Información sobre permisos -->
        <app-estanco-card class="info-section">
          <h3>
            <app-icon name="alert-circle" [size]="20" />
            Sobre Roles y Permisos
          </h3>
          <div class="info-cards">
            <div class="info-card">
              <div class="info-icon">
                <app-icon name="settings" [size]="40" [strokeWidth]="1.5" />
              </div>
              <h4>Administrador</h4>
              <p>Acceso total al sistema, gestión de usuarios y configuración</p>
            </div>
            <div class="info-card">
              <div class="info-icon">
                <app-icon name="user" [size]="40" [strokeWidth]="1.5" />
              </div>
              <h4>Empleado</h4>
              <p>Acceso a operaciones diarias, ventas y consulta de inventario</p>
            </div>
            <div class="info-card">
              <div class="info-icon">
                <app-icon name="coin" [size]="40" [strokeWidth]="1.5" />
              </div>
              <h4>Cajero</h4>
              <p>Gestión de caja, ventas y movimientos de efectivo</p>
            </div>
            <div class="info-card">
              <div class="info-icon">
                <app-icon name="package" [size]="40" [strokeWidth]="1.5" />
              </div>
              <h4>Almacenista</h4>
              <p>Control de inventario, compras y gestión de productos</p>
            </div>
          </div>
        </app-estanco-card>
      </div>
    </div>
  `,
  styles: [`
    .admin-container {
      min-height: 100vh;
      background-color: var(--color-background);
      margin-left: 250px;
    }

    .admin-header {
      background-color: var(--color-surface);
      border-bottom: 1px solid var(--color-border);
      padding: var(--space-4) var(--space-6);
      box-shadow: var(--shadow-sm);
      position: sticky;
      top: 0;
      z-index: 100;
    }

    .header-content {
      max-width: 1400px;
      margin: 0 auto;
      display: flex;
      justify-content: space-between;
      align-items: center;

      h1 {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        font-size: var(--font-size-2xl);
        color: var(--color-primary);
        margin: 0;
      }

      .header-actions {
        display: flex;
        gap: var(--space-3);
      }
    }

    .admin-content {
      max-width: 1400px;
      margin: 0 auto;
      padding: var(--space-6);
      display: flex;
      flex-direction: column;
      gap: var(--space-6);
    }

    .admin-section {
      background-color: var(--color-surface);
      border-radius: var(--radius-lg);
      padding: var(--space-6);
      box-shadow: var(--shadow-md);
    }

    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: var(--space-6);

      h2 {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        color: var(--color-primary);
        margin: 0;
        font-size: var(--font-size-xl);
      }
    }

    .form-card {
      background-color: var(--color-surface-secondary);
      border-radius: var(--radius-md);
      padding: var(--space-6);
      margin-bottom: var(--space-6);
      border: 2px solid var(--color-border);

      h3 {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        color: var(--color-text-primary);
        margin-bottom: var(--space-4);
        font-size: var(--font-size-lg);
      }

      form {
        display: flex;
        flex-direction: column;
        gap: var(--space-4);
      }
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
    }

    .checkbox-label {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      cursor: pointer;
      font-size: var(--font-size-base);
      color: var(--color-text-primary);

      input[type="checkbox"] {
        width: 20px;
        height: 20px;
        cursor: pointer;
      }

      span {
        display: flex;
        align-items: center;
        gap: var(--space-1);
        user-select: none;
      }
    }

    .form-actions {
      display: flex;
      gap: var(--space-3);
      justify-content: flex-end;
      margin-top: var(--space-2);
    }

    .roles-list {
      display: grid;
      gap: var(--space-4);
    }

    .loading-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: var(--space-8);
      gap: var(--space-3);

      .spinner {
        width: 40px;
        height: 40px;
        border: 4px solid var(--color-border);
        border-top-color: var(--color-primary);
        border-radius: 50%;
        animation: spin 1s linear infinite;
      }

      p {
        color: var(--color-text-secondary);
      }
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .empty-state {
      text-align: center;
      padding: var(--space-8);

      .empty-icon {
        display: flex;
        justify-content: center;
        align-items: center;
        margin-bottom: var(--space-4);
        color: var(--color-text-muted);
      }

      h3 {
        color: var(--color-text-primary);
        margin: 0 0 var(--space-2) 0;
      }

      p {
        color: var(--color-text-secondary);
        margin: 0;
      }
    }

    .role-card {
      background-color: var(--color-surface-secondary);
      border-radius: var(--radius-md);
      padding: var(--space-5);
      display: flex;
      gap: var(--space-4);
      align-items: center;
      transition: all 0.3s ease;
      border: 2px solid var(--color-border);

      &:hover {
        transform: translateY(-2px);
        box-shadow: var(--shadow-md);
        border-color: var(--color-primary);
      }

      &.inactive {
        opacity: 0.6;
        background-color: var(--color-background);
      }
    }

    .role-icon {
      width: 70px;
      height: 70px;
      display: flex;
      align-items: center;
      justify-content: center;
      background-color: var(--color-primary-light);
      border-radius: var(--radius-md);
      flex-shrink: 0;
      color: var(--color-primary);
    }

    .role-info {
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
    }

    .role-header {
      display: flex;
      align-items: center;
      gap: var(--space-3);

      h3 {
        color: var(--color-text-primary);
        margin: 0;
        font-size: var(--font-size-lg);
        font-weight: var(--font-weight-semibold);
      }
    }

    .role-description {
      color: var(--color-text-secondary);
      margin: 0;
      font-size: var(--font-size-sm);
      line-height: 1.5;
    }

    .role-id {
      font-size: var(--font-size-xs);
      color: var(--color-text-muted);
      font-family: var(--font-mono);
    }

    .status {
      display: inline-flex;
      align-items: center;
      gap: var(--space-1);
      padding: var(--space-1) var(--space-3);
      border-radius: var(--radius-sm);
      font-size: var(--font-size-xs);
      font-weight: var(--font-weight-semibold);
      background-color: var(--color-error-light);
      color: var(--color-error);

      &.active {
        background-color: var(--color-success-light);
        color: var(--color-success);
      }
    }

    .role-actions {
      display: flex;
      gap: var(--space-2);
      flex-shrink: 0;
    }

    .info-section {
      background-color: var(--color-surface);
      border-radius: var(--radius-lg);
      padding: var(--space-6);
      box-shadow: var(--shadow-md);

      h3 {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        color: var(--color-primary);
        margin: 0 0 var(--space-4) 0;
        font-size: var(--font-size-lg);
      }
    }

    .info-cards {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
      gap: var(--space-4);
    }

    .info-card {
      background-color: var(--color-surface-secondary);
      border-radius: var(--radius-md);
      padding: var(--space-4);
      text-align: center;
      border: 2px solid var(--color-border);

      .info-icon {
        display: flex;
        justify-content: center;
        align-items: center;
        margin-bottom: var(--space-2);
        color: var(--color-primary);
      }

      h4 {
        color: var(--color-text-primary);
        margin: 0 0 var(--space-2) 0;
        font-size: var(--font-size-base);
        font-weight: var(--font-weight-semibold);
      }

      p {
        color: var(--color-text-secondary);
        margin: 0;
        font-size: var(--font-size-sm);
        line-height: 1.4;
      }
    }

    .error-message {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      background-color: var(--color-error-light);
      color: var(--color-error);
      padding: var(--space-3) var(--space-4);
      border-radius: var(--radius-md);
      font-size: var(--font-size-sm);
      border-left: 4px solid var(--color-error);
      margin-bottom: var(--space-4);
    }

    .success-message {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      background-color: var(--color-success-light);
      color: var(--color-success);
      padding: var(--space-3) var(--space-4);
      border-radius: var(--radius-md);
      font-size: var(--font-size-sm);
      border-left: 4px solid var(--color-success);
      margin-bottom: var(--space-4);
    }

    @media (max-width: 768px) {
      .admin-content {
        padding: var(--space-4);
      }

      .header-content {
        flex-direction: column;
        align-items: flex-start;
        gap: var(--space-3);
      }

      .section-header {
        flex-direction: column;
        align-items: flex-start;
        gap: var(--space-3);
      }

      .role-card {
        flex-direction: column;
        align-items: flex-start;
        gap: var(--space-3);
      }

      .role-header {
        flex-direction: column;
        align-items: flex-start;
      }

      .role-actions {
        width: 100%;
        justify-content: flex-end;
      }

      .info-cards {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class AdminRolesComponent implements OnInit {
  private authService = inject(AuthService);
  private rolService = inject(RolService);
  private router = inject(Router);

  menuSections = ADMIN_MENU_SECTIONS;

  roles = signal<RolDto[]>([]);
  loadingRoles = signal(false);
  loadingRol = signal(false);
  showRolForm = signal(false);
  editingRol = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  rolForm = signal<RolDto>({
    typeRol: '',
    description: '',
    active: true
  });

  ngOnInit() {
    this.loadRoles();
  }

  getRoleIconName(roleName: string): string {
    const icons: Record<string, string> = {
      'administrador': 'settings',
      'empleado': 'user',
      'cajero': 'coin',
      'almacenista': 'package',
      'supervisor': 'users',
      'vendedor': 'shopping-cart'
    };
    return icons[roleName.toLowerCase()] || 'lock';
  }

  loadRoles() {
    this.loadingRoles.set(true);
    this.rolService.getAll().subscribe({
      next: (roles) => {
        this.roles.set(roles);
        this.loadingRoles.set(false);
      },
      error: (error) => {
        console.error('Error loading roles:', error);
        this.errorMessage.set('Error al cargar los roles');
        this.loadingRoles.set(false);
      }
    });
  }

  showCreateRolForm() {
    this.rolForm.set({
      typeRol: '',
      description: '',
      active: true
    });
    this.editingRol.set(false);
    this.showRolForm.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  editRol(rol: RolDto) {
    this.rolForm.set({ ...rol });
    this.editingRol.set(true);
    this.showRolForm.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  cancelRolForm() {
    this.showRolForm.set(false);
    this.editingRol.set(false);
    this.rolForm.set({
      typeRol: '',
      description: '',
      active: true
    });
  }

  updateRolField(field: keyof RolDto, value: string) {
    this.rolForm.update(form => ({ ...form, [field]: value }));
  }

  toggleActive(event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    this.rolForm.update(form => ({ ...form, active: checked }));
  }

  saveRol(event: Event) {
    event.preventDefault();
    this.loadingRol.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const rol = this.rolForm();

    if (!rol.typeRol || !rol.description) {
      this.errorMessage.set('Todos los campos son requeridos');
      this.loadingRol.set(false);
      return;
    }

    const operation = this.editingRol()
      ? this.rolService.update(rol)
      : this.rolService.create(rol);

    operation.subscribe({
      next: () => {
        this.successMessage.set(`Rol ${this.editingRol() ? 'actualizado' : 'creado'} exitosamente`);
        this.loadingRol.set(false);
        this.cancelRolForm();
        this.loadRoles();
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Error saving role:', error);
        this.errorMessage.set(error.error?.message || 'Error al guardar el rol');
        this.loadingRol.set(false);
      }
    });
  }

  deleteRol(id: number) {
    if (!confirm('¿Estás seguro de eliminar este rol? Esta acción no se puede deshacer.')) {
      return;
    }

    this.rolService.delete(id).subscribe({
      next: () => {
        this.successMessage.set('Rol eliminado exitosamente');
        this.loadRoles();
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Error deleting role:', error);
        this.errorMessage.set(error.error?.message || 'Error al eliminar el rol. Puede que esté en uso por usuarios.');
      }
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
