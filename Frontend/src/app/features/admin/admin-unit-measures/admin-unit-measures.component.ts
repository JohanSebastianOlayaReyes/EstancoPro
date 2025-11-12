import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UnitMeasureService } from '../../../core/services/unit-measure.service';
import { SidebarMenuComponent } from '../../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../../shared/config/menu-sections';
import { UnitMeasureDto } from '../../../core/models/unit-measure.model';

@Component({
  selector: 'app-admin-unit-measures',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarMenuComponent],
  templateUrl: './admin-unit-measures.component.html',
  styleUrls: ['./admin-unit-measures.component.css']
})
export class AdminUnitMeasuresComponent implements OnInit {
  private unitMeasureService = inject(UnitMeasureService);

  menuSections = ADMIN_MENU_SECTIONS;

  unitMeasures = signal<UnitMeasureDto[]>([]);
  isModalOpen = signal(false);
  isEditMode = signal(false);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  currentUnitMeasure = signal<UnitMeasureDto>({
    name: '',
    description: null,
    active: true
  });

  ngOnInit(): void {
    this.loadUnitMeasures();
  }

  loadUnitMeasures(): void {
    this.isLoading.set(true);
    this.unitMeasureService.getAll().subscribe({
      next: (data) => {
        this.unitMeasures.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.showError('Error al cargar las unidades de medida');
        this.isLoading.set(false);
        console.error('Error loading unit measures:', error);
      }
    });
  }

  openCreateModal(): void {
    this.currentUnitMeasure.set({
      name: '',
      description: null,
      active: true
    });
    this.isEditMode.set(false);
    this.isModalOpen.set(true);
  }

  openEditModal(unitMeasure: UnitMeasureDto): void {
    this.currentUnitMeasure.set({ ...unitMeasure });
    this.isEditMode.set(true);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.errorMessage.set(null);
  }

  saveUnitMeasure(): void {
    const unitMeasure = this.currentUnitMeasure();

    if (!this.validateUnitMeasure(unitMeasure)) {
      return;
    }

    this.isLoading.set(true);

    if (this.isEditMode()) {
      this.unitMeasureService.update(unitMeasure).subscribe({
        next: () => {
          this.showSuccess('Unidad de medida actualizada exitosamente');
          this.loadUnitMeasures();
          this.closeModal();
        },
        error: (error) => {
          this.showError('Error al actualizar la unidad de medida');
          console.error('Error updating unit measure:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    } else {
      this.unitMeasureService.create(unitMeasure).subscribe({
        next: () => {
          this.showSuccess('Unidad de medida creada exitosamente');
          this.loadUnitMeasures();
          this.closeModal();
        },
        error: (error) => {
          this.showError('Error al crear la unidad de medida');
          console.error('Error creating unit measure:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  deleteUnitMeasure(id: number | undefined): void {
    if (!id) return;

    if (confirm('¿Está seguro de que desea eliminar esta unidad de medida?')) {
      this.isLoading.set(true);
      this.unitMeasureService.delete(id).subscribe({
        next: () => {
          this.showSuccess('Unidad de medida eliminada exitosamente');
          this.loadUnitMeasures();
        },
        error: (error) => {
          this.showError('Error al eliminar la unidad de medida');
          console.error('Error deleting unit measure:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  validateUnitMeasure(unitMeasure: UnitMeasureDto): boolean {
    if (!unitMeasure.name || unitMeasure.name.trim() === '') {
      this.showError('El nombre de la unidad de medida es requerido');
      return false;
    }

    return true;
  }

  showSuccess(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(null), 3000);
  }

  showError(message: string): void {
    this.errorMessage.set(message);
  }

  updateUnitMeasureField<K extends keyof UnitMeasureDto>(field: K, value: UnitMeasureDto[K]): void {
    this.currentUnitMeasure.update(unitMeasure => ({
      ...unitMeasure,
      [field]: value
    }));
  }
}
