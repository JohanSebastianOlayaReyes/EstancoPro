# Endpoints del Backend Necesarios para los Dashboards

## Endpoints Actualmente Utilizados que Pueden Necesitar Implementación

### SaleService

#### 1. GET /api/Sale/by-user/{userId}
**Descripción**: Obtener ventas realizadas por un usuario específico
**Usado en**: Dashboard del Vendedor
**Request**:
- Path parameter: `userId` (number)
**Response**: `SaleDto[]`
```json
[
  {
    "id": 1,
    "soldAt": "2025-11-13T10:30:00",
    "status": "Finalized",
    "subtotal": 100,
    "taxTotal": 19,
    "grandTotal": 119,
    "cashSessionId": 5
  }
]
```

#### 2. GET /api/Sale/top-products?limit={limit}
**Descripción**: Obtener los productos más vendidos
**Usado en**: Dashboard del Administrador
**Request**:
- Query parameter: `limit` (number, default: 5)
**Response**: Array de productos más vendidos
```json
[
  {
    "productId": 1,
    "productName": "Coca Cola 2L",
    "totalQuantity": 150,
    "totalRevenue": 3000
  }
]
```

**NOTA**: Si estos endpoints no existen, el frontend actualmente maneja los errores y muestra datos vacíos. Los métodos `getTodaySales()` y `getMonthSales()` utilizan `getByDateRange()` que ya existe.

---

## Endpoints que Ya Deberían Existir (Verificar)

### CashSessionService

#### 1. GET /api/CashSession/open
**Descripción**: Obtener la sesión de caja abierta del usuario actual
**Usado en**: Dashboard del Cajero
**Response**: `CashSessionDto | null`

#### 2. POST /api/CashSession/open
**Descripción**: Abrir una nueva sesión de caja
**Usado en**: Dashboard del Cajero
**Request Body**:
```json
{
  "openingAmount": 100.00
}
```
**Response**: `CashSessionDto`

#### 3. POST /api/CashSession/{id}/close
**Descripción**: Cerrar sesión de caja
**Usado en**: Dashboard del Cajero
**Request Body**:
```json
{
  "closingAmount": 500.00
}
```
**Response**:
```json
{
  "message": "Caja cerrada exitosamente",
  "difference": -10.50,
  "status": "shortage"
}
```

---

## Endpoints Opcionales para Mejorar Funcionalidad

### 1. GET /api/Sale/by-user/{userId}/today
**Descripción**: Obtener ventas del día de un usuario específico
**Beneficio**: Evita tener que filtrar en el frontend
**Response**: `SaleDto[]`

### 2. GET /api/Sale/stats/today
**Descripción**: Estadísticas rápidas de ventas del día
**Beneficio**: Reduce carga en el frontend
**Response**:
```json
{
  "totalRevenue": 5000,
  "totalTransactions": 45,
  "averageTicket": 111.11,
  "topProducts": [...]
}
```

### 3. GET /api/Dashboard/admin
**Descripción**: Dashboard consolidado para administrador
**Beneficio**: Una sola llamada al backend
**Response**:
```json
{
  "todayRevenue": 5000,
  "monthRevenue": 150000,
  "productsCount": 250,
  "usersCount": 10,
  "suppliersCount": 5,
  "lowStockProducts": [...],
  "topProducts": [...],
  "salesByHour": [...]
}
```

### 4. GET /api/Dashboard/supervisor
**Descripción**: Dashboard consolidado para supervisor
**Response**:
```json
{
  "openSessions": [...],
  "todayStats": {...},
  "weekStats": {...},
  "alerts": [...]
}
```

---

## Notas de Implementación

### Seguridad
- Todos los endpoints deben validar que el usuario tiene permiso para acceder a la información
- Los endpoints `/by-user/{userId}` deben verificar que el usuario autenticado puede ver esa información
- El endpoint `/CashSession/open` debe retornar solo la sesión del usuario autenticado (no todas las sesiones abiertas)

### Performance
- Considerar implementar caché para estadísticas que no cambian con frecuencia
- Los endpoints de dashboard consolidados reducen el número de llamadas HTTP

### Formato de Fechas
- Todas las fechas deben usar formato ISO 8601 (YYYY-MM-DDTHH:mm:ss)
- El backend debe manejar zonas horarias correctamente

---

## Estado Actual

Los dashboards funcionarán con los endpoints existentes, pero con funcionalidad limitada:
- ✅ Dashboard del Administrador: Funciona con endpoints existentes
- ⚠️ Dashboard del Cajero: Requiere endpoints de CashSession (verificar implementación)
- ⚠️ Dashboard del Vendedor: Funciona pero sin filtrar por usuario (muestra todas las ventas)
- ✅ Dashboard del Supervisor: Funciona con endpoints existentes

### Endpoints Críticos a Implementar
1. `GET /api/CashSession/open` - Para el Dashboard del Cajero
2. `POST /api/CashSession/open` - Para abrir caja
3. `POST /api/CashSession/{id}/close` - Para cerrar caja
4. `GET /api/Sale/by-user/{userId}` - Para filtrar ventas por vendedor

### Endpoints Opcionales pero Recomendados
1. `GET /api/Sale/top-products` - Mejora Dashboard del Admin
2. `GET /api/Dashboard/admin` - Optimización de performance
3. `GET /api/Dashboard/supervisor` - Optimización de performance
