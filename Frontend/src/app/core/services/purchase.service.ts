import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { Purchase, PurchaseProductDetail, Supplier } from '../models';

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {

  // Mock data de proveedores
  private mockSuppliers: Supplier[] = [
    {
      id: 1,
      name: 'Distribuidora Nacional S.A.',
      contactName: 'Carlos Méndez',
      phone: '+57 300 123 4567',
      email: 'carlos@distribuidora.com',
      address: 'Calle 45 # 23-15, Bogotá',
      active: true
    },
    {
      id: 2,
      name: 'Almacenes El Mayorista',
      contactName: 'María González',
      phone: '+57 310 987 6543',
      email: 'maria@mayorista.com',
      address: 'Carrera 30 # 50-20, Medellín',
      active: true
    },
    {
      id: 3,
      name: 'Comercializadora del Caribe',
      contactName: 'Luis Rodríguez',
      phone: '+57 315 555 7890',
      email: 'luis@caribe.com',
      address: 'Avenida 5 # 12-34, Barranquilla',
      active: true
    }
  ];

  // Mock data de compras
  private mockPurchases: Purchase[] = [
    {
      id: 1,
      invoiceNumber: 'FC-001-2024',
      orderedAt: '2024-01-15T10:30:00',
      receivedAt: '2024-01-17T14:20:00',
      status: 'Received',
      subtotal: 850000,
      taxTotal: 161500,
      grandTotal: 1011500,
      supplierId: 1,
      active: true,
      supplier: this.mockSuppliers[0],
      purchaseProductDetails: [
        {
          id: 1,
          purchaseId: 1,
          productId: 1,
          quantity: 50,
          unitCost: 12000,
          subtotal: 600000,
          taxAmount: 114000,
          active: true,
          productName: 'Coca Cola 350ml'
        },
        {
          id: 2,
          purchaseId: 1,
          productId: 2,
          quantity: 25,
          unitCost: 10000,
          subtotal: 250000,
          taxAmount: 47500,
          active: true,
          productName: 'Papas Margarita Grande'
        }
      ]
    },
    {
      id: 2,
      invoiceNumber: 'FC-002-2024',
      orderedAt: '2024-01-20T09:15:00',
      status: 'Ordered',
      subtotal: 1200000,
      taxTotal: 228000,
      grandTotal: 1428000,
      supplierId: 2,
      active: true,
      supplier: this.mockSuppliers[1],
      purchaseProductDetails: [
        {
          id: 3,
          purchaseId: 2,
          productId: 3,
          quantity: 100,
          unitCost: 12000,
          subtotal: 1200000,
          taxAmount: 228000,
          active: true,
          productName: 'Marlboro Rojo'
        }
      ]
    },
    {
      id: 3,
      invoiceNumber: 'FC-003-2024',
      orderedAt: '2024-01-22T11:00:00',
      receivedAt: '2024-01-24T16:30:00',
      status: 'Received',
      subtotal: 650000,
      taxTotal: 123500,
      grandTotal: 773500,
      supplierId: 3,
      active: true,
      supplier: this.mockSuppliers[2],
      purchaseProductDetails: [
        {
          id: 4,
          purchaseId: 3,
          productId: 4,
          quantity: 30,
          unitCost: 15000,
          subtotal: 450000,
          taxAmount: 85500,
          active: true,
          productName: 'Agua Brisa 600ml'
        },
        {
          id: 5,
          purchaseId: 3,
          productId: 5,
          quantity: 20,
          unitCost: 10000,
          subtotal: 200000,
          taxAmount: 38000,
          active: true,
          productName: 'Chocolatina Jet'
        }
      ]
    },
    {
      id: 4,
      invoiceNumber: 'FC-004-2024',
      orderedAt: '2024-01-25T08:45:00',
      status: 'Cancelled',
      subtotal: 300000,
      taxTotal: 57000,
      grandTotal: 357000,
      supplierId: 1,
      active: false,
      supplier: this.mockSuppliers[0],
      purchaseProductDetails: []
    }
  ];

  constructor() {}

  // Obtener todas las compras
  getAll(): Observable<Purchase[]> {
    return of(this.mockPurchases).pipe(delay(500));
  }

  // Obtener compra por ID
  getById(id: number): Observable<Purchase | undefined> {
    const purchase = this.mockPurchases.find(p => p.id === id);
    return of(purchase).pipe(delay(300));
  }

  // Crear nueva compra
  create(purchase: Partial<Purchase>): Observable<Purchase> {
    const newPurchase: Purchase = {
      id: this.mockPurchases.length + 1,
      invoiceNumber: purchase.invoiceNumber || `FC-${String(this.mockPurchases.length + 1).padStart(3, '0')}-2024`,
      orderedAt: new Date().toISOString(),
      status: 'Ordered',
      subtotal: purchase.subtotal || 0,
      taxTotal: purchase.taxTotal || 0,
      grandTotal: purchase.grandTotal || 0,
      supplierId: purchase.supplierId || 0,
      active: true,
      supplier: this.mockSuppliers.find(s => s.id === purchase.supplierId),
      purchaseProductDetails: purchase.purchaseProductDetails || []
    };

    this.mockPurchases.unshift(newPurchase);
    return of(newPurchase).pipe(delay(500));
  }

  // Recibir compra
  receive(id: number): Observable<void> {
    const purchase = this.mockPurchases.find(p => p.id === id);
    if (purchase) {
      purchase.status = 'Received';
      purchase.receivedAt = new Date().toISOString();
    }
    return of(undefined).pipe(delay(500));
  }

  // Cancelar compra
  cancel(id: number): Observable<void> {
    const purchase = this.mockPurchases.find(p => p.id === id);
    if (purchase) {
      purchase.status = 'Cancelled';
      purchase.active = false;
    }
    return of(undefined).pipe(delay(500));
  }

  // Obtener todos los proveedores
  getSuppliers(): Observable<Supplier[]> {
    return of(this.mockSuppliers).pipe(delay(300));
  }

  // Crear proveedor
  createSupplier(supplier: Partial<Supplier>): Observable<Supplier> {
    const newSupplier: Supplier = {
      id: this.mockSuppliers.length + 1,
      name: supplier.name || '',
      contactName: supplier.contactName || '',
      phone: supplier.phone || '',
      email: supplier.email || '',
      address: supplier.address || '',
      active: true
    };

    this.mockSuppliers.push(newSupplier);
    return of(newSupplier).pipe(delay(500));
  }
}
