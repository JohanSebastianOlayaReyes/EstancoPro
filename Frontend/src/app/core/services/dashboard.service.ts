import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, map } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface DashboardStats {
  totalSalesToday: number;
  salesCountToday: number;
  totalProfit: number;
  totalProducts: number;
  lowStockProducts: number;
  totalExpenses: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * Obtiene todas las estadísticas del dashboard
   */
  getDashboardStats(): Observable<DashboardStats> {
    const today = new Date().toISOString().split('T')[0];

    return forkJoin({
      sales: this.http.get<any[]>(`${this.API_URL}/Sale`),
      products: this.http.get<any[]>(`${this.API_URL}/Product`),
      expenses: this.http.get<any[]>(`${this.API_URL}/Expense`)
    }).pipe(
      map(({ sales, products, expenses }) => {
        // Filtrar ventas del día
        const salesToday = sales.filter(sale => {
          const saleDate = new Date(sale.saleDate).toISOString().split('T')[0];
          return saleDate === today && sale.status === 'Finalizada';
        });

        // Calcular totales
        const totalSalesToday = salesToday.reduce((sum, sale) => sum + (sale.totalAmount || 0), 0);
        const salesCountToday = salesToday.length;

        // Calcular utilidad (total - costo)
        const totalProfit = salesToday.reduce((sum, sale) => {
          const costAmount = sale.totalAmount - (sale.totalAmount * 0.3); // Estimado 30% margen
          return sum + (sale.totalAmount - costAmount);
        }, 0);

        // Contar productos con stock bajo
        const lowStockProducts = products.filter(p => p.stockQuantity <= (p.minStock || 10)).length;

        // Sumar gastos del día
        const expensesToday = expenses.filter(expense => {
          const expenseDate = new Date(expense.expenseDate).toISOString().split('T')[0];
          return expenseDate === today;
        });
        const totalExpenses = expensesToday.reduce((sum, expense) => sum + (expense.amount || 0), 0);

        return {
          totalSalesToday,
          salesCountToday,
          totalProfit,
          totalProducts: products.length,
          lowStockProducts,
          totalExpenses
        };
      })
    );
  }
}
