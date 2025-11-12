# Métodos Específicos por Entidad - EstancoPro

Este documento detalla los métodos específicos que necesita cada entidad del sistema más allá del CRUD básico, basándose en las reglas de negocio y flujos operativos.

---

## 📋 MODELO DE SEGURIDAD

### User, Rol, Person, Permission, Form, Module, etc.

**Estado:** ✅ Ya cumple con CRUD básico

**Métodos específicos ya implementados:**
- `Login` (autenticación)
- `RefreshToken` (renovar token)
- Gestión de permisos por rol-formulario

**Por qué solo CRUD:** Son entidades de catálogo/configuración que solo necesitan operaciones básicas de mantenimiento.

---

## 🏷️ CATÁLOGOS BÁSICOS

### Category (Categoría)

**Estado:** ✅ Solo necesita CRUD básico

**Por qué:** Es una entidad de catálogo simple. Las consultas de "productos por categoría" se hacen desde `Product`, no desde `Category`.

---

### UnitMeasure (Unidad de Medida)

**Estado:** ✅ Solo necesita CRUD básico

**Por qué:** Es catálogo de presentaciones. Solo se valida que `ConversionFactor > 0` en el CreateAsync/UpdateAsync heredado.

**Validaciones en CRUD:**
- `ConversionFactor` debe ser > 0

---

### Supplier (Proveedor)

**Estado:** ✅ Solo necesita CRUD básico

**Por qué:** Es catálogo simple de proveedores. Las consultas de "compras por proveedor" se hacen desde `Purchase`.

---

## 📦 INVENTARIO Y PRECIOS

### Product (Producto)

**Métodos específicos necesarios:**

```csharp
// 1. Obtener productos con stock bajo (para reposición)
Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();
// Lógica: WHERE StockOnHand <= ReorderPoint AND DeleteAt IS NULL

// 2. Obtener productos por categoría (búsqueda por nombre)
Task<IEnumerable<ProductDto>> GetByCategoryNameAsync(string categoryName);
// Lógica: JOIN con Category, filtrar por Category.Name

// 3. Ajustar stock manualmente (inventario físico, mermas, ajustes) - por nombre
Task AdjustStockAsync(string productName, int quantityChange, string reason);
// Lógica:
// - Buscar producto por nombre
// - StockOnHand += quantityChange (puede ser negativo)
// - Validar que no quede negativo

// 4. Obtener stock en diferentes presentaciones - por nombre
Task<Dictionary<string, decimal>> GetStockByPresentationsAsync(string productName);
// Lógica:
// - Buscar producto por nombre
// - JOIN con ProductUnitPrice para obtener todas las presentaciones
// - Devuelve stock convertido a cada UnitMeasure
// Ej: { "Unidad": 200, "Paquete": 33.33, "Caja": 8.33 }
```

**Por qué:**
- **GetLowStockProductsAsync**: KPI crítico para reposición (punto 6 del doc)
- **GetByCategoryAsync**: Navegación común en UI
- **AdjustStockAsync**: Ajustes de inventario, mermas, correcciones
- **GetStockByPresentationsAsync**: Mostrar disponibilidad en todas las presentaciones

---

### ProductUnitPrice (Precio por Presentación)

**Métodos específicos necesarios:**

```csharp
// 1. Obtener todos los precios de un producto (búsqueda por nombre de producto)
Task<IEnumerable<ProductUnitPriceDto>> GetByProductNameAsync(string productName);
// Lógica: JOIN con Product, filtrar por Product.Name

// 2. Obtener precio específico (para UX rápida en POS - búsqueda por nombres)
Task<ProductUnitPriceDto?> GetPriceByNamesAsync(string productName, string unitMeasureName);
// Lógica: JOIN con Product y UnitMeasure, buscar por nombres, retornar null si no existe
```

**Por qué:**
- **GetByProductNameAsync**: Cargar todas las presentaciones disponibles al seleccionar un producto por nombre (regla UX punto 7)
- **GetPriceByNamesAsync**: Consulta rápida para precargar precio/costo en ventas/compras usando nombres visibles al usuario

---

## 🛒 COMPRAS

### Purchase (Compra)

**Métodos específicos necesarios:**

```csharp
// 1. Recibir compra (flujo 3.1)
Task ReceivePurchaseAsync(int purchaseId, bool payInCash = false, int? cashSessionId = null);
// Lógica:
// - Validar Status == Ordered
// - Validar que tenga líneas
// - Por cada línea: stock += qty × ConversionFactor
// - Status = Received, ReceivedAt = ahora
// - Si payInCash && cashSessionId: crear CashMovement tipo PurchasePayment
// Validaciones críticas (punto 4)

// 2. Cancelar compra
Task CancelPurchaseAsync(int purchaseId, string reason);
// Lógica:
// - Validar Status == Ordered
// - Cambiar Status a Cancelled

// 3. Obtener compras por proveedor (búsqueda por nombre)
Task<IEnumerable<PurchaseDto>> GetBySupplierNameAsync(string supplierName);
// Lógica: JOIN con Supplier, filtrar por Supplier.Name

// 4. Obtener compras por rango de fechas
Task<IEnumerable<PurchaseDto>> GetByDateRangeAsync(DateTime from, DateTime to);

// 5. Obtener compras por estado
Task<IEnumerable<PurchaseDto>> GetByStatusAsync(bool status);
```

**Por qué:**
- **ReceivePurchaseAsync**: Flujo crítico de entrada de inventario con impacto en stock y caja (punto 3.1)
- **CancelPurchaseAsync**: Cancelar pedidos no recibidos
- **Get filtrados**: Reportes y consultas comunes (punto 6)

---

### PurchaseProductDetail (Detalle de Compra)

**Estado:** ✅ Ya cumple con CRUD básico

**Por qué:** Las líneas de compra se gestionan siempre en el contexto de una compra específica (ya se tiene el purchaseId). El frontend trabaja con el encabezado de Purchase que ya tiene su ID. No necesita búsquedas por nombre aquí.

---

## 💰 VENTAS

### Sale (Venta)

**Métodos específicos necesarios:**

```csharp
// 1. Finalizar venta (flujo 3.2) - EL MÁS CRÍTICO
Task FinalizeSaleAsync(int saleId);
// Lógica:
// - Validar Status == Draft (validación punto 4)
// - Validar CashSessionId no nulo y sesión abierta (validación punto 4)
// - Recalcular totales por línea y encabezado (regla 2.3)
// - Validar stock suficiente con conversiones (regla 2.1, validación punto 4)
// - Descontar stock por cada línea: stock -= qty × ConversionFactor
// - Crear CashMovement tipo Sale por GrandTotal
// - Status = Completed, SoldAt = ahora

// 2. Cancelar venta en borrador
Task CancelSaleAsync(int saleId);
// Lógica:
// - Validar Status == Draft
// - Eliminar líneas y venta

// 3. Recalcular totales (al agregar/editar líneas)
Task RecalculateTotalsAsync(int saleId);
// Lógica: aplicar reglas punto 2.3 (subtotal, tax, total)

// 4. Obtener ventas por sesión de caja
Task<IEnumerable<SaleDto>> GetByCashSessionAsync(int cashSessionId);

// 5. Obtener ventas por rango de fechas
Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime from, DateTime to);

// 6. Obtener ventas por estado
Task<IEnumerable<SaleDto>> GetByStatusAsync(string status);

// 7. Obtener reporte de ventas con totales
Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to);
// Lógica: sumar GrandTotal, agrupar por categoría/producto/presentación
```

**Por qué:**
- **FinalizeSaleAsync**: Flujo crítico con múltiples validaciones y efectos (stock, caja, totales) - punto 3.2
- **RecalculateTotalsAsync**: Mantener totales sincronizados al editar carrito - punto 2.3
- **CancelSaleAsync**: Limpiar borradores abandonados
- **Get filtrados**: Reportes críticos (punto 6)

---

### SaleProductDetail (Detalle de Venta)

**Estado:** ✅ Ya cumple con CRUD básico

**Por qué:** Las líneas de venta se gestionan siempre en el contexto de una venta específica (carrito). La validación de stock se hace en Business al finalizar la venta, no necesita método específico en Data.

---

## 💵 CAJA

### CashSession (Sesión de Caja)

**Métodos específicos necesarios:**

```csharp
// 1. Abrir sesión (flujo 3.3)
Task<CashSessionDto> OpenSessionAsync(decimal openingAmount);
// Lógica:
// - Validar que no haya otra sesión abierta (ClosedAt IS NULL)
// - Crear sesión con OpeningAmount, OpenedAt = ahora
// - Opcional: crear CashMovement tipo Opening

// 2. Cerrar sesión (flujo 3.3)
Task CloseSessionAsync(int sessionId, decimal closingAmount);
// Lógica:
// - Validar sesión existe y está abierta
// - Calcular esperado = OpeningAmount + ΣEntradas - ΣSalidas
// - ClosingAmount = closingAmount (conteo físico)
// - ClosedAt = ahora
// - Retornar diferencia (closingAmount - esperado)

// 3. Obtener sesión abierta actual
Task<CashSessionDto?> GetOpenSessionAsync();
// Lógica: WHERE ClosedAt IS NULL

// 4. Obtener sesiones por rango de fechas
Task<IEnumerable<CashSessionDto>> GetByDateRangeAsync(DateTime from, DateTime to);

// 5. Calcular balance de sesión
Task<CashSessionBalanceDto> GetSessionBalanceAsync(int sessionId);
// Lógica:
// - expected = OpeningAmount + Σ(entradas) - Σ(salidas)
// - actual = ClosingAmount ?? expected (si aún abierta)
// - difference = actual - expected
// Retornar: { Expected, Actual, Difference, Movements[] }
```

**Por qué:**
- **OpenSessionAsync**: Inicio del turno con validación de única sesión abierta (flujo 3.3)
- **CloseSessionAsync**: Cuadre de caja con diferencia física vs esperado (flujo 3.3, punto 6)
- **GetOpenSessionAsync**: Validar sesión antes de ventas/movimientos (validación punto 4)
- **GetSessionBalanceAsync**: Reporte de cuadre de caja (punto 6)

---

### CashMovement (Movimiento de Caja)

**Estado:** ✅ Ya cumple con CRUD básico

**Por qué:** Los movimientos de caja se consultan siempre en el contexto de una sesión específica (ya se tiene el cashSessionId desde CashSession). Los cálculos de balance se hacen en Business, no en Data.

---

## 📊 RESUMEN POR CAPA

### ✅ Solo CRUD básico (heredan todo de BaseData):
- **Category**: catálogo simple
- **UnitMeasure**: catálogo simple con validación de ConversionFactor
- **Supplier**: catálogo simple
- **PurchaseProductDetail**: se gestiona en contexto de Purchase (ya tiene purchaseId)
- **SaleProductDetail**: se gestiona en contexto de Sale (ya tiene saleId)
- **CashMovement**: se gestiona en contexto de CashSession (ya tiene cashSessionId)
- **User, Rol, Person, Permission, Form, Module**: ya implementado

### 🔧 Necesitan métodos específicos:

| Entidad | Métodos | Razón |
|---------|---------|-------|
| **Product** | 4 métodos | Stock bajo, por categoría (nombre), ajustes, conversiones |
| **ProductUnitPrice** | 2 métodos | Por producto (nombre), consulta por nombres |
| **Purchase** | 5 métodos | Recibir (crítico), cancelar, reportes |
| **Sale** | 7 métodos | Finalizar (crítico), recalcular, reportes |
| **CashSession** | 5 métodos | Abrir/cerrar (críticos), sesión abierta, balance |

---

## 🎯 MÉTODOS MÁS CRÍTICOS (prioridad alta)

1. **Sale.FinalizeSaleAsync** - Flujo completo de venta con validaciones
2. **Purchase.ReceivePurchaseAsync** - Flujo de entrada de inventario
3. **CashSession.OpenSessionAsync / CloseSessionAsync** - Control de turno
4. **Product.GetLowStockProductsAsync** - KPI de reposición
5. **ProductUnitPrice.GetByProductNameAsync / GetPriceByNamesAsync** - UX POS con nombres

---

## 📝 NOTAS FINALES

### Principios de diseño:

1. **Frontend trabaja con NOMBRES, no IDs**: El usuario nunca ve IDs, por eso los métodos de búsqueda son por nombre (productName, categoryName, etc.)

2. **Data vs Business**:
   - **Data**: consultas simples por nombre/filtros (GetByNameAsync, GetByCategoryNameAsync)
   - **Business**: lógica compleja, validaciones, cálculos, transacciones (FinalizeSaleAsync, ReceivePurchaseAsync)

3. **Entidades pivote/detalle**: PurchaseProductDetail, SaleProductDetail, CashMovement solo necesitan CRUD porque siempre se trabaja en contexto de su entidad padre (Purchase, Sale, CashSession) que ya tiene el ID

4. **Transacciones**: Todos los métodos que modifican múltiples entidades deben usar **DbContext.BeginTransaction**

5. **Validaciones críticas**: Implementar en Business según punto 4 del documento de operación

### Búsquedas por nombre vs ID:

**Por NOMBRE (desde frontend - usuario selecciona):**
- ✅ `GetByCategoryNameAsync` - usuario filtra por categoría
- ✅ `GetByProductNameAsync` - usuario busca producto
- ✅ `GetPriceByNamesAsync` - usuario selecciona producto + presentación
- ✅ `GetBySupplierNameAsync` - usuario filtra compras por proveedor
- ✅ `AdjustStockAsync(productName, ...)` - usuario ajusta inventario de un producto
- ✅ `GetStockByPresentationsAsync(productName)` - usuario consulta stock de un producto

**Por ID (uso interno - entidad ya creada en flujo):**
- ✅ `FinalizeSaleAsync(saleId)` - la venta ya existe en Draft con su ID
- ✅ `CancelSaleAsync(saleId)` - la venta ya existe
- ✅ `RecalculateTotalsAsync(saleId)` - la venta ya existe
- ✅ `ReceivePurchaseAsync(purchaseId, ...)` - la compra ya fue creada
- ✅ `CancelPurchaseAsync(purchaseId, ...)` - la compra ya fue creada
- ✅ `CloseSessionAsync(sessionId, ...)` - la sesión ya está abierta
- ✅ `GetSessionBalanceAsync(sessionId)` - consultar sesión específica ya abierta
- ✅ `GetByCashSessionAsync(cashSessionId)` - filtrar ventas de una sesión ya abierta
- ✅ `GetByIdAsync` (heredado de BaseData) - recuperar entidad específica en Business

**Regla general:**
- **NOMBRE**: cuando el usuario SELECCIONA/BUSCA algo desde cero (catálogos, filtros)
- **ID**: cuando la entidad YA FUE CREADA en el flujo y tenemos su identificador

---

**Fecha:** 2025-11-03
**Proyecto:** EstancoPro - Sistema de Administración de Estanco
