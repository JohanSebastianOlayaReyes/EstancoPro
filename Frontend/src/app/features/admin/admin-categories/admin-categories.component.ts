import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../../core/services/category.service';
import { SidebarMenuComponent } from '../../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../../shared/config/menu-sections';
import { CategoryDto } from '../../../core/models/category.model';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarMenuComponent],
  templateUrl: './admin-categories.component.html',
  styleUrls: ['./admin-categories.component.css']
})
export class AdminCategoriesComponent implements OnInit {
  private categoryService = inject(CategoryService);

  menuSections = ADMIN_MENU_SECTIONS;

  categories = signal<CategoryDto[]>([]);
  isModalOpen = signal(false);
  isEditMode = signal(false);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  currentCategory = signal<CategoryDto>({
    name: '',
    description: '',
    active: true
  });

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading.set(true);
    this.categoryService.getAll().subscribe({
      next: (data) => {
        this.categories.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.showError('Error al cargar las categorías');
        this.isLoading.set(false);
        console.error('Error loading categories:', error);
      }
    });
  }

  openCreateModal(): void {
    this.currentCategory.set({
      name: '',
      description: '',
      active: true
    });
    this.isEditMode.set(false);
    this.isModalOpen.set(true);
  }

  openEditModal(category: CategoryDto): void {
    this.currentCategory.set({ ...category });
    this.isEditMode.set(true);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.errorMessage.set(null);
  }

  saveCategory(): void {
    const category = this.currentCategory();

    if (!this.validateCategory(category)) {
      return;
    }

    this.isLoading.set(true);

    if (this.isEditMode()) {
      this.categoryService.update(category).subscribe({
        next: () => {
          this.showSuccess('Categoría actualizada exitosamente');
          this.loadCategories();
          this.closeModal();
        },
        error: (error) => {
          this.showError('Error al actualizar la categoría');
          console.error('Error updating category:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    } else {
      this.categoryService.create(category).subscribe({
        next: () => {
          this.showSuccess('Categoría creada exitosamente');
          this.loadCategories();
          this.closeModal();
        },
        error: (error) => {
          this.showError('Error al crear la categoría');
          console.error('Error creating category:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  deleteCategory(id: number | undefined): void {
    if (!id) return;

    if (confirm('¿Está seguro de que desea eliminar esta categoría?')) {
      this.isLoading.set(true);
      this.categoryService.delete(id).subscribe({
        next: () => {
          this.showSuccess('Categoría eliminada exitosamente');
          this.loadCategories();
        },
        error: (error) => {
          this.showError('Error al eliminar la categoría');
          console.error('Error deleting category:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  validateCategory(category: CategoryDto): boolean {
    if (!category.name || category.name.trim() === '') {
      this.showError('El nombre de la categoría es requerido');
      return false;
    }

    if (!category.description || category.description.trim() === '') {
      this.showError('La descripción de la categoría es requerida');
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

  updateCategoryField<K extends keyof CategoryDto>(field: K, value: CategoryDto[K]): void {
    this.currentCategory.update(category => ({
      ...category,
      [field]: value
    }));
  }
}
