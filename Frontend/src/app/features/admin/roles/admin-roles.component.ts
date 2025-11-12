import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { RolService } from '../../../core/services/rol.service';
import { ButtonComponent } from '../../../shared/components/button.component';
import { InputComponent } from '../../../shared/components/input.component';
import { IconComponent } from '../../../shared/components/icon.component';
import { EstancoCardComponent } from '../../../shared/components/estanco-card.component';
import { SidebarMenuComponent } from '../../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../../shared/config/menu-sections';
import { RolDto } from '../../../core/models/rol.model';

@Component({
  selector: 'app-admin-roles',
  imports: [CommonModule, ButtonComponent, InputComponent, IconComponent, EstancoCardComponent, SidebarMenuComponent],
  templateUrl: './admin-roles.component.html',
  styleUrls: ['./admin-roles.component.scss']
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

  rolForm = signal<RolDto>({ typeRol: '', description: '', active: true });

  ngOnInit() { this.loadRoles(); }
  loadRoles() {
    this.loadingRoles.set(true);
    this.rolService.getAll().subscribe({
      next: (data) => { this.roles.set(data ?? []); this.loadingRoles.set(false); },
      error: (e) => { this.errorMessage.set(e?.error?.message || 'Error al cargar roles'); this.loadingRoles.set(false); }
    });
  }

  showCreateRolForm() {
    this.editingRol.set(false);
    this.rolForm.set({ typeRol: '', description: '', active: true });
    this.showRolForm.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  editRol(rol: RolDto) {
    this.editingRol.set(true);
    this.rolForm.set({ id: rol.id, typeRol: rol.typeRol, description: rol.description, active: rol.active ?? true });
    this.showRolForm.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  cancelRolForm() {
    this.showRolForm.set(false);
    this.rolForm.set({ typeRol: '', description: '', active: true });
    this.loadingRol.set(false);
    this.editingRol.set(false);
    this.errorMessage.set('');
  }

  updateRolField(field: keyof RolDto, value: string) {
    this.rolForm.update(r => ({ ...r, [field]: value } as RolDto));
  }

  toggleActive(event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    this.rolForm.update(r => ({ ...r, active: checked } as RolDto));
  }

  saveRol(event: Event) {
    event.preventDefault();
    const payload = this.rolForm();
    if (!payload.typeRol || payload.typeRol.trim() === '') {
      this.errorMessage.set('El nombre del rol es requerido');
      return;
    }
    this.loadingRol.set(true);
    if (this.editingRol()) {
      this.rolService.update(payload).subscribe({
        next: () => {
          this.successMessage.set('Rol actualizado ✓');
          this.cancelRolForm();
          this.loadRoles();
          this.loadingRol.set(false);
          setTimeout(() => this.successMessage.set(''), 3000);
        },
        error: (e) => { this.errorMessage.set(e?.error?.message || 'Error al actualizar'); this.loadingRol.set(false); }
      });
    } else {
      this.rolService.create(payload).subscribe({
        next: () => {
          this.successMessage.set('Rol creado ✓');
          this.cancelRolForm();
          this.loadRoles();
          this.loadingRol.set(false);
          setTimeout(() => this.successMessage.set(''), 3000);
        },
        error: (e) => { this.errorMessage.set(e?.error?.message || 'Error al crear'); this.loadingRol.set(false); }
      });
    }
  }

  deleteRol(id: number) {
    if (!id) return;
    if (!confirm('¿Eliminar rol?')) return;
    this.rolService.delete(id).subscribe({
      next: () => { this.successMessage.set('Rol eliminado ✓'); this.loadRoles(); setTimeout(() => this.successMessage.set(''), 3000); },
      error: (e) => { this.errorMessage.set(e?.error?.message || 'Error al eliminar'); }
    });
  }
  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}
