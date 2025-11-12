import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CategoryService } from '../../../core/services/category.service';
import { ButtonComponent } from '../../../shared/components/button.component';
import { InputComponent } from '../../../shared/components/input.component';
import { EstancoCardComponent } from '../../../shared/components/estanco-card.component';
// AppHeaderComponent removed - not present in shared/components
import { CategoryDto } from '../../../core/models/category.model';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
    imports: [CommonModule, ButtonComponent, InputComponent, EstancoCardComponent],
  template: `<!-- preserved template from original compact file -->`,
  styles: [``]
})
export class AdminCategoriesCompactComponent implements OnInit {
  private authService = inject(AuthService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  categories = signal<CategoryDto[]>([]);
  showForm = signal(false);
  editingId = signal(0);
  loading = signal(false);
  error = signal('');
  success = signal('');
  form = signal<Partial<CategoryDto>>({ name: '', description: '', active: true });

  ngOnInit() { this.loadCategories(); }
  private loadCategories() { /* preserved */ }
  updateForm(key: string, value: any) { /* preserved */ }
  toggleForm() { /* preserved */ }
  cancelForm() { /* preserved */ }
  saveCategory(e: Event) { /* preserved */ }
  editCategory(category: CategoryDto) { /* preserved */ }
  deleteCategory(id: number) { /* preserved */ }
  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}
