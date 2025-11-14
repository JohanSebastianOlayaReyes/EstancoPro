import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomerService } from '../../core/services/customer.service';
import { Customer, DebtTransaction } from '../../core/models';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent],
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.scss']
})
export class CustomersComponent implements OnInit {
  customers: Customer[] = [];
  filteredCustomers: Customer[] = [];

  // Filters
  searchTerm = '';
  selectedStatus = 'all';
  debtFilter = 'all'; // all, with-debt, no-debt

  // Loading states
  loading = true;
  processingAction = false;

  // Customer Modal
  showCustomerModal = false;
  editingCustomer: Customer | null = null;
  customerForm: Partial<Customer> = this.getEmptyCustomerForm();

  // Debt Modal
  showDebtModal = false;
  selectedCustomer: Customer | null = null;
  debtAmount: number = 0;
  debtDescription: string = '';

  // Payment Modal
  showPaymentModal = false;
  paymentAmount: number = 0;
  paymentDescription: string = '';

  // Transactions Modal
  showTransactionsModal = false;
  customerTransactions: DebtTransaction[] = [];
  loadingTransactions = false;

  // Delete Modal
  showDeleteModal = false;
  customerToDelete: Customer | null = null;

  // Success Modal
  showSuccessModal = false;
  successMessage = '';

  constructor(
    private router: Router,
    private customerService: CustomerService
  ) {}

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.loading = true;
    this.customerService.getAll().subscribe({
      next: (customers) => {
        this.customers = customers;
        this.filterCustomers();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading customers:', error);
        this.loading = false;
      }
    });
  }

  filterCustomers(): void {
    let filtered = [...this.customers];

    // Search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(c =>
        c.name.toLowerCase().includes(term) ||
        c.phone.toLowerCase().includes(term) ||
        c.email?.toLowerCase().includes(term)
      );
    }

    // Status filter
    switch (this.selectedStatus) {
      case 'active':
        filtered = filtered.filter(c => c.active);
        break;
      case 'inactive':
        filtered = filtered.filter(c => !c.active);
        break;
    }

    // Debt filter
    switch (this.debtFilter) {
      case 'with-debt':
        filtered = filtered.filter(c => c.balance > 0);
        break;
      case 'no-debt':
        filtered = filtered.filter(c => c.balance === 0);
        break;
    }

    this.filteredCustomers = filtered;
  }

  getTotalDebt(): number {
    return this.customers.reduce((sum, c) => sum + c.balance, 0);
  }

  getCustomersWithDebt(): number {
    return this.customers.filter(c => c.balance > 0).length;
  }

  openAddCustomerModal(): void {
    this.editingCustomer = null;
    this.customerForm = this.getEmptyCustomerForm();
    this.showCustomerModal = true;
  }

  editCustomer(customer: Customer): void {
    this.editingCustomer = customer;
    this.customerForm = { ...customer };
    this.showCustomerModal = true;
  }

  closeCustomerModal(): void {
    this.showCustomerModal = false;
    this.editingCustomer = null;
    this.customerForm = this.getEmptyCustomerForm();
  }

  saveCustomer(): void {
    // Validation
    if (!this.customerForm.name || !this.customerForm.phone) {
      alert('Por favor completa todos los campos obligatorios');
      return;
    }

    // Email validation if provided
    if (this.customerForm.email) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(this.customerForm.email)) {
        alert('Por favor ingresa un correo electrónico válido');
        return;
      }
    }

    this.processingAction = true;

    const customerData: Partial<Customer> = {
      name: this.customerForm.name,
      phone: this.customerForm.phone,
      email: this.customerForm.email || undefined,
      address: this.customerForm.address || undefined,
      active: this.customerForm.active !== undefined ? this.customerForm.active : true
    };

    if (this.editingCustomer) {
      this.customerService.update(this.editingCustomer.id, customerData).subscribe({
        next: () => {
          this.processingAction = false;
          this.showCustomerModal = false;
          this.successMessage = 'Cliente actualizado exitosamente';
          this.showSuccessModal = true;
        },
        error: (error: any) => {
          console.error('Error saving customer:', error);
          alert('Error al guardar cliente');
          this.processingAction = false;
        }
      });
    } else {
      this.customerService.create(customerData).subscribe({
        next: () => {
          this.processingAction = false;
          this.showCustomerModal = false;
          this.successMessage = 'Cliente creado exitosamente';
          this.showSuccessModal = true;
        },
        error: (error: any) => {
          console.error('Error saving customer:', error);
          alert('Error al guardar cliente');
          this.processingAction = false;
        }
      });
    }
  }

  openDebtModal(customer: Customer): void {
    this.selectedCustomer = customer;
    this.debtAmount = 0;
    this.debtDescription = '';
    this.showDebtModal = true;
  }

  addDebt(): void {
    if (!this.selectedCustomer || this.debtAmount <= 0) {
      alert('Por favor ingresa un monto válido');
      return;
    }

    if (!this.debtDescription.trim()) {
      alert('Por favor ingresa una descripción');
      return;
    }

    this.processingAction = true;

    this.customerService.addDebt(this.selectedCustomer.id, this.debtAmount, this.debtDescription).subscribe({
      next: () => {
        this.processingAction = false;
        this.showDebtModal = false;
        this.successMessage = `Se registró una deuda de $${this.debtAmount.toLocaleString()}`;
        this.showSuccessModal = true;
        this.selectedCustomer = null;
      },
      error: (error) => {
        console.error('Error adding debt:', error);
        alert('Error al registrar deuda');
        this.processingAction = false;
      }
    });
  }

  openPaymentModal(customer: Customer): void {
    this.selectedCustomer = customer;
    this.paymentAmount = 0;
    this.paymentDescription = '';
    this.showPaymentModal = true;
  }

  addPayment(): void {
    if (!this.selectedCustomer || this.paymentAmount <= 0) {
      alert('Por favor ingresa un monto válido');
      return;
    }

    if (this.paymentAmount > this.selectedCustomer.balance) {
      alert('El pago no puede ser mayor a la deuda actual');
      return;
    }

    if (!this.paymentDescription.trim()) {
      alert('Por favor ingresa una descripción');
      return;
    }

    this.processingAction = true;

    this.customerService.addPayment(this.selectedCustomer.id, this.paymentAmount, this.paymentDescription).subscribe({
      next: () => {
        this.processingAction = false;
        this.showPaymentModal = false;
        this.successMessage = `Se registró un pago de $${this.paymentAmount.toLocaleString()}`;
        this.showSuccessModal = true;
        this.selectedCustomer = null;
      },
      error: (error) => {
        console.error('Error adding payment:', error);
        alert('Error al registrar pago');
        this.processingAction = false;
      }
    });
  }

  viewTransactions(customer: Customer): void {
    this.selectedCustomer = customer;
    this.loadingTransactions = true;
    this.showTransactionsModal = true;

    this.customerService.getCustomerTransactions(customer.id).subscribe({
      next: (transactions) => {
        this.customerTransactions = transactions;
        this.loadingTransactions = false;
      },
      error: (error) => {
        console.error('Error loading transactions:', error);
        this.loadingTransactions = false;
      }
    });
  }

  confirmDelete(customer: Customer): void {
    this.customerToDelete = customer;
    this.showDeleteModal = true;
  }

  deleteCustomer(): void {
    if (!this.customerToDelete) return;

    this.processingAction = true;

    this.customerService.delete(this.customerToDelete.id).subscribe({
      next: () => {
        this.processingAction = false;
        this.showDeleteModal = false;
        this.successMessage = 'Cliente desactivado exitosamente';
        this.showSuccessModal = true;
        this.customerToDelete = null;
      },
      error: (error) => {
        console.error('Error deleting customer:', error);
        alert('Error al desactivar cliente');
        this.processingAction = false;
      }
    });
  }

  getEmptyCustomerForm(): Partial<Customer> {
    return {
      name: '',
      phone: '',
      email: '',
      address: '',
      active: true
    };
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}
