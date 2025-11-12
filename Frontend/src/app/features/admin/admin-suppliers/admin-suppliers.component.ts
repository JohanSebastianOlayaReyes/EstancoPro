import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SupplierService } from '../../../core/services/supplier.service';
import { SidebarMenuComponent } from '../../../shared/components/sidebar-menu.component';
import { ADMIN_MENU_SECTIONS } from '../../../shared/config/menu-sections';
import { SupplierDto } from '../../../core/models/supplier.model';

@Component({
  selector: 'app-admin-suppliers',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarMenuComponent],
  templateUrl: './admin-suppliers.component.html',
  styleUrls: ['./admin-suppliers.component.css']
})
export class AdminSuppliersComponent implements OnInit {
  private supplierService = inject(SupplierService);

  menuSections = ADMIN_MENU_SECTIONS;

  suppliers = signal<SupplierDto[]>([]);
  isModalOpen = signal(false);
  isEditMode = signal(false);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  currentSupplier = signal<SupplierDto>({
    name: '',
    phone: '',
    active: true
  });

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.isLoading.set(true);
    this.supplierService.getAll().subscribe({
      next: (data) => {
        this.suppliers.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.showError('Error al cargar los proveedores');
        this.isLoading.set(false);
        console.error('Error loading suppliers:', error);
      }
    });
  }

  openCreateModal(): void {
    this.currentSupplier.set({
      name: '',
      phone: '',
      active: true
    });
    this.isEditMode.set(false);
    this.isModalOpen.set(true);
  }

  openEditModal(supplier: SupplierDto): void {
    this.currentSupplier.set({ ...supplier });
    this.isEditMode.set(true);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.errorMessage.set(null);
  }

  saveSupplier(): void {
    const supplier = this.currentSupplier();

    if (!this.validateSupplier(supplier)) {
      return;
    }

    this.isLoading.set(true);

    if (this.isEditMode()) {
      this.supplierService.update(supplier).subscribe({
        next: () => {
          this.showSuccess('Proveedor actualizado exitosamente');
          this.loadSuppliers();
          this.closeModal();
        },
        error: (error) => {
          this.showError('Error al actualizar el proveedor');
          console.error('Error updating supplier:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    } else {
      this.supplierService.create(supplier).subscribe({
        next: () => {
          this.showSuccess('Proveedor creado exitosamente');
          this.loadSuppliers();
          this.closeModal();
        },
        error: (error) => {
          this.showError('Error al crear el proveedor');
          console.error('Error creating supplier:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  deleteSupplier(id: number | undefined): void {
    if (!id) return;

    if (confirm('¿Está seguro de que desea eliminar este proveedor?')) {
      this.isLoading.set(true);
      this.supplierService.delete(id).subscribe({
        next: () => {
          this.showSuccess('Proveedor eliminado exitosamente');
          this.loadSuppliers();
        },
        error: (error) => {
          this.showError('Error al eliminar el proveedor');
          console.error('Error deleting supplier:', error);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  validateSupplier(supplier: SupplierDto): boolean {
    if (!supplier.name || supplier.name.trim() === '') {
      this.showError('El nombre del proveedor es requerido');
      return false;
    }

    if (!supplier.phone || supplier.phone.trim() === '') {
      this.showError('El teléfono del proveedor es requerido');
      return false;
    }

    return true;
  }

  showSuccess(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(null), 3000);
  }

  showError(message: string): void {
    this.errorMessage.set(message);
  }

  updateSupplierField<K extends keyof SupplierDto>(field: K, value: SupplierDto[K]): void {
    this.currentSupplier.update(supplier => ({
      ...supplier,
      [field]: value
    }));
  }
}
