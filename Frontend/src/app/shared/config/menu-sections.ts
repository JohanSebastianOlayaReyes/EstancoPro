import { MenuSection } from '../components/sidebar-menu.component';

export const ADMIN_MENU_SECTIONS: MenuSection[] = [
  {
    title: 'Sistema',
    items: [
      { title: 'Gestión de Usuarios', icon: 'users', route: '/admin/users' },
      { title: 'Roles y Permisos', icon: 'lock', route: '/admin/roles' }
    ]
  },
  {
    title: 'Inventario',
    items: [
      { title: 'Productos', icon: 'package', route: '/admin/products' },
      { title: 'Categorías', icon: 'tag', route: '/admin/categories' },
      { title: 'Unidades de Medida', icon: 'scale', route: '/admin/unit-measures' },
      { title: 'Precios por Unidad', icon: 'currency-dollar', route: '/admin/product-prices' }
    ]
  },
  {
    title: 'Operaciones',
    items: [
      { title: 'Proveedores', icon: 'building-store', route: '/admin/suppliers' },
      { title: 'Compras', icon: 'shopping-cart', route: '/admin/purchases' },
      { title: 'Ventas', icon: 'coin', route: '/admin/sales' },
      { title: 'Caja', icon: 'currency-dollar', route: '/admin/cash' }
    ]
  },
  {
    title: 'Reportes',
    items: [
      { title: 'Reportes de Ventas', icon: 'chart-bar', route: '/admin/reports/sales' },
      { title: 'Inventario', icon: 'chart-line', route: '/admin/reports/inventory' },
      { title: 'Flujo de Caja', icon: 'trending-up', route: '/admin/reports/cash-flow' }
    ]
  }
];
