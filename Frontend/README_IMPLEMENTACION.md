# EstancoPro - Frontend Angular

## ✅ Estado Actual de Implementación

### Sistema de Autenticación Completo
- ✅ Login funcional conectado al backend ASP.NET Core
- ✅ Gestión de JWT tokens (acceso y refresh)
- ✅ Guards para proteger rutas
- ✅ Interceptores HTTP para añadir tokens automáticamente
- ✅ Manejo de errores y redirecciones
- ✅ Compatible con SSR (Server-Side Rendering)

### Diseño UI
- ✅ Sistema de diseño con tokens CSS (colores, tipografías, espaciado)
- ✅ Paleta de colores EstancoPro
  - Primario: #0F4C5C
  - Éxito: #2A9D8F
  - Acento: #E9C46A
  - Error: #E63946
- ✅ Tipografías Inter (UI) y Roboto Mono (números)

### Componentes Base
- ✅ ButtonComponent (primary, secondary, ghost, destructive)
- ✅ InputComponent (con validaciones y estados de error)

## 🚀 Cómo Ejecutar

### Prerequisitos
1. Backend corriendo en `http://localhost:5000`
2. Node.js y npm instalados

### Iniciar Frontend
```bash
cd Frontend
npm install
npm start
```

La aplicación estará disponible en `http://localhost:4200`

### Credenciales de Prueba
Para probar el login, necesitas crear un usuario en la base de datos del backend.

## 📁 Estructura del Proyecto

```
src/
├── app/
│   ├── core/                      # Funcionalidad central
│   │   ├── guards/                # Guards de navegación
│   │   │   └── auth.guard.ts      # Protección de rutas
│   │   ├── interceptors/          # Interceptores HTTP
│   │   │   ├── auth.interceptor.ts    # Añade JWT a requests
│   │   │   └── error.interceptor.ts   # Manejo de errores
│   │   ├── models/                # Interfaces TypeScript
│   │   │   ├── auth.model.ts      # Modelos de autenticación
│   │   │   └── api-response.model.ts
│   │   └── services/              # Servicios compartidos
│   │       ├── api.service.ts     # Cliente HTTP genérico
│   │       └── auth.service.ts    # Gestión de autenticación
│   │
│   ├── shared/                    # Componentes compartidos
│   │   └── components/
│   │       ├── button.component.ts
│   │       └── input.component.ts
│   │
│   ├── features/                  # Módulos funcionales
│   │   ├── auth/
│   │   │   └── pages/
│   │   │       └── login.component.ts
│   │   └── dashboard/
│   │       └── dashboard.component.ts
│   │
│   ├── app.routes.ts             # Configuración de rutas
│   └── app.config.ts             # Configuración de la app
│
├── environments/                  # Variables de entorno
│   ├── environment.ts
│   ├── environment.development.ts
│   └── environment.prod.ts
│
└── styles.scss                   # Estilos globales y tokens

```

## 🔐 Autenticación

### Flujo de Autenticación
1. Usuario ingresa credenciales en `/login`
2. Se envía POST a `/api/Auth/login`
3. Backend responde con `token` y `refreshToken`
4. Tokens se guardan en localStorage (solo en navegador)
5. Usuario es redirigido a `/dashboard`
6. Todas las peticiones posteriores incluyen el token JWT

### Protección de Rutas
```typescript
// Ruta protegida (requiere autenticación)
{
  path: 'dashboard',
  component: DashboardComponent,
  canActivate: [authGuard]
}

// Ruta de login (redirige si ya está autenticado)
{
  path: 'login',
  component: LoginComponent,
  canActivate: [loginGuard]
}
```

## 🎨 Sistema de Diseño

### Tokens CSS
Todos los valores de diseño están definidos como variables CSS en `styles.scss`:

```scss
// Colores
--color-primary: #0F4C5C
--color-success: #2A9D8F
--color-accent: #E9C46A
--color-error: #E63946

// Tipografías
--font-ui: 'Inter'
--font-mono: 'Roboto Mono'

// Espaciado
--space-1 a --space-8

// Bordes
--radius-sm, --radius-md, --radius-lg
```

### Uso en Componentes
```scss
.my-button {
  background-color: var(--color-primary);
  padding: var(--space-4);
  border-radius: var(--radius-md);
  font-family: var(--font-ui);
}
```

## 🧩 Componentes

### ButtonComponent
```typescript
<app-button
  variant="primary"          // primary | secondary | ghost | destructive
  size="md"                  // sm | md | lg
  [loading]="isLoading"
  [disabled]="isDisabled"
  [fullWidth]="true"
  (clicked)="handleClick()"
>
  Texto del botón
</app-button>
```

### InputComponent
```typescript
<app-input
  id="email"
  type="email"
  label="Correo electrónico"
  placeholder="usuario@ejemplo.com"
  [value]="email"
  (valueChange)="onEmailChange($event)"
  [error]="errorMessage"
  [required]="true"
/>
```

## 🔌 Conexión con Backend

### Configuración de API
```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

### Uso del ApiService
```typescript
// Inyectar el servicio
private apiService = inject(ApiService);

// GET
this.apiService.get<Product[]>('Product').subscribe(products => {
  console.log(products);
});

// POST
this.apiService.post<Product>('Product', newProduct).subscribe(result => {
  console.log('Producto creado:', result);
});

// PUT
this.apiService.put<Product>('Product', updatedProduct).subscribe();

// DELETE
this.apiService.delete<void>('Product/123').subscribe();
```

### Token JWT Automático
Los interceptores añaden automáticamente el token JWT a todas las peticiones:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📱 Accesibilidad

- ✅ Contraste de colores ≥ 4.5:1
- ✅ Focus visible para navegación por teclado
- ✅ Aria-labels en componentes interactivos
- ✅ Mensajes de error claros
- ✅ Estados de loading visibles

## 🎯 Próximos Pasos

### Módulos Pendientes de Implementación:

1. **Dashboard con KPIs**
   - Ventas del día
   - Estado de caja
   - Productos con bajo stock
   - Últimas transacciones

2. **POS (Punto de Venta)**
   - Búsqueda de productos por SKU/nombre
   - Scanner de códigos de barras
   - Carrito de compras
   - Cálculo de totales e impuestos
   - Validación de stock en tiempo real

3. **Gestión de Inventario**
   - Lista de productos con filtros
   - Editor de productos
   - Gestión de categorías
   - Unidades de medida
   - Ajustes de stock

4. **Gestión de Caja**
   - Abrir sesión de caja
   - Registrar movimientos
   - Cerrar caja con conteo
   - Historial de sesiones

5. **Módulo de Compras**
   - Gestión de proveedores
   - Crear órdenes de compra
   - Recibir mercancía
   - Registro de pagos

6. **Usuarios y Permisos**
   - Gestión de roles (Admin, Empleado)
   - Permisos por módulo
   - Restricciones en UI y backend

## 🐛 Troubleshooting

### Error: "No se puede conectar con el backend"
- Verifica que el backend esté corriendo en `http://localhost:5000`
- Revisa la consola del backend por errores
- Verifica CORS en `Program.cs`

### Error: "localStorage is not defined"
- Este error ya está solucionado con la detección de plataforma
- El servicio AuthService verifica si está en el navegador antes de usar localStorage

### Error: "Token inválido" o 401
- El token puede haber expirado (60 minutos por defecto)
- Cierra sesión e inicia sesión nuevamente
- Verifica la configuración JWT en `appsettings.json`

## 📝 Notas de Desarrollo

### Angular Puro con TypeScript
Este proyecto está desarrollado completamente con:
- ✅ Angular 20 (última versión)
- ✅ TypeScript
- ✅ Standalone Components (sin NgModules)
- ✅ Signals para estado reactivo
- ✅ Control Flow Syntax (@if, @for)
- ✅ SSR compatible

### Buenas Prácticas Implementadas
- ✅ Separación de concerns (core, shared, features)
- ✅ Servicios inyectables reutilizables
- ✅ Componentes standalone
- ✅ Tipos fuertes de TypeScript
- ✅ Manejo de errores robusto
- ✅ Compatible con SSR
