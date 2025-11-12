import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { ButtonComponent } from '../../../shared/components/button.component';
import { InputComponent } from '../../../shared/components/input.component';
import { IconComponent } from '../../../shared/components/icon.component';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent, IconComponent],
  template: `
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <div class="logo">
            <svg width="48" height="48" viewBox="0 0 48 48" fill="none">
              <rect width="48" height="48" rx="8" fill="var(--color-primary)"/>
              <path d="M24 12L32 20L24 28L16 20L24 12Z" fill="white"/>
              <path d="M24 20L32 28L24 36L16 28L24 20Z" fill="var(--color-accent)"/>
            </svg>
          </div>
          <h1>EstancoPro</h1>
          <p>Sistema de gestión de estanco</p>
        </div>

        <form (ngSubmit)="onSubmit()" class="login-form">
          <app-input
            id="email"
            name="email"
            type="email"
            label="Correo electrónico"
            placeholder="usuario@ejemplo.com"
            [value]="credentials().email"
            (valueChange)="updateEmail($event)"
            [error]="errors().email"
            [required]="true"
          />

          <app-input
            id="password"
            name="password"
            type="password"
            label="Contraseña"
            placeholder="••••••••"
            [value]="credentials().password"
            (valueChange)="updatePassword($event)"
            [error]="errors().password"
            [required]="true"
          />

          @if (errorMessage()) {
            <div class="error-alert" role="alert">
              <app-icon name="alert-circle" [size]="20" />
              <span>{{ errorMessage() }}</span>
            </div>
          }

          <app-button
            type="submit"
            variant="primary"
            size="lg"
            [loading]="loading()"
            [disabled]="!isFormValid()"
            [fullWidth]="true"
          >
            {{ loading() ? 'Iniciando sesión...' : 'Iniciar sesión' }}
          </app-button>
        </form>

        <div class="login-footer">
          <p class="text-muted">¿Problemas para acceder? Contacta al administrador</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: var(--space-4);
      background: linear-gradient(135deg, var(--color-primary) 0%, var(--color-primary-light) 100%);
    }

    .login-card {
      width: 100%;
      max-width: 420px;
      background: var(--color-surface);
      border-radius: var(--radius-lg);
      box-shadow: var(--shadow-lg);
      padding: var(--space-8);
    }

    .login-header {
      text-align: center;
      margin-bottom: var(--space-8);

      .logo {
        display: flex;
        justify-content: center;
        margin-bottom: var(--space-4);
      }

      h1 {
        font-size: var(--font-size-2xl);
        color: var(--color-text-primary);
        margin-bottom: var(--space-2);
      }

      p {
        font-size: var(--font-size-sm);
        color: var(--color-text-secondary);
        margin: 0;
      }
    }

    .login-form {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .error-alert {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      padding: var(--space-3);
      background-color: var(--color-error-light);
      color: var(--color-error);
      border-radius: var(--radius-md);
      font-size: var(--font-size-sm);

      svg {
        flex-shrink: 0;
      }
    }

    .login-footer {
      margin-top: var(--space-6);
      text-align: center;

      .text-muted {
        font-size: var(--font-size-sm);
        color: var(--color-text-muted);
        margin: 0;

        .register-link {
          color: var(--color-primary);
          text-decoration: none;
          font-weight: var(--font-weight-semibold);
          transition: color 0.2s ease;

          &:hover {
            color: var(--color-success);
            text-decoration: underline;
          }
        }
      }
    }

    @media (max-width: 480px) {
      .login-card {
        padding: var(--space-6);
      }
    }
  `]
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  credentials = signal({
    email: '',
    password: ''
  });

  errors = signal({
    email: '',
    password: ''
  });

  loading = signal(false);
  errorMessage = signal('');

  updateEmail(value: string) {
    this.credentials.update(creds => ({ ...creds, email: value }));
    if (this.errors().email) {
      this.errors.update(errs => ({ ...errs, email: '' }));
    }
  }

  updatePassword(value: string) {
    this.credentials.update(creds => ({ ...creds, password: value }));
    if (this.errors().password) {
      this.errors.update(errs => ({ ...errs, password: '' }));
    }
  }

  isFormValid(): boolean {
    const { email, password } = this.credentials();
    return email.length > 0 && password.length > 0;
  }

  validateForm(): boolean {
    const { email, password } = this.credentials();
    const newErrors = { email: '', password: '' };
    let isValid = true;

    if (!email) {
      newErrors.email = 'El correo electrónico es requerido';
      isValid = false;
    } else if (!email.includes('@')) {
      newErrors.email = 'Ingrese un correo electrónico válido';
      isValid = false;
    }

    if (!password) {
      newErrors.password = 'La contraseña es requerida';
      isValid = false;
    } else if (password.length < 6) {
      newErrors.password = 'La contraseña debe tener al menos 6 caracteres';
      isValid = false;
    }

    this.errors.set(newErrors);
    return isValid;
  }

  onSubmit() {
    this.errorMessage.set('');

    if (!this.validateForm()) {
      return;
    }

    this.loading.set(true);

    // El backend espera "Email" y "Password" según LoginDto
    const loginData = {
      Email: this.credentials().email,
      Password: this.credentials().password
    };

    this.authService.login(loginData as any).subscribe({
      next: (response) => {
        this.loading.set(false);
        console.log('Login exitoso:', response);
        // Redirigir al dashboard
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.loading.set(false);
        console.error('Error de login:', error);

        if (error.status === 401) {
          this.errorMessage.set('Credenciales incorrectas. Por favor, verifica tu correo y contraseña.');
        } else if (error.status === 0) {
          this.errorMessage.set('No se pudo conectar con el servidor. Verifica tu conexión.');
        } else {
          this.errorMessage.set('Ocurrió un error al iniciar sesión. Intenta nuevamente.');
        }
      }
    });
  }
}
