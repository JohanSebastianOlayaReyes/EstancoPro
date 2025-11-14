import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PurchaseService } from '../../core/services/purchase.service';
import { ProductService } from '../../core/services/product.service';
import { Purchase, Supplier, Product, PurchaseProductDetail } from '../../core/models';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Component({
  selector: 'app-purchases',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent],
  templateUrl: './purchases.component.html',
  styleUrls: ['./purchases.component.scss']
})
export class PurchasesComponent implements OnInit {
  purchases: Purchase[] = [];
  filteredPurchases: Purchase[] = [];
  suppliers: Supplier[] = [];
  products: Product[] = [];

  // Filters
  searchTerm = '';
  selectedStatus: string = 'all';
  selectedSupplierId: number | null = null;

  // Loading states
  loading = true;
  processingAction = false;

  // Purchase Modal
  showPurchaseModal = false;
  purchaseForm: Partial<Purchase> = this.getEmptyPurchaseForm();
  purchaseItems: Partial<PurchaseProductDetail>[] = [];

  // Detail Modal
  showDetailModal = false;
  selectedPurchase: Purchase | null = null;

  // Receive Modal
  showReceiveModal = false;
  purchaseToReceive: Purchase | null = null;

  // Cancel Modal
  showCancelModal = false;
  purchaseToCancel: Purchase | null = null;

  // Success Modal
  showSuccessModal = false;
  successMessage = '';

  constructor(
    private router: Router,
    private purchaseService: PurchaseService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.loadPurchases();
    this.loadSuppliers();
    this.loadProducts();
  }

  loadPurchases(): void {
    this.loading = true;
    this.purchaseService.getAll().subscribe({
      next: (purchases) => {
        this.purchases = purchases;
        this.filterPurchases();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading purchases:', error);
        this.loading = false;
      }
    });
  }

  loadSuppliers(): void {
    this.purchaseService.getSuppliers().subscribe({
      next: (suppliers) => {
        this.suppliers = suppliers;
      },
      error: (error) => {
        console.error('Error loading suppliers:', error);
      }
    });
  }

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products = products;
      },
      error: (error) => {
        console.error('Error loading products:', error);
      }
    });
  }

  filterPurchases(): void {
    let filtered = [...this.purchases];

    // Search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(p =>
        p.invoiceNumber.toLowerCase().includes(term) ||
        p.supplier?.name.toLowerCase().includes(term)
      );
    }

    // Status filter
    if (this.selectedStatus !== 'all') {
      filtered = filtered.filter(p => p.status === this.selectedStatus);
    }

    // Supplier filter
    if (this.selectedSupplierId) {
      filtered = filtered.filter(p => p.supplierId === this.selectedSupplierId);
    }

    this.filteredPurchases = filtered;
  }

  openAddPurchaseModal(): void {
    this.purchaseForm = this.getEmptyPurchaseForm();
    this.purchaseItems = [];
    this.showPurchaseModal = true;
  }

  closePurchaseModal(): void {
    this.showPurchaseModal = false;
    this.purchaseForm = this.getEmptyPurchaseForm();
    this.purchaseItems = [];
  }

  addPurchaseItem(): void {
    this.purchaseItems.push({
      productId: undefined,
      quantity: 1,
      unitCost: 0,
      subtotal: 0,
      taxAmount: 0,
      active: true
    });
  }

  removePurchaseItem(index: number): void {
    this.purchaseItems.splice(index, 1);
    this.calculateTotals();
  }

  updateItemSubtotal(item: Partial<PurchaseProductDetail>): void {
    if (item.quantity && item.unitCost) {
      item.subtotal = item.quantity * item.unitCost;
      item.taxAmount = item.subtotal * 0.19; // IVA 19%
    }
    this.calculateTotals();
  }

  calculateTotals(): void {
    this.purchaseForm.subtotal = this.purchaseItems.reduce((sum, item) => sum + (item.subtotal || 0), 0);
    this.purchaseForm.taxTotal = this.purchaseItems.reduce((sum, item) => sum + (item.taxAmount || 0), 0);
    this.purchaseForm.grandTotal = this.purchaseForm.subtotal + this.purchaseForm.taxTotal;
  }

  savePurchase(): void {
    // Validation
    if (!this.purchaseForm.invoiceNumber || !this.purchaseForm.supplierId) {
      alert('Por favor completa todos los campos obligatorios');
      return;
    }

    if (this.purchaseItems.length === 0) {
      alert('Debes agregar al menos un producto');
      return;
    }

    // Validate all items
    const hasInvalidItems = this.purchaseItems.some(item =>
      !item.productId || !item.quantity || item.quantity <= 0 || !item.unitCost || item.unitCost <= 0
    );

    if (hasInvalidItems) {
      alert('Todos los productos deben tener cantidad y costo válidos');
      return;
    }

    this.processingAction = true;

    const purchaseData: Partial<Purchase> = {
      invoiceNumber: this.purchaseForm.invoiceNumber,
      supplierId: this.purchaseForm.supplierId,
      subtotal: this.purchaseForm.subtotal,
      taxTotal: this.purchaseForm.taxTotal,
      grandTotal: this.purchaseForm.grandTotal,
      purchaseProductDetails: this.purchaseItems as PurchaseProductDetail[]
    };

    this.purchaseService.create(purchaseData).subscribe({
      next: () => {
        this.processingAction = false;
        this.showPurchaseModal = false;
        this.successMessage = 'Compra creada exitosamente';
        this.showSuccessModal = true;
      },
      error: (error: any) => {
        console.error('Error saving purchase:', error);
        alert('Error al guardar compra');
        this.processingAction = false;
      }
    });
  }

  viewPurchaseDetails(purchase: Purchase): void {
    this.selectedPurchase = purchase;
    this.showDetailModal = true;
  }

  openReceiveModal(purchase: Purchase): void {
    this.purchaseToReceive = purchase;
    this.showReceiveModal = true;
  }

  receivePurchase(): void {
    if (!this.purchaseToReceive) return;

    this.processingAction = true;
    this.purchaseService.receive(this.purchaseToReceive.id).subscribe({
      next: () => {
        this.processingAction = false;
        this.showReceiveModal = false;
        this.successMessage = 'Compra recibida exitosamente';
        this.showSuccessModal = true;
        this.purchaseToReceive = null;
      },
      error: (error) => {
        console.error('Error receiving purchase:', error);
        alert('Error al recibir compra');
        this.processingAction = false;
      }
    });
  }

  openCancelModal(purchase: Purchase): void {
    this.purchaseToCancel = purchase;
    this.showCancelModal = true;
  }

  cancelPurchase(): void {
    if (!this.purchaseToCancel) return;

    this.processingAction = true;
    this.purchaseService.cancel(this.purchaseToCancel.id).subscribe({
      next: () => {
        this.processingAction = false;
        this.showCancelModal = false;
        this.successMessage = 'Compra cancelada exitosamente';
        this.showSuccessModal = true;
        this.purchaseToCancel = null;
      },
      error: (error) => {
        console.error('Error cancelling purchase:', error);
        alert('Error al cancelar compra');
        this.processingAction = false;
      }
    });
  }

  getEmptyPurchaseForm(): Partial<Purchase> {
    return {
      invoiceNumber: '',
      supplierId: undefined,
      subtotal: 0,
      taxTotal: 0,
      grandTotal: 0
    };
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Ordered':
        return 'bg-warning/10 text-warning';
      case 'Received':
        return 'bg-success/10 text-success';
      case 'Cancelled':
        return 'bg-error/10 text-error';
      default:
        return 'bg-gray-100 text-gray-600';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Ordered':
        return 'Ordenada';
      case 'Received':
        return 'Recibida';
      case 'Cancelled':
        return 'Cancelada';
      default:
        return status;
    }
  }

  getProductName(productId: number | undefined): string {
    if (!productId) return '';
    const product = this.products.find(p => p.id === productId);
    return product?.name || '';
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}
