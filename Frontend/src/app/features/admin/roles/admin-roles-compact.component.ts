import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';
import { RolService } from '../../../core/services/rol.service';
import { RolDto } from '../../../core/models/rol.model';

import { ADMIN_MENU_SECTIONS } from '../../../shared/config/menu-sections';
import { ButtonComponent } from '../../../shared/components/button.component';
import { EstancoCardComponent } from '../../../shared/components/estanco-card.component';
import { InputComponent } from '../../../shared/components/input.component';
import { SidebarMenuComponent } from '../../../shared/components/sidebar-menu.component';

@Component({
  selector: 'app-admin-roles',
  standalone: true,
  imports: [CommonModule, ButtonComponent, InputComponent, EstancoCardComponent, SidebarMenuComponent],
  templateUrl: './admin-roles-compact.component.html',
  styleUrls: ['./admin-roles-compact.component.scss']
})
export class AdminRolesCompactComponent implements OnInit {
  // injected services
  private readonly authService = inject(AuthService);
  private readonly rolService = inject(RolService);
  private readonly router = inject(Router);

  // --- Signals / state ---
  readonly roles = signal<RolDto[]>([]);

  // search and filter
  readonly searchTerm = signal('');
  readonly filterActive = signal<'all' | 'active' | 'inactive'>('all');

  readonly filteredRoles = computed(() => {
    const q = this.searchTerm().trim().toLowerCase();
    const f = this.filterActive();
    return this.roles().filter(r => {
      const matchName = !q || (r.typeRol || '').toLowerCase().includes(q) || (r.description || '').toLowerCase().includes(q);
      const matchActive = f === 'all' ? true : f === 'active' ? !!r.active : !r.active;
      return matchName && matchActive;
    });
  });

  readonly showForm = signal(false);
  readonly editingId = signal<number | null>(null);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly success = signal('');

  // form model
  readonly form = signal<RolDto>({ typeRol: '', description: '', active: true });

  // expose menu sections to sidebar
  readonly menuSections = ADMIN_MENU_SECTIONS;

  // Lifecycle
  ngOnInit(): void {
    this.loadRoles();
  }

  // --- Data loading / API ---
  // Make public so templates or external callers can trigger a refresh if needed
  public loadRoles(): void {
    this.loading.set(true);
    this.rolService.getAll().subscribe({
      next: (data) => { this.roles.set(data ?? []); this.loading.set(false); },
      error: (e) => { this.error.set(e?.error?.message || 'Error al cargar roles'); this.loading.set(false); }
    });
  }

  // --- Form helpers ---
  public updateForm(key: string, value: any): void {
    this.form.update(f => ({ ...f, [key]: key === 'active' ? !!value : value } as RolDto));
  }

  public toggleForm(): void {
    this.showForm.update(v => !v);
    if (!this.showForm()) { this.cancelForm(); }
  }

  public cancelForm(): void {
    this.showForm.set(false);
    this.editingId.set(null);
    this.form.set({ typeRol: '', description: '', active: true });
    this.loading.set(false);
    this.error.set('');
  }

  public editRol(rol: RolDto): void {
    this.editingId.set(rol.id ?? null);
    this.form.set({ id: rol.id, typeRol: rol.typeRol, description: rol.description, active: rol.active ?? true });
    this.showForm.set(true);
  }

  // --- Actions ---
  public saveRol(e: Event): void {
    e.preventDefault();
    const payload = this.form();
    if (!payload.typeRol || payload.typeRol.trim() === '') {
      this.error.set('El nombre del rol es requerido');
      return;
    }
    this.loading.set(true);
    if (this.editingId()) {
      this.rolService.update(payload).subscribe({
        next: () => {
          this.success.set('Rol actualizado ✓');
          this.cancelForm();
          this.loadRoles();
          this.loading.set(false);
          setTimeout(() => this.success.set(''), 3000);
        },
        error: (er) => { this.error.set(er?.error?.message || 'Error al actualizar'); this.loading.set(false); }
      });
    } else {
      this.rolService.create(payload).subscribe({
        next: () => {
          this.success.set('Rol creado ✓');
          this.cancelForm();
          this.loadRoles();
          this.loading.set(false);
          setTimeout(() => this.success.set(''), 3000);
        },
        error: (er) => { this.error.set(er?.error?.message || 'Error al crear'); this.loading.set(false); }
      });
    }
  }

  public deleteRol(id: number): void {
    if (!id) return;
    if (!confirm('¿Eliminar rol?')) return;
    this.rolService.delete(id).subscribe({
      next: () => { this.success.set('Rol eliminado ✓'); this.loadRoles(); setTimeout(() => this.success.set(''), 3000); },
      error: (er) => { this.error.set(er?.error?.message || 'Error al eliminar'); }
    });
  }

  public logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
