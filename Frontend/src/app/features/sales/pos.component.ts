import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SaleService } from '../../core/services/sale.service';
import { SaleProductDetailService } from '../../core/services/sale-product-detail.service';
import { ProductService } from '../../core/services/product.service';
import { CashSessionService } from '../../core/services/cash-session.service';
import { ProductUnitPriceService, ProductUnitPriceDto } from '../../core/services/product-unit-price.service';
import { SaleDto, SaleProductDetailDto } from '../../core/models/sale.model';
import { ProductDto } from '../../core/models/product.model';
import { CashSessionDto } from '../../core/models/cash.model';

interface CartItem {
  product: ProductDto;
  unitPrice: ProductUnitPriceDto;
  quantity: number;
  subtotal: number;
}

@Component({
  selector: 'app-pos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="pos-container">
      <!-- Header -->
      <div class="pos-header">
        <h1>Punto de Venta (POS)</h1>
        @if (currentSession()) {
          <div class="session-info">
            <span>Sesión de Caja #{{ currentSession()!.id }}</span>
            <span>Apertura: {{ currentSession()!.openingAmount | currency }}</span>
          </div>
        } @else {
          <div class="alert alert-warning">
            No hay sesión de caja abierta. <button (click)="goToSessions()">Abrir Sesión</button>
          </div>
        }
      </div>

      <div class="pos-content">
        <!-- Products Section -->
        <div class="products-section">
          <h2>Productos</h2>
          <input
            type="text"
            placeholder="Buscar producto..."
            [(ngModel)]="searchTerm"
            (ngModelChange)="filterProducts()"
            class="search-input"
          />

          <div class="products-grid">
            @for (product of filteredProducts(); track product.id) {
              <div class="product-card" (click)="selectProduct(product)">
                <h3>{{ product.name }}</h3>
                <p class="category">{{ product.categoryName }}</p>
                <p class="price">{{ product.unitPrice | currency }}</p>
                <p class="stock" [class.low-stock]="product.stockOnHand <= product.reorderPoint">
                  Stock: {{ product.stockOnHand }} {{ product.unitMeasureName }}
                </p>
              </div>
            }
          </div>
        </div>

        <!-- Cart Section -->
        <div class="cart-section">
          <h2>Carrito</h2>

          @if (selectedProduct()) {
            <div class="product-selector">
              <h3>{{ selectedProduct()!.name }}</h3>
              <div class="presentations">
                @for (presentation of productPresentations(); track presentation.unitMeasureId) {
                  <button
                    class="presentation-btn"
                    (click)="selectPresentation(presentation)"
                  >
                    {{ presentation.unitMeasureName }} - {{ presentation.price | currency }}
                  </button>
                }
              </div>
              <button class="btn-cancel" (click)="cancelSelection()">Cancelar</button>
            </div>
          }

          @if (selectedPresentation()) {
            <div class="quantity-selector">
              <h3>Cantidad</h3>
              <input
                type="number"
                [(ngModel)]="quantity"
                min="1"
                class="quantity-input"
              />
              <button class="btn-add" (click)="addToCart()">Agregar al Carrito</button>
              <button class="btn-cancel" (click)="cancelPresentation()">Cancelar</button>
            </div>
          }

          <div class="cart-items">
            @for (item of cart(); track $index) {
              <div class="cart-item">
                <div class="item-info">
                  <h4>{{ item.product.name }}</h4>
                  <p>{{ item.unitPrice.unitMeasureName }}</p>
                  <p>{{ item.unitPrice.price | currency }} x {{ item.quantity }}</p>
                </div>
                <div class="item-actions">
                  <span class="subtotal">{{ item.subtotal | currency }}</span>
                  <button class="btn-remove" (click)="removeFromCart($index)">×</button>
                </div>
              </div>
            }
          </div>

          <div class="cart-totals">
            <div class="total-line">
              <span>Subtotal:</span>
              <span>{{ totals().subtotal | currency }}</span>
            </div>
            <div class="total-line">
              <span>Impuestos ({{ TAX_RATE * 100 }}%):</span>
              <span>{{ totals().tax | currency }}</span>
            </div>
            <div class="total-line grand-total">
              <span>Total:</span>
              <span>{{ totals().grandTotal | currency }}</span>
            </div>
          </div>

          <div class="cart-actions">
            <button
              class="btn-clear"
              (click)="clearCart()"
              [disabled]="cart().length === 0"
            >
              Limpiar
            </button>
            <button
              class="btn-finalize"
              (click)="finalizeSale()"
              [disabled]="cart().length === 0 || !currentSession() || loading()"
            >
              {{ loading() ? 'Procesando...' : 'Finalizar Venta' }}
            </button>
          </div>

          @if (error()) {
            <div class="alert alert-error">{{ error() }}</div>
          }
          @if (success()) {
            <div class="alert alert-success">{{ success() }}</div>
          }
        </div>
      </div>
    </div>
  `,
  styles: [`
    .pos-container {
      padding: 20px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .pos-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
      padding-bottom: 10px;
      border-bottom: 2px solid #e0e0e0;
    }

    .session-info {
      display: flex;
      gap: 20px;
      font-size: 14px;
      color: #666;
    }

    .pos-content {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 20px;
    }

    .products-section, .cart-section {
      background: white;
      padding: 20px;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .search-input {
      width: 100%;
      padding: 10px;
      margin-bottom: 20px;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 14px;
    }

    .products-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 15px;
      max-height: 600px;
      overflow-y: auto;
    }

    .product-card {
      padding: 15px;
      border: 1px solid #ddd;
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.3s ease;
    }

    .product-card:hover {
      border-color: #4CAF50;
      box-shadow: 0 2px 8px rgba(76, 175, 80, 0.2);
    }

    .product-card h3 {
      margin: 0 0 5px 0;
      font-size: 16px;
    }

    .product-card .category {
      font-size: 12px;
      color: #666;
      margin: 0 0 10px 0;
    }

    .product-card .price {
      font-size: 18px;
      font-weight: bold;
      color: #4CAF50;
      margin: 0 0 5px 0;
    }

    .product-card .stock {
      font-size: 12px;
      color: #666;
      margin: 0;
    }

    .product-card .low-stock {
      color: #f44336;
      font-weight: bold;
    }

    .product-selector, .quantity-selector {
      padding: 15px;
      background: #f5f5f5;
      border-radius: 8px;
      margin-bottom: 20px;
    }

    .presentations {
      display: flex;
      flex-direction: column;
      gap: 10px;
      margin: 15px 0;
    }

    .presentation-btn {
      padding: 12px;
      background: white;
      border: 1px solid #ddd;
      border-radius: 4px;
      cursor: pointer;
      text-align: left;
      transition: all 0.2s;
    }

    .presentation-btn:hover {
      border-color: #4CAF50;
      background: #f1f8f4;
    }

    .quantity-input {
      width: 100%;
      padding: 10px;
      margin: 15px 0;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 16px;
    }

    .cart-items {
      max-height: 300px;
      overflow-y: auto;
      margin: 20px 0;
    }

    .cart-item {
      display: flex;
      justify-content: space-between;
      padding: 15px;
      border-bottom: 1px solid #eee;
    }

    .item-info h4 {
      margin: 0 0 5px 0;
      font-size: 14px;
    }

    .item-info p {
      margin: 0;
      font-size: 12px;
      color: #666;
    }

    .item-actions {
      display: flex;
      align-items: center;
      gap: 15px;
    }

    .subtotal {
      font-weight: bold;
      color: #4CAF50;
    }

    .btn-remove {
      background: #f44336;
      color: white;
      border: none;
      width: 30px;
      height: 30px;
      border-radius: 50%;
      cursor: pointer;
      font-size: 20px;
      line-height: 1;
    }

    .cart-totals {
      border-top: 2px solid #eee;
      padding-top: 15px;
      margin-top: 15px;
    }

    .total-line {
      display: flex;
      justify-content: space-between;
      margin-bottom: 10px;
      font-size: 14px;
    }

    .grand-total {
      font-size: 18px;
      font-weight: bold;
      color: #4CAF50;
      border-top: 1px solid #eee;
      padding-top: 10px;
      margin-top: 10px;
    }

    .cart-actions {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 10px;
      margin-top: 20px;
    }

    button {
      padding: 12px 24px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 500;
      transition: all 0.2s;
    }

    .btn-add {
      background: #4CAF50;
      color: white;
    }

    .btn-add:hover {
      background: #45a049;
    }

    .btn-cancel {
      background: #9e9e9e;
      color: white;
    }

    .btn-clear {
      background: #ff9800;
      color: white;
    }

    .btn-finalize {
      background: #2196F3;
      color: white;
    }

    .btn-finalize:disabled, .btn-clear:disabled {
      background: #ccc;
      cursor: not-allowed;
    }

    .alert {
      padding: 12px;
      border-radius: 4px;
      margin-top: 15px;
    }

    .alert-error {
      background: #ffebee;
      color: #c62828;
      border: 1px solid #ef5350;
    }

    .alert-success {
      background: #e8f5e9;
      color: #2e7d32;
      border: 1px solid #66bb6a;
    }

    .alert-warning {
      background: #fff3e0;
      color: #e65100;
      border: 1px solid #ffb74d;
    }
  `]
})
export class PosComponent implements OnInit {
  // Services injected using Angular's inject() function
  private saleService = inject(SaleService);
  private saleDetailService = inject(SaleProductDetailService);
  private productService = inject(ProductService);
  private cashSessionService = inject(CashSessionService);
  private productUnitPriceService = inject(ProductUnitPriceService);
  private router = inject(Router);

  TAX_RATE = 0.19; // 19% IVA

  products = signal<ProductDto[]>([]);
  filteredProducts = signal<ProductDto[]>([]);
  cart = signal<CartItem[]>([]);
  currentSession = signal<CashSessionDto | null>(null);
  selectedProduct = signal<ProductDto | null>(null);
  selectedPresentation = signal<ProductUnitPriceDto | null>(null);
  productPresentations = signal<ProductUnitPriceDto[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  success = signal<string | null>(null);

  searchTerm = '';
  quantity = 1;

  totals = computed(() => {
    const items = this.cart();
    const subtotal = items.reduce((sum, item) => sum + item.subtotal, 0);
    const tax = subtotal * this.TAX_RATE;
    const grandTotal = subtotal + tax;
    return { subtotal, tax, grandTotal };
  });

  async ngOnInit() {
    await this.loadCurrentSession();
    await this.loadProducts();
  }

  async loadCurrentSession() {
    try {
      this.cashSessionService.getOpenSession().subscribe({
        next: (session) => {
          this.currentSession.set(session);
        },
        error: (err) => {
          console.error('Error loading session:', err);
        }
      });
    } catch (err) {
      console.error('Error:', err);
    }
  }

  async loadProducts() {
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products.set(products);
        this.filteredProducts.set(products);
      },
      error: (err) => {
        this.error.set('Error al cargar productos');
        console.error(err);
      }
    });
  }

  filterProducts() {
    const term = this.searchTerm.toLowerCase();
    const filtered = this.products().filter(p =>
      p.name.toLowerCase().includes(term) ||
      p.categoryName?.toLowerCase().includes(term)
    );
    this.filteredProducts.set(filtered);
  }

  selectProduct(product: ProductDto) {
    this.selectedProduct.set(product);
    this.error.set(null);

    // Load presentations for this product
    this.productUnitPriceService.getByProductName(product.name).subscribe({
      next: (presentations) => {
        this.productPresentations.set(presentations);
      },
      error: (err) => {
        this.error.set('Error al cargar presentaciones');
        console.error(err);
      }
    });
  }

  selectPresentation(presentation: ProductUnitPriceDto) {
    this.selectedPresentation.set(presentation);
    this.quantity = 1;
  }

  cancelSelection() {
    this.selectedProduct.set(null);
    this.productPresentations.set([]);
  }

  cancelPresentation() {
    this.selectedPresentation.set(null);
    this.quantity = 1;
  }

  addToCart() {
    const product = this.selectedProduct();
    const presentation = this.selectedPresentation();

    if (!product || !presentation || this.quantity <= 0) {
      return;
    }

    const subtotal = presentation.price * this.quantity;

    const cartItem: CartItem = {
      product,
      unitPrice: presentation,
      quantity: this.quantity,
      subtotal
    };

    this.cart.update(items => [...items, cartItem]);

    // Reset selection
    this.selectedProduct.set(null);
    this.selectedPresentation.set(null);
    this.productPresentations.set([]);
    this.quantity = 1;
  }

  removeFromCart(index: number) {
    this.cart.update(items => items.filter((_, i) => i !== index));
  }

  clearCart() {
    this.cart.set([]);
    this.selectedProduct.set(null);
    this.selectedPresentation.set(null);
  }

  async finalizeSale() {
    const session = this.currentSession();
    if (!session || this.cart().length === 0) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);

    try {
      const totalsCalc = this.totals();

      // Create sale
      const sale: SaleDto = {
        soldAt: new Date().toISOString(),
        status: 'Draft',
        subtotal: totalsCalc.subtotal,
        taxTotal: totalsCalc.tax,
        grandTotal: totalsCalc.grandTotal,
        cashSessionId: session.id!
      };

      this.saleService.create(sale).subscribe({
        next: async (createdSale) => {
          // Add sale details
          const detailPromises = this.cart().map(item => {
            const detail: SaleProductDetailDto = {
              saleId: createdSale.id!,
              productId: item.product.id!,
              quantity: item.quantity,
              unitPrice: item.unitPrice.price,
              subtotal: item.subtotal
            };
            return this.saleDetailService.create(detail).toPromise();
          });

          await Promise.all(detailPromises);

          // Finalize sale
          this.saleService.finalizeSale(createdSale.id!).subscribe({
            next: () => {
              this.success.set(`Venta #${createdSale.id} finalizada exitosamente`);
              this.clearCart();
              this.loading.set(false);

              setTimeout(() => {
                this.success.set(null);
              }, 3000);
            },
            error: (err) => {
              this.error.set('Error al finalizar venta: ' + (err.error?.message || err.message));
              this.loading.set(false);
            }
          });
        },
        error: (err) => {
          this.error.set('Error al crear venta: ' + (err.error?.message || err.message));
          this.loading.set(false);
        }
      });
    } catch (err: any) {
      this.error.set('Error inesperado: ' + err.message);
      this.loading.set(false);
    }
  }

  goToSessions() {
    this.router.navigate(['/cash-sessions']);
  }
}
