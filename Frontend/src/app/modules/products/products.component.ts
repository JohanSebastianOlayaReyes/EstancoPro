import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../core/services/product.service';
import { Product, Category, UnitMeasure } from '../../core/models';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  categories: Category[] = [];
  unitMeasures: UnitMeasure[] = [];

  // Filters
  searchTerm = '';
  selectedCategoryId: number | null = null;
  selectedStatus = 'all';

  // Loading states
  loading = true;
  processingAction = false;

  // Product Modal
  showProductModal = false;
  editingProduct: Product | null = null;
  productForm: Partial<Product> = this.getEmptyProductForm();

  // Delete Modal
  showDeleteModal = false;
  productToDelete: Product | null = null;

  // Success Modal
  showSuccessModal = false;
  successMessage = '';

  // File handling
  selectedFile: File | null = null;

  constructor(
    private router: Router,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
    this.loadUnitMeasures();
  }

  loadProducts(): void {
    this.loading = true;
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products = products;
        this.filterProducts();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        alert('Error al cargar productos');
        this.loading = false;
      }
    });
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadUnitMeasures(): void {
    this.productService.getUnitMeasures().subscribe({
      next: (units) => {
        this.unitMeasures = units;
      },
      error: (error) => {
        console.error('Error loading unit measures:', error);
      }
    });
  }

  filterProducts(): void {
    let filtered = [...this.products];

    // Search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(p =>
        p.name.toLowerCase().includes(term) ||
        p.code.toLowerCase().includes(term) ||
        p.description?.toLowerCase().includes(term)
      );
    }

    // Category filter
    if (this.selectedCategoryId) {
      filtered = filtered.filter(p => p.categoryId === this.selectedCategoryId);
    }

    // Status filter
    switch (this.selectedStatus) {
      case 'active':
        filtered = filtered.filter(p => p.state);
        break;
      case 'inactive':
        filtered = filtered.filter(p => !p.state);
        break;
      case 'low-stock':
        filtered = filtered.filter(p => p.stockQuantity <= p.minStock);
        break;
    }

    this.filteredProducts = filtered;
  }

  openAddProductModal(): void {
    this.editingProduct = null;
    this.productForm = this.getEmptyProductForm();
    this.selectedFile = null;
    this.showProductModal = true;
  }

  editProduct(product: Product): void {
    this.editingProduct = product;
    this.productForm = { ...product };
    this.selectedFile = null;
    this.showProductModal = true;
  }

  closeProductModal(): void {
    this.showProductModal = false;
    this.editingProduct = null;
    this.productForm = this.getEmptyProductForm();
    this.selectedFile = null;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.selectedFile = input.files[0];

      // Preview image
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        this.productForm.photo = e.target?.result as string;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  removePhoto(): void {
    this.productForm.photo = null;
    this.selectedFile = null;
  }

  saveProduct(): void {
    // Validation
    if (!this.productForm.code || !this.productForm.name || !this.productForm.categoryId ||
        !this.productForm.unitMeasureId || this.productForm.purchasePrice === undefined ||
        this.productForm.salePrice === undefined || this.productForm.stockQuantity === undefined ||
        this.productForm.minStock === undefined || this.productForm.maxStock === undefined) {
      alert('Por favor completa todos los campos obligatorios');
      return;
    }

    if (this.productForm.salePrice < this.productForm.purchasePrice) {
      alert('El precio de venta debe ser mayor o igual al precio de compra');
      return;
    }

    if (this.productForm.minStock > this.productForm.maxStock) {
      alert('El stock mínimo no puede ser mayor al stock máximo');
      return;
    }

    this.processingAction = true;

    const productData: Partial<Product> = {
      code: this.productForm.code,
      name: this.productForm.name,
      description: this.productForm.description || '',
      photo: this.productForm.photo || null,
      purchasePrice: this.productForm.purchasePrice,
      salePrice: this.productForm.salePrice,
      stockQuantity: this.productForm.stockQuantity,
      minStock: this.productForm.minStock,
      maxStock: this.productForm.maxStock,
      expirationDate: this.productForm.expirationDate || null,
      categoryId: this.productForm.categoryId,
      unitMeasureId: this.productForm.unitMeasureId,
      state: this.productForm.state !== undefined ? this.productForm.state : true
    };

    if (this.editingProduct) {
      this.productService.update(this.editingProduct.id, productData).subscribe({
        next: () => {
          this.processingAction = false;
          this.showProductModal = false;
          this.successMessage = 'Producto actualizado exitosamente';
          this.showSuccessModal = true;
        },
        error: (error: any) => {
          console.error('Error saving product:', error);
          alert('Error al guardar producto: ' + (error.error?.message || 'Error desconocido'));
          this.processingAction = false;
        }
      });
    } else {
      this.productService.create(productData).subscribe({
        next: () => {
          this.processingAction = false;
          this.showProductModal = false;
          this.successMessage = 'Producto creado exitosamente';
          this.showSuccessModal = true;
        },
        error: (error: any) => {
          console.error('Error saving product:', error);
          alert('Error al guardar producto: ' + (error.error?.message || 'Error desconocido'));
          this.processingAction = false;
        }
      });
    }
  }

  confirmDelete(product: Product): void {
    this.productToDelete = product;
    this.showDeleteModal = true;
  }

  deleteProduct(): void {
    if (!this.productToDelete) return;

    this.processingAction = true;

    this.productService.delete(this.productToDelete.id).subscribe({
      next: () => {
        this.processingAction = false;
        this.showDeleteModal = false;
        this.successMessage = 'Producto eliminado exitosamente';
        this.showSuccessModal = true;
        this.productToDelete = null;
      },
      error: (error) => {
        console.error('Error deleting product:', error);
        alert('Error al eliminar producto: ' + (error.error?.message || 'Error desconocido'));
        this.processingAction = false;
      }
    });
  }

  getEmptyProductForm(): Partial<Product> {
    return {
      code: '',
      name: '',
      description: '',
      photo: null,
      purchasePrice: 0,
      salePrice: 0,
      stockQuantity: 0,
      minStock: 0,
      maxStock: 0,
      expirationDate: null,
      categoryId: undefined,
      unitMeasureId: undefined,
      state: true
    };
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}
