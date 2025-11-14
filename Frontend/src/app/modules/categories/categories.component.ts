import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../core/services/product.service';
import { Category } from '../../core/models';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent],
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.scss']
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  filteredCategories: Category[] = [];

  // Filters
  searchTerm = '';
  selectedStatus = 'all';

  // Loading states
  loading = true;
  processingAction = false;

  // Category Modal
  showCategoryModal = false;
  editingCategory: Category | null = null;
  categoryForm: Partial<Category> = this.getEmptyCategoryForm();

  // Delete Modal
  showDeleteModal = false;
  categoryToDelete: Category | null = null;

  // Success Modal
  showSuccessModal = false;
  successMessage = '';

  constructor(
    private router: Router,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading = true;
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.filterCategories();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        alert('Error al cargar categorías');
        this.loading = false;
      }
    });
  }

  filterCategories(): void {
    let filtered = [...this.categories];

    // Search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(c =>
        c.name.toLowerCase().includes(term) ||
        c.description?.toLowerCase().includes(term)
      );
    }

    // Status filter
    switch (this.selectedStatus) {
      case 'active':
        filtered = filtered.filter(c => c.state);
        break;
      case 'inactive':
        filtered = filtered.filter(c => !c.state);
        break;
    }

    this.filteredCategories = filtered;
  }

  openAddCategoryModal(): void {
    this.editingCategory = null;
    this.categoryForm = this.getEmptyCategoryForm();
    this.showCategoryModal = true;
  }

  editCategory(category: Category): void {
    this.editingCategory = category;
    this.categoryForm = { ...category };
    this.showCategoryModal = true;
  }

  closeCategoryModal(): void {
    this.showCategoryModal = false;
    this.editingCategory = null;
    this.categoryForm = this.getEmptyCategoryForm();
  }

  saveCategory(): void {
    // Validation
    if (!this.categoryForm.name || !this.categoryForm.description) {
      alert('Por favor completa todos los campos obligatorios');
      return;
    }

    this.processingAction = true;

    const categoryData: Partial<Category> = {
      name: this.categoryForm.name,
      description: this.categoryForm.description,
      state: this.categoryForm.state !== undefined ? this.categoryForm.state : true
    };

    if (this.editingCategory) {
      this.productService.updateCategory(this.editingCategory.id, categoryData).subscribe({
        next: () => {
          this.processingAction = false;
          this.showCategoryModal = false;
          this.successMessage = 'Categoría actualizada exitosamente';
          this.showSuccessModal = true;
        },
        error: (error: any) => {
          console.error('Error saving category:', error);
          alert('Error al guardar categoría: ' + (error.error?.message || 'Error desconocido'));
          this.processingAction = false;
        }
      });
    } else {
      this.productService.createCategory(categoryData).subscribe({
        next: () => {
          this.processingAction = false;
          this.showCategoryModal = false;
          this.successMessage = 'Categoría creada exitosamente';
          this.showSuccessModal = true;
        },
        error: (error: any) => {
          console.error('Error saving category:', error);
          alert('Error al guardar categoría: ' + (error.error?.message || 'Error desconocido'));
          this.processingAction = false;
        }
      });
    }
  }

  confirmDelete(category: Category): void {
    this.categoryToDelete = category;
    this.showDeleteModal = true;
  }

  deleteCategory(): void {
    if (!this.categoryToDelete) return;

    this.processingAction = true;

    this.productService.deleteCategory(this.categoryToDelete.id).subscribe({
      next: () => {
        this.processingAction = false;
        this.showDeleteModal = false;
        this.successMessage = 'Categoría eliminada exitosamente';
        this.showSuccessModal = true;
        this.categoryToDelete = null;
      },
      error: (error) => {
        console.error('Error deleting category:', error);
        alert('Error al eliminar categoría: ' + (error.error?.message || 'Error desconocido'));
        this.processingAction = false;
      }
    });
  }

  getEmptyCategoryForm(): Partial<Category> {
    return {
      name: '',
      description: '',
      state: true
    };
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}
