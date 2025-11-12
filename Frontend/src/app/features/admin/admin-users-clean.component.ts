import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { RolService } from '../../core/services/rol.service';
import { SidebarMenuComponent } from '../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../shared/config/menu-sections';
import { CreateUserDto, UserListDto } from '../../core/models/user.model';
import { RolDto } from '../../core/models/rol.model';

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
export class AdminUsersComponent implements OnInit {
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private rolService = inject(RolService);
  private router = inject(Router);

  menuSections = ADMIN_MENU_SECTIONS;

  users = signal<UserListDto[]>([]);
  roles = signal<RolDto[]>([]);
  loadingUsers = signal(false);
  loadingUser = signal(false);
  showUserForm = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  userForm = signal<CreateUserDto>({
    email: '',
    password: '',
    username: '',
    rolId: 0
  });

  ngOnInit() {
    this.loadUsers();
    this.loadRoles();
  }

  loadUsers() {
    this.loadingUsers.set(true);
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users.set(users);
        this.loadingUsers.set(false);
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.errorMessage.set('Error al cargar los usuarios');
        this.loadingUsers.set(false);
      }
    });
  }

  loadRoles() {
    this.rolService.getAll().subscribe({
      next: (roles) => {
        this.roles.set(roles.filter(r => r.active));
      },
      error: (error) => {
        console.error('Error loading roles:', error);
      }
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
