# Estructura de Archivos - Dashboards por Rol

## Árbol de Directorios

```
Frontend/src/app/
│
├── core/
│   ├── models/
│   │   ├── cash.model.ts (ya existía)
│   │   ├── sale.model.ts (ya existía)
│   │   └── purchase.model.ts (ya existía)
│   │
│   └── services/
│       ├── auth.service.ts (ya existía)
│       ├── sale.service.ts (MODIFICADO - nuevos métodos)
│       ├── cash-session.service.ts (ya existía)
│       ├── purchase.service.ts (ya existía)
│       ├── user.service.ts (ya existía)
│       └── product.service.ts (ya existía)
│
├── features/
│   ├── admin/
│   │   └── dashboard/
│   │       ├── admin-dashboard.component.ts (ya existía)
│   │       ├── admin-dashboard.component.html (ya existía)
│   │       └── admin-dashboard.component.scss (ya existía)
│   │
│   ├── cajero/ (NUEVO)
│   │   ├── cajero-dashboard.component.ts
│   │   ├── cajero-dashboard.component.html
│   │   └── cajero-dashboard.component.scss
│   │
│   ├── vendedor/ (NUEVO)
│   │   ├── vendedor-dashboard.component.ts
│   │   ├── vendedor-dashboard.component.html
│   │   └── vendedor-dashboard.component.scss
│   │
│   ├── supervisor/ (NUEVO)
│   │   ├── supervisor-dashboard.component.ts
│   │   ├── supervisor-dashboard.component.html
│   │   └── supervisor-dashboard.component.scss
│   │
│   └── dashboard/
│       └── dashboard.component.ts (MODIFICADO - router por roles)
│
├── shared/
│   └── components/
│       ├── stat-card.component.ts (NUEVO)
│       ├── chart-card.component.ts (NUEVO)
│       ├── estanco-card.component.ts (ya existía)
│       ├── button.component.ts (ya existía)
│       ├── icon.component.ts (ya existía)
│       └── app-modal.component.ts (ya existía)
│
└── app.routes.ts (MODIFICADO - nuevas rutas)
```

---

## Mapa de Rutas

```
/login
  └─> LoginComponent

/dashboard
  └─> DashboardComponent (Router)
       │
       ├─> Administrador → /admin
       │    └─> AdminDashboardComponent
       │
       ├─> Cajero → /cajero
       │    └─> CajeroDashboardComponent
       │
       ├─> Vendedor → /vendedor
       │    └─> VendedorDashboardComponent
       │
       └─> Supervisor → /supervisor
            └─> SupervisorDashboardComponent

/pos
  └─> PosComponent (Cajero, Vendedor)

/sales
  └─> SalesListComponent (Todos)

/purchases
  └─> PurchasesComponent (Administrador)

/cash-sessions
  └─> CashSessionsComponent (Administrador, Cajero)

/admin/*
  └─> Admin routes (Administrador)
```

---

## Dependencias de Componentes

### AdminDashboardComponent
```
AdminDashboardComponent
├── SidebarMenuComponent
├── EstancoCardComponent
├── IconComponent
└── Services:
    ├── UserService
    ├── ProductService
    ├── SupplierService
    └── SaleService
```

### CajeroDashboardComponent
```
CajeroDashboardComponent
├── StatCardComponent
├── EstancoCardComponent
├── ButtonComponent
├── IconComponent
├── AppModalComponent
└── Services:
    ├── AuthService
    ├── CashSessionService
    └── SaleService
```

### VendedorDashboardComponent
```
VendedorDashboardComponent
├── StatCardComponent
├── EstancoCardComponent
├── ButtonComponent
├── IconComponent
└── Services:
    ├── AuthService
    ├── SaleService
    └── ProductService
```

### SupervisorDashboardComponent
```
SupervisorDashboardComponent
├── StatCardComponent
├── EstancoCardComponent
├── ChartCardComponent
├── IconComponent
└── Services:
    ├── AuthService
    ├── SaleService
    ├── CashSessionService
    ├── UserService
    └── ProductService
```

---

## Flujo de Datos

### Login → Dashboard
```
1. LoginComponent
   └─> AuthService.login()
       └─> Guarda token y user en localStorage
           └─> Actualiza signals: isAuthenticated, currentUser

2. Router navega a /dashboard

3. DashboardComponent.ngOnInit()
   └─> AuthService.getUserRole()
       └─> Switch case:
           ├─> 'Administrador' → router.navigate(['/admin'])
           ├─> 'Cajero' → router.navigate(['/cajero'])
           ├─> 'Vendedor' → router.navigate(['/vendedor'])
           └─> 'Supervisor' → router.navigate(['/supervisor'])
```

### Apertura de Caja (Cajero)
```
1. CajeroDashboardComponent.ngOnInit()
   └─> CashSessionService.getOpenSession()
       └─> Signal: currentSession

2. Usuario click "Abrir Caja"
   └─> openCashSessionModal()
       └─> Signal: showOpenModal = true

3. Usuario ingresa monto y confirma
   └─> confirmOpenSession()
       └─> CashSessionService.openSession(request)
           └─> Backend: POST /api/CashSession/open
               └─> Actualiza signal: currentSession
                   └─> UI se actualiza automáticamente (computed values)
```

### Ventas del Vendedor
```
1. VendedorDashboardComponent.ngOnInit()
   └─> loadMySales()
       └─> SaleService.getTodaySales()
           └─> Backend: GET /api/Sale/by-date-range
               └─> Signal: mySales

2. Computed values se actualizan automáticamente:
   ├─> todayTotalSales = computed(() => sum(mySales))
   ├─> todayTransactionCount = computed(() => count(mySales))
   └─> goalProgress = computed(() => (total / goal) * 100)

3. UI se actualiza reactivamente
```

---

## Signals y Computed Values

### Patrón Utilizado en Todos los Dashboards

```typescript
// Signals para datos
todaySales = signal<SaleDto[]>([]);
loading = signal(false);

// Computed values (se actualizan automáticamente)
todayRevenue = computed(() => {
  return this.todaySales().reduce((sum, sale) => sum + sale.grandTotal, 0);
});

// Uso en template
{{ todayRevenue() | currency }}
```

### Ventajas
- ✅ Reactivo automático
- ✅ No necesita subscripciones manuales
- ✅ Performance optimizado (solo recalcula cuando cambian dependencias)
- ✅ Código más limpio y mantenible

---

## Estilos y Temas

### Variables CSS Utilizadas
```css
--color-primary
--color-success
--color-warning
--color-danger
--color-info
--color-surface
--color-background
--color-border
--color-text-primary
--color-text-secondary
--space-1 to --space-8
--radius-sm, --radius-md, --radius-lg
--shadow-sm, --shadow-md, --shadow-lg
```

### Componentes de Estilo
- Todos los dashboards usan las mismas variables CSS
- Diseño consistente en toda la aplicación
- Fácil cambio de tema global

---

## Guías de Seguridad

### Guards Implementados
```typescript
// authGuard - Verifica autenticación
{
  path: 'dashboard',
  canActivate: [authGuard]
}

// roleGuard - Verifica rol específico
{
  path: 'cajero',
  canActivate: [authGuard, roleGuard],
  data: { roles: ['Cajero'] }
}
```

### Verificación en Componentes
```typescript
// En el componente
currentUser = this.authService.currentUser;
userRole = this.authService.getUserRole();

// En el template
@if (authService.isCajero()) {
  <!-- Contenido solo para cajeros -->
}
```

---

## Errores Comunes y Soluciones

### 1. "Math is not defined" en templates
**Problema**: No se puede usar Math directamente en templates Angular

**Solución**: Exponer Math como propiedad del componente
```typescript
export class MyComponent {
  Math = Math; // Exponer en la clase
}
```

### 2. Variantes de botón incorrectas
**Problema**: Usar 'danger' o 'outline' que no existen

**Solución**: Usar variantes válidas
```typescript
type ButtonVariant = 'primary' | 'secondary' | 'ghost' | 'destructive';
```

### 3. Modal no se muestra
**Problema**: Usar @if en lugar de [open]

**Solución**:
```html
<!-- Incorrecto -->
@if (showModal()) {
  <app-modal>...</app-modal>
}

<!-- Correcto -->
<app-modal [open]="showModal()">...</app-modal>
```

---

## Checklist de Testing

### Por Dashboard
- [ ] Verifica que carga datos correctamente
- [ ] Verifica que signals se actualizan
- [ ] Verifica que computed values funcionan
- [ ] Verifica responsive design (mobile, tablet, desktop)
- [ ] Verifica accesibilidad (navegación por teclado)
- [ ] Verifica manejo de errores
- [ ] Verifica estados de carga
- [ ] Verifica permisos de rol

### General
- [ ] Login redirige al dashboard correcto según rol
- [ ] Guards bloquean acceso no autorizado
- [ ] Botones y links funcionan correctamente
- [ ] Modales abren y cierran correctamente
- [ ] Formularios validan datos
- [ ] Mensajes de error son claros

---

## Métricas de Código

### Líneas de Código por Dashboard
- Cajero: ~370 líneas (TS + HTML + SCSS)
- Vendedor: ~320 líneas
- Supervisor: ~380 líneas
- Admin: Ya existente

### Componentes Reutilizables
- StatCard: ~130 líneas
- ChartCard: ~120 líneas

### Total Añadido
- Archivos nuevos: 11
- Archivos modificados: 3
- Total líneas de código: ~1,500 líneas

---

## Comandos Útiles

### Desarrollo
```bash
# Ejecutar en modo desarrollo
npm start

# Compilar
npm run build

# Verificar tipos
npm run ng build -- --configuration production
```

### Testing
```bash
# Tests unitarios
npm test

# Tests e2e
npm run e2e

# Coverage
npm run test:coverage
```

### Linting
```bash
# Verificar código
npm run lint

# Auto-fix
npm run lint:fix
```

---

## Contacto y Soporte

Para dudas sobre la implementación:
1. Revisar `DASHBOARD_IMPLEMENTATION.md` para detalles técnicos
2. Revisar `ENDPOINTS_FALTANTES.md` para endpoints del backend
3. Consultar código fuente con comentarios inline

**Fecha de implementación**: 13 de noviembre de 2025
**Versión de Angular**: 19.x
**Estado**: Compilación exitosa, listo para testing
