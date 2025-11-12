import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../shared/config/menu-sections';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, SidebarMenuComponent],
  template: `
    <div class="admin-shell">
      <app-sidebar-menu [sections]="menuSections"></app-sidebar-menu>
      <div class="admin-content">
        <h1>Gestión de Usuarios</h1>
        <p>Contenido de gestión de usuarios aquí...</p>
      </div>
    </div>
  `,
  styles: [`
    .admin-shell {
      display: flex;
      min-height: 100vh;
      background-color: #f5f5f5;
    }

    .admin-content {
      flex: 1;
      padding: 2rem;
    }

    h1 {
      color: #333;
      margin-bottom: 1rem;
    }
  `]
})
export class AdminUsersComponent {
  menuSections = ADMIN_MENU_SECTIONS;
}
