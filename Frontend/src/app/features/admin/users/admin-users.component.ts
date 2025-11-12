import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../../shared/config/menu-sections';

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
  styles: [``]
})
export class AdminUsersComponent {
  menuSections = ADMIN_MENU_SECTIONS;
}
