import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../../../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../../../shared/config/menu-sections';
import { AdminUsersCompactComponent } from '../components/admin-users-compact.component';

@Component({
	selector: 'app-admin-users-page',
	standalone: true,
	imports: [CommonModule, SidebarMenuComponent, AdminUsersCompactComponent],
	template: `
		<div class="admin-shell">
			<app-sidebar-menu [sections]="menuSections"></app-sidebar-menu>
			<div class="admin-content">
				<app-admin-users-compact></app-admin-users-compact>
			</div>
		</div>
	`,
	styles: [``]
})
export class AdminUsersComponent {
	menuSections = ADMIN_MENU_SECTIONS;
}
