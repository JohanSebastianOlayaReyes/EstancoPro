import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CashService } from '../../core/services/cash.service';
import { ProductService } from '../../core/services/product.service';
import { SaleService } from '../../core/services/sale.service';
import { AuthUser, CashSession, Product, PaymentMethod, Customer, Sale } from '../../core/models';
import { ModalComponent } from '../../shared/components/modal/modal.component';

interface SaleItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
  availableStock: number;
}

@Component({
  selector: 'app-pos',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent],
  templateUrl: './pos.component.html',
  styleUrls: ['./pos.component.scss']
})
export class PosComponent implements OnInit {
  currentUser: AuthUser | null = null;
  currentSession: CashSession | null = null;

  // Products
  products: Product[] = [];
  filteredProducts: Product[] = [];
  searchTerm = '';
  loadingProducts = true;

  // Sale Cart
  saleItems: SaleItem[] = [];
  taxRate = 19; // IVA 19%

  // Payment
  paymentMethods: PaymentMethod[] = [];
  customers: Customer[] = [];
  selectedPaymentMethod: number | null = null;
  selectedCustomerId: number | null = null;
  saleObservations = '';

  // Modals
  showPaymentModal = false;
  showSuccessModal = false;
  lastSaleTotal = 0;

  constructor(
    private router: Router,
    private authService: AuthService,
    private cashService: CashService,
    private productService: ProductService,
    private saleService: SaleService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;

    // Subscribe to cash session
    this.cashService.currentSession$.subscribe(session => {
      this.currentSession = session;
    });

    // Load initial data
    this.loadProducts();
    this.loadPaymentMethods();
    this.loadCustomers();
  }

  loadProducts(): void {
    this.loadingProducts = true;
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products = products.filter(p => p.state);
        this.filteredProducts = this.products;
        this.loadingProducts = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.loadingProducts = false;
      }
    });
  }

  loadPaymentMethods(): void {
    this.saleService.getPaymentMethods().subscribe({
      next: (methods) => {
        this.paymentMethods = methods.filter(m => m.state);
      },
      error: (error) => {
        console.error('Error loading payment methods:', error);
      }
    });
  }

  loadCustomers(): void {
    this.saleService.getCustomers().subscribe({
      next: (customers) => {
        this.customers = customers.filter(c => c.state);
      },
      error: (error) => {
        console.error('Error loading customers:', error);
      }
    });
  }

  filterProducts(): void {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredProducts = this.products;
      return;
    }

    this.filteredProducts = this.products.filter(product =>
      product.name.toLowerCase().includes(term) ||
      product.code?.toLowerCase().includes(term) ||
      product.categoryName?.toLowerCase().includes(term)
    );
  }

  addProductToSale(product: Product): void {
    if (product.stockQuantity <= 0) {
      alert('Este producto no tiene stock disponible');
      return;
    }

    const existingItem = this.saleItems.find(item => item.productId === product.id);

    if (existingItem) {
      if (existingItem.quantity < product.stockQuantity) {
        existingItem.quantity++;
        existingItem.subtotal = existingItem.quantity * existingItem.unitPrice;
      } else {
        alert(`Stock máximo alcanzado (${product.stockQuantity} unidades)`);
      }
    } else {
      const newItem: SaleItem = {
        productId: product.id,
        productName: product.name,
        quantity: 1,
        unitPrice: product.salePrice,
        subtotal: product.salePrice,
        availableStock: product.stockQuantity
      };
      this.saleItems.push(newItem);
    }
  }

  removeFromSale(item: SaleItem): void {
    const index = this.saleItems.indexOf(item);
    if (index > -1) {
      this.saleItems.splice(index, 1);
    }
  }

  increaseQuantity(item: SaleItem): void {
    if (item.quantity < item.availableStock) {
      item.quantity++;
      item.subtotal = item.quantity * item.unitPrice;
    } else {
      alert(`Stock máximo alcanzado (${item.availableStock} unidades)`);
    }
  }

  decreaseQuantity(item: SaleItem): void {
    if (item.quantity > 1) {
      item.quantity--;
      item.subtotal = item.quantity * item.unitPrice;
    } else {
      this.removeFromSale(item);
    }
  }

  calculateSubtotal(): number {
    return this.saleItems.reduce((sum, item) => sum + item.subtotal, 0);
  }

  calculateTax(): number {
    return this.calculateSubtotal() * (this.taxRate / 100);
  }

  calculateTotal(): number {
    return this.calculateSubtotal() + this.calculateTax();
  }

  clearSale(): void {
    if (confirm('¿Estás seguro de que deseas limpiar el carrito?')) {
      this.saleItems = [];
    }
  }

  openPaymentModal(): void {
    if (!this.currentSession) {
      alert('Debes abrir una sesión de caja antes de realizar ventas');
      return;
    }
    this.showPaymentModal = true;
  }

  completeSale(): void {
    if (!this.selectedPaymentMethod) {
      alert('Debes seleccionar un método de pago');
      return;
    }

    if (!this.currentSession) {
      alert('No hay una sesión de caja abierta');
      return;
    }

    // Create sale object
    const saleData: Partial<Sale> = {
      cashSessionId: this.currentSession.id,
      customerId: this.selectedCustomerId,
      saleDate: new Date().toISOString(),
      totalAmount: this.calculateTotal(),
      taxAmount: this.calculateTax(),
      observations: this.saleObservations || null,
      status: 'Pendiente' as 'Pendiente',
      state: true
    };

    // Create sale
    this.saleService.create(saleData).subscribe({
      next: (sale) => {
        // Add products to sale
        const productPromises = this.saleItems.map(item => {
          const detailData = {
            saleId: sale.id,
            productId: item.productId,
            quantity: item.quantity,
            unitPrice: item.unitPrice,
            subtotal: item.subtotal,
            state: true
          };
          return this.saleService.addProduct(detailData).toPromise();
        });

        Promise.all(productPromises).then(() => {
          // Finalize sale
          this.saleService.finalizeSale(sale.id!).subscribe({
            next: () => {
              this.lastSaleTotal = this.calculateTotal();
              this.showPaymentModal = false;
              this.showSuccessModal = true;

              // Reset sale
              this.saleItems = [];
              this.selectedPaymentMethod = null;
              this.selectedCustomerId = null;
              this.saleObservations = '';

              // Reload products to update stock
              this.loadProducts();
            },
            error: (error) => {
              console.error('Error finalizing sale:', error);
              alert('Error al finalizar la venta: ' + (error.error?.message || 'Error desconocido'));
            }
          });
        });
      },
      error: (error) => {
        console.error('Error creating sale:', error);
        alert('Error al crear la venta: ' + (error.error?.message || 'Error desconocido'));
      }
    });
  }

  closeSuccessModal(): void {
    this.showSuccessModal = false;
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}
