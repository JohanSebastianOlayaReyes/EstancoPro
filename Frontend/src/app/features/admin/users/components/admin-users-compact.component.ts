import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { UserService } from '../../../../core/services/user.service';
import { PersonService } from '../../../../core/services/person.service';
import { RolService } from '../../../../core/services/rol.service';
import { ButtonComponent } from '../../../../shared/components/button.component';
import { InputComponent } from '../../../../shared/components/input.component';
import { EstancoCardComponent } from '../../../../shared/components/estanco-card.component';
import { SidebarMenuComponent } from '../../../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../../../shared/config/menu-sections';
import { CreateUserDto, UserListDto, UserDto } from '../../../../core/models/user.model';
import { RolDto } from '../../../../core/models/rol.model';

@Component({
	selector: 'app-admin-users-compact',
	standalone: true,
	imports: [CommonModule, ButtonComponent, InputComponent, EstancoCardComponent, SidebarMenuComponent],
	templateUrl: './admin-users-compact.component.html',
	styleUrls: ['./admin-users-compact.component.scss']
})
export class AdminUsersCompactComponent implements OnInit {
	private authService = inject(AuthService);
	private userService = inject(UserService);
	private rolService = inject(RolService);
	private personService = inject(PersonService);
	private router = inject(Router);

	users = signal<UserListDto[]>([]);
	// Whether to include inactive (soft-deleted) users in the list
	showInactive = signal(false);
	roles = signal<RolDto[]>([]);
	showForm = signal(false);
	loading = signal(false);
	error = signal('');
	success = signal('');
	form = signal<CreateUserDto>({ email: '', password: '', username: '', rolId: 0 });
	filterName = signal('');
	filterRoleId = signal(0);
	editingUserId = signal<number | null>(null);

	filteredUsers = computed(() => {
		const name = this.filterName().trim().toLowerCase();
		const roleId = Number(this.filterRoleId());
		return this.users().filter(u => {
			const matchesName = !name || (u.username || '').toLowerCase().includes(name);
			if (!roleId) return matchesName;
			const selected = this.roles().find(r => r.id === roleId);
			const matchesRole = !!selected && u.roleName === selected.typeRol;
			return matchesName && matchesRole;
		});
	});

	ngOnInit() {
		this.loadUsers();
		this.loadRoles();
	}

	// menu sections used by the sidebar menu
	menuSections = ADMIN_MENU_SECTIONS;

	getUserRoleName(user: UserListDto) {
		// Prefer server-provided roleName, otherwise lookup by rolId if present
		if (user.roleName) return user.roleName;
		const r = this.roles().find(x => (user as any).rolId ? x.id === (user as any).rolId : false);
		return r ? r.typeRol : 'Sin rol';
	}

	openCreate() {
		this.editingUserId.set(null);
		this.editingUserDto.set(null);
		this.form.set({ email: '', password: '', username: '', rolId: 0 });
		this.showForm.set(true);
	}

	loadUsers() {
		const includeDeleted = this.showInactive();
		this.userService.getAll(includeDeleted).subscribe({ next: (data) => this.users.set(data), error: () => this.error.set('Error al cargar usuarios') });
	}

	loadRoles() {
		this.rolService.getAll().subscribe({ next: (data) => this.roles.set(data.filter(r => r.active)), error: () => {} });
	}

	updateForm(key: string, value: any) { this.form.update(f => ({ ...f, [key]: key === 'rolId' ? parseInt(value) : value })); }

	onFilterName(v: string) { this.filterName.set(v || ''); }

	onFilterRole(v: any) { this.filterRoleId.set(parseInt(v) || 0); }

	editingUserDto = signal<UserDto | null>(null);

	startEdit(user: UserListDto) {
		// fetch full user DTO to preserve personId/rolId for update
		this.userService.getById(user.id).subscribe({ next: (u) => {
			this.editingUserDto.set(u);
			this.editingUserId.set(u.id ?? null);
			// username comes from the UserListDto (user) while other fields from full UserDto
			this.form.set({ username: user.username || '', email: u.email || '', password: '', rolId: u.rolId || 0 });
			this.showForm.set(true);
		}, error: () => this.error.set('No se pudo cargar usuario') });
	}

	onRolChange(e: Event) { const value = (e.target as HTMLSelectElement).value; this.updateForm('rolId', value); }

	toggleForm() { this.showForm.update(v => !v); }
	cancelForm() { this.showForm.set(false); this.form.set({ email: '', password: '', username: '', rolId: 0 }); }

	saveUser(e: Event) {
		e.preventDefault();
		const f = this.form();
		if (!f.email || !f.username || !f.rolId) { this.error.set('Todos los campos requeridos'); return; }
		this.loading.set(true);
		const editingId = this.editingUserId();
		if (editingId) {
			const base = this.editingUserDto();
			if (!base) { this.error.set('Usuario no cargado'); this.loading.set(false); return; }
			// Preserve existing hashed password unless a new password was provided in the form.
			const payload: UserDto = { ...base, fullName: f.username, email: f.email, rolId: f.rolId };
			if (f.password) payload.password = f.password;
			this.userService.update(payload).subscribe({ next: () => { this.success.set('Usuario actualizado ✓'); this.cancelForm(); this.loadUsers(); setTimeout(() => { this.success.set(''); this.error.set(''); }, 3000); this.loading.set(false); this.editingUserId.set(null); this.editingUserDto.set(null); }, error: (e) => { this.error.set(e.error?.message || 'Error al actualizar'); this.loading.set(false); } });
		} else {
			// Creating a user requires a Person entity first on the backend.
			// Build a Person DTO: backend validates FirstName and FirstLastName, so split the full name
			const nameParts = (f.username || '').trim().split(/\s+/).filter(Boolean);
			const firstName = nameParts.length ? nameParts[0] : '';
			const firstLastName = nameParts.length > 1 ? nameParts.slice(1).join(' ') : '';
			const createPerson = { fullName: f.username, firstName, firstLastName };
			this.personService.create(createPerson).subscribe({
				next: (p) => {
					const payload: any = { fullName: f.username, email: f.email, password: f.password, rolId: f.rolId, personId: p.id };
					this.userService.create(payload).subscribe({ next: () => { this.success.set('Usuario creado ✓'); this.cancelForm(); this.loadUsers(); setTimeout(() => { this.success.set(''); this.error.set(''); }, 3000); this.loading.set(false); }, error: (e) => { this.error.set(e.error?.message || 'Error al crear usuario'); this.loading.set(false); } });
				},
				error: (e) => { this.error.set(e.error?.message || 'Error al crear persona'); this.loading.set(false); }
			});
		}
	}

	deleteUser(id: number) {
		if (confirm('¿Desactivar usuario?')) {
			this.userService.delete(id).subscribe({ next: () => { this.success.set('Usuario desactivado ✓'); this.loadUsers(); }, error: (e) => this.error.set(e.error?.message || 'Error al desactivar') });
		}
	}

	activateUser(id: number) {
		if (confirm('¿Activar usuario?')) {
			this.userService.activate(id).subscribe({ next: () => { this.success.set('Usuario activado ✓'); this.loadUsers(); }, error: (e) => this.error.set(e.error?.message || 'Error al activar') });
		}
	}

	logout() { this.authService.logout(); this.router.navigate(['/login']); }
}

