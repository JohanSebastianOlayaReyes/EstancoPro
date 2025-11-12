import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="input-wrapper">
      @if (label()) {
        <label [for]="id()" class="input-label">
          {{ label() }}
          @if (required()) {
            <span class="required">*</span>
          }
        </label>
      }

      <div class="input-container" [class.has-error]="error()">
        @if (type() === 'textarea') {
          <textarea
            [id]="id()"
            [name]="name()"
            [placeholder]="placeholder()"
            [disabled]="disabled()"
            [required]="required()"
            [rows]="rows()"
            [(ngModel)]="internalValue"
            (ngModelChange)="onValueChange($event)"
            (blur)="onBlur()"
            class="input-field"
          ></textarea>
        } @else {
          <input
            [id]="id()"
            [type]="type()"
            [name]="name()"
            [placeholder]="placeholder()"
            [disabled]="disabled()"
            [required]="required()"
            [(ngModel)]="internalValue"
            (ngModelChange)="onValueChange($event)"
            (blur)="onBlur()"
            class="input-field"
          />
        }
      </div>

      @if (error()) {
        <span class="error-message">{{ error() }}</span>
      }

      @if (hint() && !error()) {
        <span class="hint-message">{{ hint() }}</span>
      }
    </div>
  `,
  styles: [`
    .input-wrapper {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
      width: 100%;
    }

    .input-label {
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-medium);
      color: var(--color-text-primary);

      .required {
        color: var(--color-error);
        margin-left: var(--space-1);
      }
    }

    .input-container {
      position: relative;

      &.has-error .input-field {
        border-color: var(--color-error);

        &:focus {
          outline-color: var(--color-error);
        }
      }
    }

    .input-field {
      width: 100%;
      padding: var(--space-3);
      font-family: var(--font-ui);
      font-size: var(--font-size-base);
      color: var(--color-text-primary);
      background-color: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      transition: all var(--transition-fast);

      &::placeholder {
        color: var(--color-text-muted);
      }

      &:hover:not(:disabled) {
        border-color: var(--color-primary-light);
      }

      &:focus {
        outline: 2px solid var(--color-primary);
        outline-offset: 0;
        border-color: var(--color-primary);
      }

      &:disabled {
        background-color: var(--color-surface-secondary);
        cursor: not-allowed;
        opacity: 0.6;
      }
    }

    textarea.input-field {
      resize: vertical;
      min-height: 80px;
    }

    .error-message {
      font-size: var(--font-size-sm);
      color: var(--color-error);
    }

    .hint-message {
      font-size: var(--font-size-sm);
      color: var(--color-text-muted);
    }
  `]
})
export class InputComponent {
  id = input<string>('');
  name = input<string>('');
  type = input<'text' | 'password' | 'email' | 'number' | 'tel' | 'textarea'>('text');
  label = input<string>('');
  placeholder = input<string>('');
  value = input<string>('');
  disabled = input<boolean>(false);
  required = input<boolean>(false);
  error = input<string>('');
  hint = input<string>('');
  rows = input<number>(4);

  valueChange = output<string>();
  blurred = output<void>();

  internalValue = signal('');

  ngOnInit() {
    this.internalValue.set(this.value());
  }

  onValueChange(newValue: string) {
    this.internalValue.set(newValue);
    this.valueChange.emit(newValue);
  }

  onBlur() {
    this.blurred.emit();
  }
}
