import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-estanco-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="estanco-card" [class.flat]="flat" [class]="variantClass()">
      <div class="accent" *ngIf="!flat"></div>
      <div class="estanco-card-content">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .estanco-card {
      position: relative;
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: 12px;
      padding: 18px;
      box-shadow: 0 8px 24px rgba(16,24,40,0.06);
      transition: transform 180ms ease, box-shadow 180ms ease;
      overflow: hidden;
    }
    .estanco-card:hover { transform: translateY(-6px); box-shadow: 0 18px 40px rgba(16,24,40,0.08); }
    .estanco-card.flat { box-shadow: none; transform: none; }

    .accent {
      position: absolute;
      left: 0;
      top: 0;
      height: 100%;
      width: 6px;
      background: var(--color-primary);
    }

    .estanco-card-content { position: relative; }

    /* Variants */
    .variant-primary { border-left-color: var(--color-primary); }
    .variant-accent { border-left-color: var(--color-accent); }
    .variant-info { border-left-color: #4b7bec; }
    .variant-danger { border-left-color: var(--color-error); }
  `]
})
export class EstancoCardComponent {
  @Input() flat = false;
  @Input() variant: 'primary' | 'accent' | 'info' | 'danger' | 'neutral' = 'neutral';

  variantClass() {
    if (!this.variant || this.variant === 'neutral') return '';
    return `variant-${this.variant}`;
  }
}
