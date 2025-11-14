import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { Customer, DebtTransaction } from '../models';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  // Mock data de clientes
  private mockCustomers: Customer[] = [
    {
      id: 1,
      name: 'María García',
      phone: '+57 301 234 5678',
      email: 'maria.garcia@email.com',
      address: 'Calle 15 # 20-30, Bogotá',
      balance: 150000, // Debe 150,000
      active: true,
      createdAt: '2024-01-10T10:00:00'
    },
    {
      id: 2,
      name: 'Carlos Rodríguez',
      phone: '+57 312 987 6543',
      email: 'carlos.rodriguez@email.com',
      address: 'Carrera 10 # 5-15, Medellín',
      balance: 0, // No debe nada
      active: true,
      createdAt: '2024-01-12T14:30:00'
    },
    {
      id: 3,
      name: 'Ana Martínez',
      phone: '+57 320 555 1234',
      address: 'Avenida 3 # 12-45, Cali',
      balance: 75000, // Debe 75,000
      active: true,
      createdAt: '2024-01-15T09:15:00'
    },
    {
      id: 4,
      name: 'Luis Pérez',
      phone: '+57 315 777 8888',
      email: 'luis.perez@email.com',
      address: 'Calle 50 # 25-10, Barranquilla',
      balance: 250000, // Debe 250,000
      active: true,
      createdAt: '2024-01-18T16:20:00'
    },
    {
      id: 5,
      name: 'Sandra López',
      phone: '+57 318 444 9999',
      address: 'Carrera 20 # 8-12, Cartagena',
      balance: 50000, // Debe 50,000
      active: true,
      createdAt: '2024-01-20T11:45:00'
    }
  ];

  // Mock data de transacciones de deuda
  private mockTransactions: DebtTransaction[] = [
    {
      id: 1,
      customerId: 1,
      amount: 100000,
      type: 'debt',
      description: 'Compra de mercancía',
      date: '2024-01-10T10:30:00',
      customerName: 'María García'
    },
    {
      id: 2,
      customerId: 1,
      amount: 50000,
      type: 'debt',
      description: 'Compra adicional',
      date: '2024-01-15T14:20:00',
      customerName: 'María García'
    },
    {
      id: 3,
      customerId: 3,
      amount: 75000,
      type: 'debt',
      description: 'Compra a crédito',
      date: '2024-01-16T09:00:00',
      customerName: 'Ana Martínez'
    },
    {
      id: 4,
      customerId: 4,
      amount: 300000,
      type: 'debt',
      description: 'Compra grande',
      date: '2024-01-18T16:30:00',
      customerName: 'Luis Pérez'
    },
    {
      id: 5,
      customerId: 4,
      amount: 50000,
      type: 'payment',
      description: 'Abono parcial',
      date: '2024-01-22T10:00:00',
      customerName: 'Luis Pérez'
    }
  ];

  constructor() {}

  // Obtener todos los clientes
  getAll(): Observable<Customer[]> {
    return of(this.mockCustomers).pipe(delay(500));
  }

  // Obtener cliente por ID
  getById(id: number): Observable<Customer | undefined> {
    const customer = this.mockCustomers.find(c => c.id === id);
    return of(customer).pipe(delay(300));
  }

  // Crear nuevo cliente
  create(customer: Partial<Customer>): Observable<Customer> {
    const newCustomer: Customer = {
      id: this.mockCustomers.length + 1,
      name: customer.name || '',
      phone: customer.phone || '',
      email: customer.email,
      address: customer.address,
      balance: 0, // Los clientes nuevos empiezan sin deuda
      active: true,
      createdAt: new Date().toISOString()
    };

    this.mockCustomers.unshift(newCustomer);
    return of(newCustomer).pipe(delay(500));
  }

  // Actualizar cliente
  update(id: number, customer: Partial<Customer>): Observable<void> {
    const index = this.mockCustomers.findIndex(c => c.id === id);
    if (index !== -1) {
      this.mockCustomers[index] = { ...this.mockCustomers[index], ...customer };
    }
    return of(undefined).pipe(delay(500));
  }

  // Desactivar cliente
  delete(id: number): Observable<void> {
    const customer = this.mockCustomers.find(c => c.id === id);
    if (customer) {
      customer.active = false;
    }
    return of(undefined).pipe(delay(500));
  }

  // Registrar deuda
  addDebt(customerId: number, amount: number, description: string): Observable<void> {
    const customer = this.mockCustomers.find(c => c.id === customerId);
    if (customer) {
      customer.balance += amount;

      const newTransaction: DebtTransaction = {
        id: this.mockTransactions.length + 1,
        customerId,
        amount,
        type: 'debt',
        description,
        date: new Date().toISOString(),
        customerName: customer.name
      };

      this.mockTransactions.unshift(newTransaction);
    }
    return of(undefined).pipe(delay(500));
  }

  // Registrar pago
  addPayment(customerId: number, amount: number, description: string): Observable<void> {
    const customer = this.mockCustomers.find(c => c.id === customerId);
    if (customer) {
      customer.balance = Math.max(0, customer.balance - amount);

      const newTransaction: DebtTransaction = {
        id: this.mockTransactions.length + 1,
        customerId,
        amount,
        type: 'payment',
        description,
        date: new Date().toISOString(),
        customerName: customer.name
      };

      this.mockTransactions.unshift(newTransaction);
    }
    return of(undefined).pipe(delay(500));
  }

  // Obtener transacciones de un cliente
  getCustomerTransactions(customerId: number): Observable<DebtTransaction[]> {
    const transactions = this.mockTransactions.filter(t => t.customerId === customerId);
    return of(transactions).pipe(delay(300));
  }

  // Obtener todas las transacciones
  getAllTransactions(): Observable<DebtTransaction[]> {
    return of(this.mockTransactions).pipe(delay(300));
  }
}
