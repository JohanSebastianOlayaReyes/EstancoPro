import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PurchaseService } from '../../core/services/purchase.service';
import { PurchaseProductDetailService } from '../../core/services/purchase-product-detail.service';
import { SupplierService } from '../../core/services/supplier.service';
import { ProductService } from '../../core/services/product.service';
import { PurchaseDto, PurchaseProductDetailDto } from '../../core/models/purchase.model';
import { SupplierDto } from '../../core/models/supplier.model';
import { ProductDto } from '../../core/models/product.model';

@Component({
  selector: 'app-purchases',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container">
      <h1>Gestión de Compras</h1>

      <div class="actions">
        <button (click)="showNewPurchase = !showNewPurchase">Nueva Compra</button>
      </div>

      @if (showNewPurchase) {
        <div class="form-card">
          <h2>Nueva Compra</h2>
          <div class="form-group">
            <label>Proveedor:</label>
            <select [(ngModel)]="newPurchase.supplierId">
              <option value="">Seleccione proveedor</option>
              @for (supplier of suppliers(); track supplier.id) {
                <option [value]="supplier.id">{{ supplier.name }}</option>
              }
            </select>
          </div>

          <h3>Productos</h3>
          <div class="product-selector">
            <select [(ngModel)]="selectedProductId">
              <option value="">Seleccione producto</option>
              @for (product of products(); track product.id) {
                <option [value]="product.id">{{ product.name }}</option>
              }
            </select>
            <input type="number" [(ngModel)]="productQuantity" placeholder="Cantidad" min="1" />
            <input type="number" [(ngModel)]="productCost" placeholder="Costo Unitario" min="0" step="0.01" />
            <button (click)="addProductToNewPurchase()">Agregar</button>
          </div>

          <div class="details-list">
            @for (detail of newPurchaseDetails(); track $index) {
              <div class="detail-item">
                <span>{{ detail.productName }}</span>
                <span>{{ detail.quantity }} unidades @ {{ detail.unitCost | currency }}</span>
                <span class="subtotal">{{ detail.subtotal | currency }}</span>
                <button class="btn-remove" (click)="removeDetailFromNew($index)">×</button>
              </div>
            }
          </div>

          <div class="total">
            <strong>Total: {{ calculateNewPurchaseTotal() | currency }}</strong>
          </div>

          <div class="form-actions">
            <button (click)="createPurchase()" [disabled]="!canCreatePurchase()">Crear Compra</button>
            <button class="btn-cancel" (click)="cancelNewPurchase()">Cancelar</button>
          </div>
        </div>
      }

      <div class="table-container">
        <h2>Listado de Compras</h2>
        <table>
          <thead>
            <tr>
              <th>Proveedor</th>
              <th>Fecha Pedido</th>
              <th>Fecha Recepción</th>
              <th>Total</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            @for (purchase of purchases(); track purchase.id) {
              <tr [attr.data-purchase-id]="purchase.id">
                <td>{{ purchase.supplierName }}</td>
                <td>{{ purchase.orderedAt | date:'short' }}</td>
                <td>{{ purchase.receivedAt ? (purchase.receivedAt | date:'short') : '-' }}</td>
                <td class="total">{{ purchase.totalCost | currency }}</td>
                <td>
                  <span class="status" [class.received]="purchase.status">
                    {{ purchase.status ? 'Recibido' : 'Pendiente' }}
                  </span>
                </td>
                <td>
                  @if (!purchase.status) {
                    <button (click)="receivePurchase(purchase.id!)">Recibir</button>
                    <button class="btn-danger" (click)="cancelPurchase(purchase.id!)">Cancelar</button>
                  }
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>

      @if (error()) {
        <div class="alert alert-error">{{ error() }}</div>
      }
      @if (success()) {
        <div class="alert alert-success">{{ success() }}</div>
      }
    </div>
  `,
  styles: [`
    .container { padding: 20px; max-width: 1200px; margin: 0 auto; }
    .actions { margin-bottom: 20px; }
    .form-card { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); margin-bottom: 20px; }
    .form-group { margin-bottom: 15px; }
    .form-group label { display: block; margin-bottom: 5px; font-weight: 500; }
    .form-group select, .form-group input { width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px; }
    .product-selector { display: grid; grid-template-columns: 2fr 1fr 1fr auto; gap: 10px; margin-bottom: 15px; }
    .details-list { margin: 15px 0; }
    .detail-item { display: flex; justify-content: space-between; align-items: center; padding: 10px; background: #f5f5f5; border-radius: 4px; margin-bottom: 8px; }
    .subtotal { font-weight: bold; color: #4CAF50; }
    .btn-remove { background: #f44336; color: white; border: none; width: 30px; height: 30px; border-radius: 50%; cursor: pointer; }
    .total { text-align: right; font-size: 18px; margin: 15px 0; }
    .form-actions { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
    button { padding: 10px 20px; border: none; border-radius: 4px; background: #2196F3; color: white; cursor: pointer; }
    button:hover { background: #1976D2; }
    button:disabled { background: #ccc; cursor: not-allowed; }
    .btn-cancel { background: #9e9e9e; }
    .btn-danger { background: #f44336; }
    .table-container { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
    table { width: 100%; border-collapse: collapse; }
    th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }
    th { background: #f5f5f5; font-weight: 600; }
    .total { font-weight: bold; color: #4CAF50; }
    .status { padding: 4px 8px; border-radius: 4px; font-size: 12px; background: #fff3e0; color: #e65100; }
    .status.received { background: #c8e6c9; color: #2e7d32; }
    .alert { padding: 12px; border-radius: 4px; margin-top: 15px; }
    .alert-error { background: #ffebee; color: #c62828; border: 1px solid #ef5350; }
    .alert-success { background: #e8f5e9; color: #2e7d32; border: 1px solid #66bb6a; }
  `]
})
export class PurchasesComponent implements OnInit {
  purchases = signal<PurchaseDto[]>([]);
  suppliers = signal<SupplierDto[]>([]);
  products = signal<ProductDto[]>([]);
  newPurchaseDetails = signal<any[]>([]);
  error = signal<string | null>(null);
  success = signal<string | null>(null);

  showNewPurchase = false;
  newPurchase: any = { supplierId: '' };
  selectedProductId = '';
  productQuantity = 1;
  productCost = 0;

  constructor(
    private purchaseService: PurchaseService,
    private purchaseDetailService: PurchaseProductDetailService,
    private supplierService: SupplierService,
    private productService: ProductService
  ) {}

  ngOnInit() {
    this.loadPurchases();
    this.loadSuppliers();
    this.loadProducts();
  }

  loadPurchases() {
    this.purchaseService.getAll().subscribe({
      next: (purchases) => this.purchases.set(purchases),
      error: (err) => this.error.set('Error al cargar compras')
    });
  }

  loadSuppliers() {
    this.supplierService.getAll().subscribe({
      next: (suppliers) => this.suppliers.set(suppliers),
      error: (err) => console.error(err)
    });
  }

  loadProducts() {
    this.productService.getAll().subscribe({
      next: (products) => this.products.set(products),
      error: (err) => console.error(err)
    });
  }

  addProductToNewPurchase() {
    const product = this.products().find(p => p.id === +this.selectedProductId);
    if (!product || this.productQuantity <= 0 || this.productCost <= 0) {
      this.error.set('Datos inválidos');
      return;
    }

    const detail = {
      productId: product.id,
      productName: product.name,
      quantity: this.productQuantity,
      unitCost: this.productCost,
      subtotal: this.productQuantity * this.productCost
    };

    this.newPurchaseDetails.update(details => [...details, detail]);
    this.selectedProductId = '';
    this.productQuantity = 1;
    this.productCost = 0;
  }

  removeDetailFromNew(index: number) {
    this.newPurchaseDetails.update(details => details.filter((_, i) => i !== index));
  }

  calculateNewPurchaseTotal(): number {
    return this.newPurchaseDetails().reduce((sum, d) => sum + d.subtotal, 0);
  }

  canCreatePurchase(): boolean {
    return !!this.newPurchase.supplierId && this.newPurchaseDetails().length > 0;
  }

  createPurchase() {
    const purchase: PurchaseDto = {
      orderedAt: new Date().toISOString(),
      status: false,
      totalCost: this.calculateNewPurchaseTotal(),
      supplierId: +this.newPurchase.supplierId
    };

    this.purchaseService.create(purchase).subscribe({
      next: (created) => {
        const detailPromises = this.newPurchaseDetails().map(d => {
          const detail: PurchaseProductDetailDto = {
            purchaseId: created.id!,
            productId: d.productId,
            quantity: d.quantity,
            unitCost: d.unitCost,
            subtotal: d.subtotal
          };
          return this.purchaseDetailService.create(detail).toPromise();
        });

        Promise.all(detailPromises).then(() => {
          this.success.set('Compra creada exitosamente');
          this.cancelNewPurchase();
          this.loadPurchases();
        });
      },
      error: (err) => this.error.set('Error al crear compra')
    });
  }

  cancelNewPurchase() {
    this.showNewPurchase = false;
    this.newPurchase = { supplierId: '' };
    this.newPurchaseDetails.set([]);
  }

  receivePurchase(id: number) {
    if (!confirm('¿Recibir esta compra? Esto actualizará el inventario.')) return;

    this.purchaseService.receivePurchase(id, { payInCash: false }).subscribe({
      next: () => {
        this.success.set('Compra recibida exitosamente');
        this.loadPurchases();
      },
      error: (err) => this.error.set('Error al recibir compra')
    });
  }

  cancelPurchase(id: number) {
    const reason = prompt('Razón de la cancelación:');
    if (!reason) return;

    this.purchaseService.cancelPurchase(id, { reason }).subscribe({
      next: () => {
        this.success.set('Compra cancelada');
        this.loadPurchases();
      },
      error: (err) => this.error.set('Error al cancelar compra')
    });
  }
}
