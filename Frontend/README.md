# 🏪 EstancoPro - Frontend

Frontend de EstancoPro desarrollado con **Angular 18** y **Tailwind CSS** con paleta verde temática de estanco.

## ✅ Estado Actual

### Completado

- ✅ Proyecto Angular 18 creado
- ✅ Tailwind CSS configurado con paleta verde
- ✅ Estructura de carpetas profesional
- ✅ Modelos TypeScript (interfaces) sincronizados con el backend
- ✅ Servicios conectados al API REST del backend
- ✅ Sistema de autenticación completo (JWT)
- ✅ Guards (authGuard, publicGuard, roleGuard)
- ✅ Interceptors (auth, error)
- ✅ Componente de Login funcional
- ✅ Componente de Dashboard básico
- ✅ Rutas configuradas con lazy loading

### Por Hacer

- ⏳ Módulo POS (Punto de Venta) completo
- ⏳ Módulo de Productos (CRUD)
- ⏳ Módulo de Caja (apertura/cierre)
- ⏳ Módulo de Compras
- ⏳ Sistema de permisos en UI
- ⏳ Reportes y gráficos

---

## 🚀 Cómo Iniciar

### 1. Instalar dependencias

```bash
cd Frontend
npm install
```

### 2. Verificar que el backend esté corriendo

El backend debe estar corriendo en `http://localhost:5170`

```bash
cd ../Backend/Web
dotnet run
```

### 3. Iniciar el frontend

```bash
npm start
```

La aplicación estará disponible en: `http://localhost:4200`

---

## 🔐 Credenciales de Prueba

- **Email:** `admin@gmail.com`
- **Password:** `Admin123*`

---

## 📂 Estructura del Proyecto

```
src/app/
├── core/                          # Funcionalidad core del app
│   ├── guards/                    # Guards de rutas
│   │   ├── auth.guard.ts         # Protege rutas privadas
│   │   ├── public.guard.ts       # Protege rutas públicas
│   │   └── role.guard.ts         # Protege por roles
│   ├── interceptors/              # Interceptors HTTP
│   │   ├── auth.interceptor.ts   # Añade JWT a peticiones
│   │   └── error.interceptor.ts  # Maneja errores HTTP
│   ├── services/                  # Servicios globales
│   │   ├── auth.service.ts       # Autenticación
│   │   ├── product.service.ts    # Productos
│   │   ├── sale.service.ts       # Ventas
│   │   └── cash.service.ts       # Caja
│   └── models/                    # Interfaces TypeScript
│       ├── auth.model.ts
│       ├── product.model.ts
│       ├── sale.model.ts
│       ├── cash.model.ts
│       └── purchase.model.ts
├── modules/                       # Módulos funcionales
│   ├── auth/
│   │   └── login/                # Componente login
│   ├── dashboard/                # Dashboard principal
│   ├── pos/                      # Punto de venta (placeholder)
│   ├── products/                 # Gestión productos (placeholder)
│   ├── cash/                     # Control caja (por crear)
│   └── admin/                    # Administración (por crear)
├── shared/                        # Componentes compartidos
│   ├── components/               # Componentes reutilizables
│   └── pipes/                    # Pipes personalizados
└── environments/                  # Configuración entornos
    ├── environment.ts            # Desarrollo
    └── environment.prod.ts       # Producción
```

---

## 🎨 Paleta de Colores

### Verdes Principales
- `#2D5A2D` - Verde estanco principal
- `#3D7A3D` - Verde claro
- `#7CB342` - Verde lima (botones)
- `#C8E6C9` - Verde pastel (bordes)

### Colores de Estado
- `#43A047` - Success (verde)
- `#FBC02D` - Warning (amarillo)
- `#E53935` - Error (rojo)
- `#039BE5` - Info (azul)

### Colores Financieros
- `#4CAF50` - Ingresos/Ventas
- `#EF5350` - Gastos/Egresos
- `#FFB300` - Utilidad/Ganancias

Ver `tailwind.config.js` y `src/styles.scss` para más detalles.

---

## 🔗 Integración con Backend

### Configuración de API

El archivo `src/environments/environment.ts` contiene la URL del backend:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5170/api'
};
```

### Servicios Conectados

Todos los servicios están configurados para consumir el API REST:

- **AuthService** → `/api/Auth/*`
- **ProductService** → `/api/Product/*`, `/api/Category/*`
- **SaleService** → `/api/Sale/*`
- **CashService** → `/api/CashSession/*`

### Autenticación

El sistema usa **JWT Bearer Token**:

1. Login → Recibe token
2. AuthInterceptor añade automáticamente `Authorization: Bearer {token}` a todas las peticiones
3. ErrorInterceptor maneja errores 401 (token expirado)

---

## 📦 Dependencias Principales

```json
{
  "@angular/core": "^18.2.0",
  "@angular/common": "^18.2.0",
  "@angular/router": "^18.2.0",
  "tailwindcss": "^3.4.0",
  "rxjs": "~7.8.0"
}
```

---

## 🛠️ Scripts Disponibles

```bash
npm start          # Inicia servidor de desarrollo (puerto 4200)
npm run build      # Build de producción
npm test           # Ejecuta tests unitarios
npm run lint       # Ejecuta linter
```

---

## 🔄 Próximos Pasos

### Fase 1: Funcionalidad Básica
1. Completar módulo POS (punto de venta)
2. Implementar CRUD de productos
3. Sistema de apertura/cierre de caja

### Fase 2: Funcionalidad Avanzada
4. Gestión de compras
5. Sistema de permisos granular en UI
6. Alertas de stock bajo

### Fase 3: Reportes y Analytics
7. Dashboard con estadísticas reales
8. Reportes de ventas
9. Gráficos con Chart.js o similar

---

## 📝 Notas de Desarrollo

### Componentes Standalone
Este proyecto usa **standalone components** (Angular 18):
- No hay módulos NgModule
- Cada componente importa lo que necesita
- Lazy loading con `loadComponent()`

### Reactive Forms
Los formularios usan **ReactiveFormsModule**:
- Validación robusta
- Control programático
- Fácil testing

### Observables
Se usa RxJS para manejo de estado:
- `BehaviorSubject` para usuario actual
- `BehaviorSubject` para sesión de caja actual
- Operadores como `tap`, `catchError`, etc.

---

## 🐛 Troubleshooting

### Error: Cannot connect to backend

**Solución:** Verifica que el backend esté corriendo en `http://localhost:5170`

```bash
cd Backend/Web
dotnet run
```

### Error: CORS

**Solución:** El backend ya tiene CORS configurado. Si persiste, verifica `Program.cs`:

```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

### Error: Tailwind no carga estilos

**Solución:** Verifica que `styles.scss` tenga las directivas de Tailwind:

```scss
@tailwind base;
@tailwind components;
@tailwind utilities;
```

---

**Última actualización:** 2025-11-14
