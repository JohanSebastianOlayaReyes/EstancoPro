import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PurchaseService } from '../../core/services/purchase.service';
import { Supplier } from '../../core/models';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Component({
  selector: 'app-suppliers',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent],
  templateUrl: './suppliers.component.html',
  styleUrls: ['./suppliers.component.scss']
})
export class SuppliersComponent implements OnInit {
  suppliers: Supplier[] = [];
  filteredSuppliers: Supplier[] = [];

  // Filters
  searchTerm = '';
  selectedStatus = 'all';

  // Loading states
  loading = true;
  processingAction = false;

  // Supplier Modal
  showSupplierModal = false;
  editingSupplier: Supplier | null = null;
  supplierForm: Partial<Supplier> = this.getEmptySupplierForm();

  // Delete Modal
  showDeleteModal = false;
  supplierToDelete: Supplier | null = null;

  // Success Modal
  showSuccessModal = false;
  successMessage = '';

  constructor(
    private router: Router,
    private purchaseService: PurchaseService
  ) {}

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.loading = true;
    this.purchaseService.getSuppliers().subscribe({
      next: (suppliers) => {
        this.suppliers = suppliers;
        this.filterSuppliers();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading suppliers:', error);
        this.loading = false;
      }
    });
  }

  filterSuppliers(): void {
    let filtered = [...this.suppliers];

    // Search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(s =>
        s.name.toLowerCase().includes(term) ||
        s.contactName.toLowerCase().includes(term) ||
        s.email.toLowerCase().includes(term) ||
        s.phone.toLowerCase().includes(term)
      );
    }

    // Status filter
    switch (this.selectedStatus) {
      case 'active':
        filtered = filtered.filter(s => s.active);
        break;
      case 'inactive':
        filtered = filtered.filter(s => !s.active);
        break;
    }

    this.filteredSuppliers = filtered;
  }

  openAddSupplierModal(): void {
    this.editingSupplier = null;
    this.supplierForm = this.getEmptySupplierForm();
    this.showSupplierModal = true;
  }

  editSupplier(supplier: Supplier): void {
    this.editingSupplier = supplier;
    this.supplierForm = { ...supplier };
    this.showSupplierModal = true;
  }

  closeSupplierModal(): void {
    this.showSupplierModal = false;
    this.editingSupplier = null;
    this.supplierForm = this.getEmptySupplierForm();
  }

  saveSupplier(): void {
    // Validation
    if (!this.supplierForm.name || !this.supplierForm.contactName ||
        !this.supplierForm.phone || !this.supplierForm.email) {
      alert('Por favor completa todos los campos obligatorios');
      return;
    }

    // Email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.supplierForm.email)) {
      alert('Por favor ingresa un correo electrónico válido');
      return;
    }

    this.processingAction = true;

    const supplierData: Partial<Supplier> = {
      name: this.supplierForm.name,
      contactName: this.supplierForm.contactName,
      phone: this.supplierForm.phone,
      email: this.supplierForm.email,
      address: this.supplierForm.address || '',
      active: this.supplierForm.active !== undefined ? this.supplierForm.active : true
    };

    if (this.editingSupplier) {
      // Para mock, simulamos la actualización
      const index = this.suppliers.findIndex(s => s.id === this.editingSupplier!.id);
      if (index !== -1) {
        this.suppliers[index] = { ...this.suppliers[index], ...supplierData };
      }
      this.processingAction = false;
      this.showSupplierModal = false;
      this.successMessage = 'Proveedor actualizado exitosamente';
      this.showSuccessModal = true;
      this.filterSuppliers();
    } else {
      this.purchaseService.createSupplier(supplierData).subscribe({
        next: () => {
          this.processingAction = false;
          this.showSupplierModal = false;
          this.successMessage = 'Proveedor creado exitosamente';
          this.showSuccessModal = true;
        },
        error: (error: any) => {
          console.error('Error saving supplier:', error);
          alert('Error al guardar proveedor');
          this.processingAction = false;
        }
      });
    }
  }

  confirmDelete(supplier: Supplier): void {
    this.supplierToDelete = supplier;
    this.showDeleteModal = true;
  }

  deleteSupplier(): void {
    if (!this.supplierToDelete) return;

    this.processingAction = true;

    // Para mock, simulamos la eliminación
    const index = this.suppliers.findIndex(s => s.id === this.supplierToDelete!.id);
    if (index !== -1) {
      this.suppliers[index].active = false;
    }

    setTimeout(() => {
      this.processingAction = false;
      this.showDeleteModal = false;
      this.successMessage = 'Proveedor desactivado exitosamente';
      this.showSuccessModal = true;
      this.supplierToDelete = null;
      this.filterSuppliers();
    }, 500);
  }

  getEmptySupplierForm(): Partial<Supplier> {
    return {
      name: '',
      contactName: '',
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
