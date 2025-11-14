# 🏪 Análisis Completo del Sistema EstancoPro

## 📋 Resumen Ejecutivo

**EstancoPro** es un sistema POS (Point of Sale) especializado para la gestión de un estanco, con capacidades completas de:
- Punto de venta (POS)
- Control de inventario
- Gestión de compras a proveedores
- Control de caja (apertura/cierre)
- Sistema de roles y permisos granulares
- Autenticación JWT con refresh tokens

---

## 🏗️ Arquitectura del Sistema

### Patrón de Diseño
- **Backend**: Clean Architecture con capas separadas
  - `Entity`: Modelos y DTOs
  - `Data`: Repositorios y DbContext
  - `Business`: Lógica de negocio
  - `Web`: Controladores API REST

### Tecnologías
- **Backend**: .NET Core 8+ con Entity Framework Core
- **Base de Datos**: SQL Server
- **Autenticación**: JWT Bearer con BCrypt para passwords
- **Frontend**: Angular 18+ (por crear)

---

## 🔐 Sistema de Seguridad y Permisos

### Arquitectura de Roles

```
Usuario (User)
    ↓
UserRol (relación N:N)
    ↓
Rol (Role)
    ↓
RolFormPermission (granular)
    ↓
Form (Formularios/Pantallas) + Permission (Create, Read, Update, Delete)
```

### Entidades de Seguridad

#### 1. **User**
- Email (único)
- Password (BCrypt hash)
- RolId (rol principal)
- PersonId (datos personales)
- Campos de auditoría (Base)

#### 2. **Rol**
- TypeRol (ej: "Administrador", "Cajero", "Vendedor")
- Description
- Relaciones: UserRol, RolFormPermission

#### 3. **Permission**
- TypePermission (ej: "Create", "Read", "Update", "Delete", "Execute")
- Description

#### 4. **Form**
- Name (nombre del formulario)
- Description
- Path (ruta en el frontend)
- Relación con Module (para agrupar)

#### 5. **RolFormPermission** (Tabla Pivote)
- RolId
- FormId
- PermissionId
- Define qué puede hacer cada rol en cada formulario

#### 6. **RefreshToken**
- Token (GUID)
- JwtId (relacionado con JWT)
- UserId
- ExpiresAt
- IsUsed, IsRevoked

### Flujo de Autenticación

```
1. Login → POST /api/Auth/login
   ├─ Email + Password
   ├─ Valida con BCrypt
   ├─ Genera JWT (1 hora)
   ├─ Genera RefreshToken (7 días)
   └─ Retorna LoginResponseDto

2. Uso de API → Headers: Authorization: Bearer {token}
   └─ Validación automática por [Authorize]

3. Renovación → POST /api/Auth/refresh
   ├─ Token + RefreshToken
   ├─ Valida que RefreshToken no esté usado/revocado
   ├─ Marca como usado
   └─ Genera nuevo par de tokens

4. Logout → POST /api/Auth/logout
   └─ Revoca RefreshToken

5. Logout Global → POST /api/Auth/logout-all
   └─ Revoca TODOS los RefreshTokens del usuario
```

---

## 💼 Módulos de Negocio

### 1. 🏪 **Gestión de Productos**

#### Entidades
- **Product**: Productos vendibles
  - Name, UnitCost, UnitPrice, TaxRate
  - StockOnHand (inventario actual)
  - ReorderPoint (punto de reorden)
  - CategoryId, UnitMeasureId

- **Category**: Categorías de productos
  - Name, Description

- **UnitMeasure**: Unidades de medida
  - Abbreviation (ej: "UN", "KG", "LT")
  - Name (ej: "Unidad", "Kilogramo")

- **ProductUnitPrice**: Precios por unidad
  - ProductId
  - UnitMeasureId
  - Price

#### Endpoints Principales
```
GET    /api/Product           - Listar todos
GET    /api/Product/{id}      - Obtener por ID
POST   /api/Product           - Crear
PUT    /api/Product/{id}      - Actualizar
DELETE /api/Product/{id}      - Eliminar (soft delete)
```

---

### 2. 🛒 **Gestión de Compras (Purchase)**

#### Entidad Principal
- **Purchase**
  - InvoiceNumber (número de factura)
  - OrderedAt (fecha de pedido)
  - ReceivedAt? (fecha de recepción)
  - Status ("Ordered" | "Received" | "Cancelled")
  - Subtotal, TaxTotal, GrandTotal
  - SupplierId

- **PurchaseProductDetail** (líneas de compra)
  - PurchaseId
  - ProductId
  - Quantity
  - UnitCost
  - Subtotal
  - TaxAmount

- **Supplier** (Proveedores)
  - Name, ContactName, Phone, Email, Address

#### Flujo de Negocio
```
1. Crear compra → POST /api/Purchase
   └─ Estado: "Ordered"

2. Agregar productos → POST /api/PurchaseProductDetail
   └─ Líneas de detalle

3. Recibir compra → POST /api/Purchase/{id}/receive
   ├─ Body: { payInCash: true, cashSessionId: 1 }
   ├─ Incrementa StockOnHand de cada producto
   ├─ Si payInCash=true → Registra en CashMovement (SALIDA)
   └─ Estado: "Received"

4. Cancelar → POST /api/Purchase/{id}/cancel
   └─ Body: { reason: "..." }
```

#### Endpoints
```
POST /api/Purchase/{id}/receive        - Recibir compra
POST /api/Purchase/{id}/cancel         - Cancelar
GET  /api/Purchase/by-supplier/{name}  - Por proveedor
GET  /api/Purchase/by-date-range       - Por fechas
GET  /api/Purchase/by-status           - Por estado
```

---

### 3. 💰 **Punto de Venta (POS) - Sales**

#### Entidad Principal
- **Sale**
  - SoldAt (fecha/hora de venta)
  - Status ("Draft" | "Completed" | "Cancelled")
  - Subtotal, TaxTotal, GrandTotal
  - CashSessionId (sesión de caja asociada)

- **SaleProductDetail** (líneas de venta)
  - SaleId
  - ProductId
  - Quantity
  - UnitPrice
  - Subtotal
  - TaxAmount

#### Flujo de Negocio (CRÍTICO)
```
1. Crear venta → POST /api/Sale
   └─ Estado: "Draft"

2. Agregar productos → POST /api/SaleProductDetail
   └─ Líneas de detalle

3. Recalcular → POST /api/Sale/{id}/recalculate-totals
   └─ Actualiza Subtotal, TaxTotal, GrandTotal

4. Finalizar venta → POST /api/Sale/{id}/finalize ⭐ CRÍTICO
   ├─ Valida stock disponible
   ├─ Descuenta StockOnHand de cada producto
   ├─ Registra en CashMovement (ENTRADA)
   ├─ Estado: "Completed"
   └─ ❌ Si falla stock → Revierte y retorna error

5. Cancelar → POST /api/Sale/{id}/cancel
   └─ Solo si está en "Draft"
```

#### Endpoints
```
POST /api/Sale/{id}/finalize             - Finalizar (⭐ MÁS CRÍTICO)
POST /api/Sale/{id}/cancel               - Cancelar
POST /api/Sale/{id}/recalculate-totals   - Recalcular
GET  /api/Sale/by-cash-session/{id}      - Ventas por sesión
GET  /api/Sale/by-date-range             - Por fechas
GET  /api/Sale/by-status                 - Por estado
GET  /api/Sale/report                    - Reporte con totales
```

---

### 4. 💵 **Control de Caja (Cash Management)**

#### Entidad Principal
- **CashSession**
  - OpenedAt (fecha/hora apertura)
  - ClosedAt? (fecha/hora cierre)
  - OpeningAmount (monto inicial)
  - ExpectedClosingAmount (calculado)
  - ActualClosingAmount (conteo físico)
  - Difference (diferencia)
  - Status ("Open" | "Closed")

- **CashMovement** (movimientos de caja)
  - CashSessionId
  - At (timestamp) - ⚠️ PK compuesta con CashSessionId
  - TypeMovement ("Income" | "Expense")
  - Amount
  - Reason (descripción)
  - ReferenceType ("Sale" | "Purchase" | "Adjustment" | "Other")
  - ReferenceId (ID de la venta/compra)

#### Flujo de Negocio
```
1. Abrir caja → POST /api/CashSession/open
   ├─ Body: { openingAmount: 100000 }
   ├─ Valida que NO haya otra sesión abierta
   └─ Estado: "Open"

2. Durante el día:
   ├─ Ventas → Registra CashMovement (Income, Sale)
   ├─ Compras pagadas → Registra CashMovement (Expense, Purchase)
   └─ Ajustes manuales → POST /api/CashMovement

3. Consultar balance → GET /api/CashSession/{id}/balance
   └─ Retorna: { expected, movements, total }

4. Cerrar caja → POST /api/CashSession/{id}/close
   ├─ Body: { closingAmount: 150000 }
   ├─ Calcula: Difference = ActualClosingAmount - ExpectedClosingAmount
   ├─ Estado: "Closed"
   └─ Retorna: { difference, status: "Sobrante" | "Faltante" }
```

#### Endpoints
```
POST /api/CashSession/open                 - Abrir sesión
POST /api/CashSession/{id}/close           - Cerrar sesión
GET  /api/CashSession/open                 - Obtener sesión abierta
GET  /api/CashSession/{id}/balance         - Balance actual
GET  /api/CashSession/by-date-range        - Historial
```

---

## 📊 Flujo Completo de un Día de Operación

```
08:00 AM - APERTURA
├─ Cajero hace login
├─ POST /api/CashSession/open { openingAmount: 50000 }
└─ Frontend muestra sesión activa

09:00 AM - PRIMERA VENTA
├─ POST /api/Sale → { status: "Draft", cashSessionId: 1 }
├─ POST /api/SaleProductDetail (agregar productos)
├─ POST /api/Sale/1/recalculate-totals
├─ POST /api/Sale/1/finalize ⭐
│   ├─ Descuenta inventario
│   └─ Registra CashMovement (Income)
└─ Frontend imprime ticket

10:00 AM - RECIBE COMPRA
├─ POST /api/Purchase/5/receive { payInCash: true, cashSessionId: 1 }
│   ├─ Incrementa StockOnHand
│   └─ Registra CashMovement (Expense)
└─ Inventario actualizado

...ventas durante el día...

08:00 PM - CIERRE
├─ Cajero cuenta efectivo físico: $180,000
├─ GET /api/CashSession/1/balance
│   └─ { expected: 178500, movements: [...] }
├─ POST /api/CashSession/1/close { closingAmount: 180000 }
│   └─ { difference: 1500, status: "Sobrante" }
└─ Sistema genera reporte de cierre
```

---

## 🚨 Entidades/Funcionalidades Faltantes (Recomendaciones)

### 1. **Cliente (Customer)** - RECOMENDADO
```csharp
public class Customer : Base
{
    public string DocumentType { get; set; }  // CC, NIT, CE
    public string DocumentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public decimal LoyaltyPoints { get; set; }  // Programa de lealtad
}
```
**Razón**: Necesario para facturación electrónica, historial de compras, programas de fidelización.

### 2. **PaymentMethod** - CRÍTICO
```csharp
public class PaymentMethod : Base
{
    public string Name { get; set; }  // Efectivo, Tarjeta, Nequi, Daviplata
    public string Type { get; set; }   // Cash, Card, DigitalWallet
    public bool RequiresReference { get; set; }  // True para transferencias
}

public class SalePayment : Base
{
    public int SaleId { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string Reference { get; set; }  // Número de transacción
    public Sale sale { get; set; }
    public PaymentMethod paymentMethod { get; set; }
}
```
**Razón**: Actualmente no hay forma de registrar el método de pago. Un estanco necesita diferenciar efectivo vs. digital.

### 3. **Inventory Adjustment** - IMPORTANTE
```csharp
public class InventoryAdjustment : Base
{
    public int ProductId { get; set; }
    public int AdjustedQuantity { get; set; }
    public string Reason { get; set; }  // Merma, Robo, Conteo
    public string AdjustmentType { get; set; }  // Increase, Decrease
    public int UserId { get; set; }  // Quién hizo el ajuste
    public Product product { get; set; }
    public User user { get; set; }
}
```
**Razón**: Para manejar pérdidas, mermas, robos, o correcciones de inventario.

### 4. **Expense (Gastos)** - IMPORTANTE
```csharp
public class Expense : Base
{
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; }  // Servicios, Arriendo, Nomina
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public int? CashSessionId { get; set; }  // Si se pagó de caja
    public string InvoiceNumber { get; set; }
    public CashSession cashSession { get; set; }
}
```
**Razón**: Para control completo de gastos operativos del negocio.

### 5. **Notification/Alert System** - ÚTIL
```csharp
public class ProductAlert : Base
{
    public int ProductId { get; set; }
    public string AlertType { get; set; }  // LowStock, Expiring, OutOfStock
    public bool IsRead { get; set; }
    public Product product { get; set; }
}
```
**Razón**: Alertas automáticas cuando un producto llega al ReorderPoint.

### 6. **Dashboard/Reports** - CRÍTICO PARA NEGOCIO
No hay entidades, pero se necesitan endpoints para:
```
GET /api/Dashboard/summary
├─ Ventas del día
├─ Productos más vendidos
├─ Stock bajo
└─ Estado de caja

GET /api/Reports/sales-by-product
GET /api/Reports/sales-by-category
GET /api/Reports/profit-margin
```

---

## 🎨 Paleta de Colores - Temática Estanco

### Colores Principales (Verde Natural)

```css
/* Primary Colors */
--primary-green: #2D5A2D;        /* Verde bosque oscuro - Header/Nav */
--primary-green-light: #3D7A3D;  /* Verde medio - Botones principales */
--primary-green-lighter: #4A9A4A; /* Verde claro - Hover states */

/* Secondary Colors */
--accent-lime: #7CB342;          /* Verde lima - Accents/CTAs */
--accent-mint: #A5D6A7;          /* Verde menta - Badges/Tags */
--accent-sage: #8BC34A;          /* Verde salvia - Links */

/* Neutral Colors */
--background-light: #F1F8F4;     /* Verde muy claro - Backgrounds */
--background-white: #FFFFFF;     /* Blanco puro - Cards/Modals */
--text-dark: #1B3A1B;            /* Verde muy oscuro - Títulos */
--text-gray: #546E7A;            /* Gris azulado - Texto secundario */
--border-light: #C8E6C9;         /* Verde pastel - Bordes */

/* Status Colors */
--success: #43A047;              /* Verde éxito */
--warning: #FBC02D;              /* Amarillo advertencia */
--error: #E53935;                /* Rojo error */
--info: #039BE5;                 /* Azul información */

/* Cash/Money Colors */
--cash-green: #4CAF50;           /* Verde dinero - Ingresos */
--expense-red: #EF5350;          /* Rojo suave - Egresos */
--profit-gold: #FFB300;          /* Dorado - Ganancias */
```

### Aplicación por Componente

```
Sidebar/Navigation
├─ Background: --primary-green (#2D5A2D)
├─ Active Item: --primary-green-light (#3D7A3D)
└─ Hover: --primary-green-lighter (#4A9A4A)

Buttons
├─ Primary: --accent-lime (#7CB342)
├─ Secondary: --accent-sage (#8BC34A)
└─ Outline: --border-light (#C8E6C9)

Cards/Panels
├─ Background: --background-white (#FFFFFF)
├─ Border: --border-light (#C8E6C9)
└─ Shadow: rgba(45, 90, 45, 0.1)

Tables
├─ Header: --primary-green-light (#3D7A3D)
├─ Row Hover: --background-light (#F1F8F4)
└─ Border: --border-light (#C8E6C9)

Dashboard Stats
├─ Ventas: --cash-green (#4CAF50)
├─ Gastos: --expense-red (#EF5350)
├─ Utilidad: --profit-gold (#FFB300)
└─ Stock: --info (#039BE5)
```

### Ejemplo CSS/SCSS

```scss
// variables.scss
$color-primary: #2D5A2D;
$color-primary-light: #3D7A3D;
$color-accent: #7CB342;
$color-background: #F1F8F4;
$color-text: #1B3A1B;
$color-success: #43A047;

// Ejemplo de uso
.btn-primary {
  background-color: $color-accent;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  transition: background-color 0.3s;

  &:hover {
    background-color: darken($color-accent, 10%);
  }
}

.sidebar {
  background: linear-gradient(180deg, $color-primary 0%, darken($color-primary, 10%) 100%);
  color: white;
}

.card {
  background-color: white;
  border: 1px solid #C8E6C9;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(45, 90, 45, 0.1);
}
```

---

## 📝 Roles Recomendados

### 1. **Administrador**
- Acceso total al sistema
- Gestión de usuarios y roles
- Configuración del sistema
- Reportes completos

### 2. **Cajero**
- Abrir/cerrar caja
- Realizar ventas
- Ver inventario (solo lectura)
- Ver sus propias sesiones de caja

### 3. **Vendedor**
- Realizar ventas (si hay caja abierta)
- Ver inventario (solo lectura)
- NO puede abrir/cerrar caja

### 4. **Inventario/Bodega**
- Gestión completa de productos
- Recepción de compras
- Ajustes de inventario
- NO acceso a ventas ni caja

### 5. **Gerente**
- Todos los reportes
- Ver todas las sesiones de caja
- Ver todas las ventas
- NO puede modificar (solo lectura analítica)

---

## 🎯 Próximos Pasos para el Frontend

### 1. **Estructura de Módulos Angular**
```
src/app/
├── core/
│   ├── guards/ (auth.guard, role.guard)
│   ├── interceptors/ (auth.interceptor, error.interceptor)
│   ├── services/ (auth.service, api.service)
│   └── models/ (interfaces de todas las entidades)
├── modules/
│   ├── auth/ (login, logout)
│   ├── dashboard/ (resumen del día)
│   ├── pos/ (punto de venta) ⭐ MÁS IMPORTANTE
│   ├── products/ (CRUD productos)
│   ├── purchases/ (CRUD compras)
│   ├── cash/ (apertura/cierre caja)
│   ├── reports/ (reportes)
│   └── admin/ (usuarios, roles, permisos)
└── shared/
    ├── components/ (sidebar, header, modals)
    └── pipes/ (currency, date formatters)
```

### 2. **Prioridades de Desarrollo**
```
FASE 1 (Crítico - 1 semana)
├─ 1. Login/Auth
├─ 2. Dashboard básico
├─ 3. Gestión de productos (CRUD)
└─ 4. POS (Punto de Venta) ⭐ PRIORIDAD MÁXIMA

FASE 2 (Importante - 1 semana)
├─ 5. Apertura/Cierre de caja
├─ 6. Gestión de compras
└─ 7. Reportes básicos

FASE 3 (Complementario - 1 semana)
├─ 8. Sistema de permisos en UI
├─ 9. Gestión de usuarios
└─ 10. Reportes avanzados
```

### 3. **Pantalla POS (Mockup)**
```
┌─────────────────────────────────────────────────────────────┐
│  🏪 EstancoPro POS                        Cajero: Juan       │
│  Sesión: #25 | Apertura: $50,000    [Cerrar Sesión]         │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────────────┐  ┌───────────────────────────────┐ │
│  │ 🔍 Buscar producto  │  │  CARRITO DE COMPRA            │ │
│  │ [______________]    │  │                               │ │
│  │                     │  │  1. Coca-Cola 1.5L            │ │
│  │ Categorías:         │  │     2x $3,500 = $7,000        │ │
│  │ • Bebidas           │  │                               │ │
│  │ • Cigarrillos       │  │  2. Marlboro Rojo             │ │
│  │ • Snacks            │  │     1x $5,000 = $5,000        │ │
│  │ • Aseo              │  │                               │ │
│  │                     │  │                               │ │
│  │ [Productos más      │  │  ─────────────────────────    │ │
│  │  vendidos: ...]     │  │  Subtotal:        $12,000     │ │
│  │                     │  │  IVA (19%):        $2,280     │ │
│  │                     │  │  ═══════════════════════════   │ │
│  │                     │  │  TOTAL:           $14,280     │ │
│  │                     │  │                               │ │
│  │                     │  │  [COBRAR] [CANCELAR]          │ │
│  └─────────────────────┘  └───────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔧 Configuración Tailwind (Paleta Verde)

```javascript
// tailwind.config.js
module.exports = {
  theme: {
    extend: {
      colors: {
        estanco: {
          primary: '#2D5A2D',
          'primary-light': '#3D7A3D',
          'primary-lighter': '#4A9A4A',
          accent: '#7CB342',
          mint: '#A5D6A7',
          sage: '#8BC34A',
          background: '#F1F8F4',
          text: '#1B3A1B',
          border: '#C8E6C9',
        },
        status: {
          success: '#43A047',
          warning: '#FBC02D',
          error: '#E53935',
          info: '#039BE5',
        },
        cash: {
          green: '#4CAF50',
          red: '#EF5350',
          gold: '#FFB300',
        }
      }
    }
  }
}
```

---

## ✅ Checklist de Implementación

### Backend (Ya completado ✅)
- [x] Entidades de seguridad (User, Rol, Permission)
- [x] Sistema de autenticación JWT
- [x] CRUD de productos
- [x] Gestión de compras
- [x] Sistema de ventas (POS)
- [x] Control de caja
- [x] Movimientos de caja

### Frontend (Por crear)
- [ ] Eliminar carpeta Frontend anterior
- [ ] Crear nuevo proyecto Angular 18
- [ ] Instalar Tailwind con paleta verde
- [ ] Implementar sistema de autenticación
- [ ] Crear módulo POS (PRIORIDAD)
- [ ] Implementar control de caja
- [ ] CRUD de productos
- [ ] Sistema de permisos en UI
- [ ] Reportes y dashboard

### Entidades Pendientes (Recomendadas)
- [ ] Customer (Clientes)
- [ ] PaymentMethod + SalePayment
- [ ] InventoryAdjustment
- [ ] Expense
- [ ] ProductAlert
- [ ] Endpoints de Dashboard

---

## 📞 Soporte

Para cualquier duda sobre la arquitectura o implementación, revisar:
- `Backend/Web/Controllers/` - Endpoints disponibles
- `Backend/Entity/Model/` - Modelos de datos
- Este documento - Referencia completa del sistema

**Última actualización**: 2025-11-14
