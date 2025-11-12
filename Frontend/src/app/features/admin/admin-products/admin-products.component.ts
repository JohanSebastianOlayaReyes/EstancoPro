import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import { UnitMeasureService } from '../../../core/services/unit-measure.service';
import { SidebarMenuComponent } from '../../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../../shared/config/menu-sections';
import { ProductDto } from '../../../core/models/product.model';
import { CategoryDto } from '../../../core/models/category.model';
import { UnitMeasureDto } from '../../../core/models/unit-measure.model';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarMenuComponent],
  templateUrl: './admin-products.component.html',
  styleUrls: ['./admin-products.component.css']
})
export class AdminProductsComponent implements OnInit {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private unitMeasureService = inject(UnitMeasureService);

  menuSections = ADMIN_MENU_SECTIONS;

  products = signal<ProductDto[]>([]);
  categories = signal<CategoryDto[]>([]);
  unitMeasures = signal<UnitMeasureDto[]>([]);

  isModalOpen = signal(false);
  isEditMode = signal(false);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  currentProduct = signal<ProductDto>({
    name: '',
    unitCost: 0,
    unitPrice: 0,
    taxRate: 0,
    stockOnHand: 0,
    reorderPoint: 0,
    categoryId: 0,
    unitMeasureId: 0,
    active: true
  });

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
    this.loadUnitMeasures();
  }

  loadProducts(): void {
    this.isLoading.set(true);
    this.productService.getAll().subscribe({
      next: (data) => {
        this.products.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.showError('Error al cargar los productos');
        this.isLoading.set(false);
        console.error('Error loading products:', error);
      }
    });
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => {
        this.categories.set(data.filter(c => c.active));
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadUnitMeasures(): void {
    this.unitMeasureService.getAll().subscribe({
      next: (data) => {
        this.unitMeasures.set(data.filter(u => u.active));
      },
      error: (error) => {
        console.error('Error loading unit measures:', error);
      }
    });
  }

  openCreateModal(): void {
    this.currentProduct.set({
      name: '',
      unitCost: 0,
      unitPrice: 0,
      taxRate: 0,
      stockOnHand: 0,
      reorderPoint: 0,
      categoryId: 0,
      unitMeasureId: 0,
      active: true
    });
    this.isEditMode.set(false);
    this.isModalOpen.set(true);
  }

  openEditModal(product: ProductDto): void {
    this.currentProduct.set({ ...product });
    this.isEditMode.set(true);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.errorMessage.set(null);
  }

  saveProduct(): void {
    const product = this.currentProduct();

    if (!this.validateProduct(product)) {
      return;
    }

    this.isLoading.set(true);

    if (this.isEditMode()) {
      this.productService.update(product.id!, product).subscribe({
        next: () => {
          this.showSuccess('Producto actualizado exitosamente');
          this.loadProducts();
          this.closeModal();
        },
        error: (error) => {
          this.showError('Error al actualizar el producto');
          console.error('Error updating product:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    } else {
      this.productService.create(product).subscribe({
        next: () => {
          this.showSuccess('Producto creado exitosamente');
          this.loadProducts();
          this.closeModal();
        },
        error: (error) => {
          this.showError('Error al crear el producto');
          console.error('Error creating product:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  deleteProduct(id: number | undefined): void {
    if (!id) return;

    if (confirm('¿Está seguro de que desea eliminar este producto?')) {
      this.isLoading.set(true);
      this.productService.deleteLogic(id).subscribe({
        next: () => {
          this.showSuccess('Producto eliminado exitosamente');
          this.loadProducts();
        },
        error: (error) => {
          this.showError('Error al eliminar el producto');
          console.error('Error deleting product:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  validateProduct(product: ProductDto): boolean {
    if (!product.name || product.name.trim() === '') {
      this.showError('El nombre del producto es requerido');
      return false;
    }

    if (product.unitCost < 0) {
      this.showError('El costo unitario debe ser mayor o igual a 0');
      return false;
    }

    if (product.unitPrice < 0) {
      this.showError('El precio unitario debe ser mayor o igual a 0');
      return false;
    }

    if (product.stockOnHand < 0) {
      this.showError('El stock disponible debe ser mayor o igual a 0');
      return false;
    }

    if (product.reorderPoint < 0) {
      this.showError('El punto de reorden debe ser mayor o igual a 0');
      return false;
    }

    if (!product.categoryId || product.categoryId === 0) {
      this.showError('Debe seleccionar una categoría');
      return false;
    }

    if (!product.unitMeasureId || product.unitMeasureId === 0) {
      this.showError('Debe seleccionar una unidad de medida');
      return false;
    }

    return true;
  }

  getCategoryName(categoryId: number): string {
    const category = this.categories().find(c => c.id === categoryId);
    return category?.name || 'N/A';
  }

  getUnitMeasureName(unitMeasureId: number): string {
    const unitMeasure = this.unitMeasures().find(u => u.id === unitMeasureId);
    return unitMeasure?.name || 'N/A';
  }

  showSuccess(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(null), 3000);
  }

  showError(message: string): void {
    this.errorMessage.set(message);
  }

  updateProductField<K extends keyof ProductDto>(field: K, value: ProductDto[K]): void {
    this.currentProduct.update(product => ({
      ...product,
      [field]: value
    }));
  }
}
