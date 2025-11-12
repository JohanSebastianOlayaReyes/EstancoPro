import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { RolService } from '../../core/services/rol.service';
import { ButtonComponent } from '../../shared/components/button.component';
import { InputComponent } from '../../shared/components/input.component';
import { EstancoCardComponent } from '../../shared/components/estanco-card.component';
// AppHeaderComponent removed - not present in shared/components
import { CreateUserDto, UserListDto } from '../../core/models/user.model';
import { RolDto } from '../../core/models/rol.model';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, ButtonComponent, InputComponent, EstancoCardComponent],
  template: `
    <div class="page-container">
      <div class="page-content">
        <div class="section-title">
          <h1>Gestión de Usuarios</h1>
          <app-button variant="primary" (clicked)="toggleForm()">{{ showForm() ? 'Cancelar' : '+ Nuevo Usuario' }}</app-button>
        </div>

        @if (showForm()) {
          <app-estanco-card variant="accent">
            <h3>Crear Usuario</h3>
            <form (submit)="saveUser($event)" class="compact-form">
              <app-input id="username" label="Nombre" placeholder="Juan Pérez" [value]="form().username" (valueChange)="updateForm('username', $event)" [required]="true" />
              <app-input id="email" type="email" label="Email" placeholder="juan@test.com" [value]="form().email" (valueChange)="updateForm('email', $event)" [required]="true" />
              <app-input id="password" type="password" label="Contraseña" placeholder="••••••" [value]="form().password" (valueChange)="updateForm('password', $event)" [required]="true" />
              <div class="form-group">
                <label>Rol</label>
                <select [value]="form().rolId" (change)="onRolChange($event)" class="form-select">
                  <option value="0">Selecciona rol</option>
                  @for (r of roles(); track r.id) { 
                    <option [value]="r.id">{{ r.typeRol }}</option> 
                  }
                </select>
              </div>
              <div class="form-actions">
                <app-button type="button" variant="ghost" (clicked)="cancelForm()">Cancelar</app-button>
                <app-button type="submit" variant="primary" [loading]="loading()">Crear</app-button>
              </div>
            </form>
          </app-estanco-card>
        }

        @if (error()) { <div class="alert error">{{ error() }}</div> }
        @if (success()) { <div class="alert success">{{ success() }}</div> }

        <div class="users-grid">
          @if (users().length === 0) {
            <p class="empty-state">No hay usuarios. Crea uno para empezar.</p>
          } @else {
            @for (user of users(); track user.id) {
              <app-estanco-card class="user-item">
                <div class="user-header">
                  <h4>{{ user.username }}</h4>
                  <span [class.badge]="true" [class.active]="user.active">{{ user.active ? 'Activo' : 'Inactivo' }}</span>
                </div>
                <p class="user-email">{{ user.email }}</p>
                <p class="user-rol">Rol: {{ user.roleName }}</p>
                <app-button *ngIf="user.active" variant="destructive" size="sm" (clicked)="deleteUser(user.id)">Desactivar</app-button>
                <app-button *ngIf="!user.active" variant="primary" size="sm" (clicked)="activateUser(user.id)">Activar</app-button>
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
    .users-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 16px; }
    .user-item { cursor: pointer; }
    .user-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
    .user-header h4 { margin: 0; color: var(--color-primary); }
    .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 0.8rem; background: #fee2e2; color: #991b1b; }
    .badge.active { background: #dcfce7; color: #166534; }
    .user-email { margin: 4px 0; font-size: 0.9rem; color: var(--color-text-secondary); }
    .user-rol { margin: 4px 0 12px 0; font-size: 0.85rem; color: var(--color-text-secondary); }
    .alert { padding: 12px 16px; border-radius: 8px; margin-bottom: 16px; border-left: 4px solid; }
    .alert.error { background: #fef2f2; border-color: #dc2626; color: #7f1d1d; }
    .alert.success { background: #f0fdf4; border-color: #16a34a; color: #166534; }
    .empty-state { text-align: center; padding: 40px 20px; color: var(--color-text-secondary); }
  `]
})
export class AdminUsersCompactComponent implements OnInit {
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private rolService = inject(RolService);
  private router = inject(Router);

  users = signal<UserListDto[]>([]);
  roles = signal<RolDto[]>([]);
  showForm = signal(false);
  loading = signal(false);
  error = signal('');
  success = signal('');
  form = signal<CreateUserDto>({ email: '', password: '', username: '', rolId: 0 });

  ngOnInit() {
    this.loadUsers();
    this.loadRoles();
  }

  private loadUsers() {
    this.userService.getAll().subscribe({
      next: (data) => this.users.set(data),
      error: (e) => this.error.set('Error al cargar usuarios')
    });
  }

  private loadRoles() {
    this.rolService.getAll().subscribe({
      next: (data) => this.roles.set(data.filter(r => r.active)),
      error: () => {}
    });
  }

  updateForm(key: string, value: any) {
    this.form.update(f => ({ ...f, [key]: key === 'rolId' ? parseInt(value) : value }));
  }

  onRolChange(e: Event) {
    const value = (e.target as HTMLSelectElement).value;
    this.updateForm('rolId', value);
  }

  toggleForm() { this.showForm.update(v => !v); }
  cancelForm() { this.showForm.set(false); this.form.set({ email: '', password: '', username: '', rolId: 0 }); }

  saveUser(e: Event) {
    e.preventDefault();
    const f = this.form();
    if (!f.email || !f.password || !f.username || !f.rolId) {
      this.error.set('Todos los campos requeridos');
      return;
    }
    this.loading.set(true);
    this.userService.create(f).subscribe({
      next: () => {
        this.success.set('Usuario creado ✓');
        this.cancelForm();
        this.loadUsers();
        setTimeout(() => { this.success.set(''); this.error.set(''); }, 3000);
        this.loading.set(false);
      },
      error: (e) => {
        this.error.set(e.error?.message || 'Error al crear');
        this.loading.set(false);
      }
    });
  }

  deleteUser(id: number) {
    if (confirm('¿Desactivar usuario?')) {
      this.userService.delete(id).subscribe({
        next: () => { this.success.set('Usuario desactivado ✓'); this.loadUsers(); },
        error: (e) => this.error.set(e.error?.message || 'Error al desactivar')
      });
    }
  }

  activateUser(id: number) {
    if (confirm('¿Activar usuario?')) {
      this.userService.activate(id).subscribe({ next: () => { this.success.set('Usuario activado ✓'); this.loadUsers(); }, error: (e) => this.error.set(e.error?.message || 'Error al activar') });
    }
  }

  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}
