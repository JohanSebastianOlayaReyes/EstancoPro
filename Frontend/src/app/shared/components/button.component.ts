import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

type ButtonVariant = 'primary' | 'secondary' | 'ghost' | 'destructive';
type ButtonSize = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      [type]="type()"
      [disabled]="disabled() || loading()"
      [class]="buttonClasses()"
      (click)="handleClick($event)"
    >
      @if (loading()) {
        <span class="spinner" aria-hidden="true"></span>
      }
      <ng-content></ng-content>
    </button>
  `,
  styles: [`
    button {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: var(--space-2);
      font-family: var(--font-ui);
      font-weight: var(--font-weight-medium);
      border-radius: var(--radius-md);
      border: 1px solid transparent;
      transition: all var(--transition-fast);
      cursor: pointer;
      white-space: nowrap;

      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }

      &:focus-visible {
        outline: 2px solid var(--color-primary);
        outline-offset: 2px;
      }
    }

    /* Sizes */
    .btn-sm {
      padding: var(--space-2) var(--space-3);
      font-size: var(--font-size-sm);
      min-height: 32px;
    }

    .btn-md {
      padding: var(--space-2) var(--space-4);
      font-size: var(--font-size-base);
      min-height: 40px;
    }

    .btn-lg {
      padding: var(--space-3) var(--space-6);
      font-size: var(--font-size-lg);
      min-height: 48px;
    }

    /* Variants */
    .btn-primary {
      background-color: var(--color-primary);
      color: var(--color-text-inverse);

      &:hover:not(:disabled) {
        background-color: var(--color-primary-hover);
      }
    }

    .btn-secondary {
      background-color: var(--color-surface);
      color: var(--color-text-primary);
      border-color: var(--color-border);

      &:hover:not(:disabled) {
        background-color: var(--color-surface-secondary);
      }
    }

    .btn-ghost {
      background-color: transparent;
      color: var(--color-text-primary);

      &:hover:not(:disabled) {
        background-color: var(--color-surface-secondary);
      }
    }

    .btn-destructive {
      background-color: var(--color-error);
      color: var(--color-text-inverse);

      &:hover:not(:disabled) {
        background-color: var(--color-error-hover);
      }
    }

    .btn-full-width {
      width: 100%;
    }

    .spinner {
      width: 16px;
      height: 16px;
      border: 2px solid currentColor;
      border-right-color: transparent;
      border-radius: 50%;
      animation: spin 0.6s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class ButtonComponent {
  variant = input<ButtonVariant>('primary');
  size = input<ButtonSize>('md');
  type = input<'button' | 'submit' | 'reset'>('button');
  disabled = input<boolean>(false);
  loading = input<boolean>(false);
  fullWidth = input<boolean>(false);

  clicked = output<MouseEvent>();

  buttonClasses() {
    return [
      `btn-${this.variant()}`,
      `btn-${this.size()}`,
      this.fullWidth() ? 'btn-full-width' : ''
    ].join(' ');
  }

  handleClick(event: MouseEvent) {
    if (!this.disabled() && !this.loading()) {
      this.clicked.emit(event);
    }
  }
}
