# 🎨 Paleta de Colores EstancoPro - Temática Verde Natural

## 🌿 Concepto de Diseño

**EstancoPro** utiliza una paleta inspirada en tonos **verdes naturales** que evocan:
- Frescura y vitalidad
- Confianza y estabilidad
- Naturaleza y productos orgánicos
- Ambiente acogedor de un estanco tradicional

---

## 🎨 Paleta Completa

### 🟢 Colores Primarios (Verdes)

```css
/* Verde Bosque - Principal */
--primary-dark: #1B3A1B;          /* Títulos, textos importantes */
--primary: #2D5A2D;                /* Sidebar, header, fondos principales */
--primary-light: #3D7A3D;          /* Hover states, elementos activos */
--primary-lighter: #4A9A4A;        /* Botones secundarios, badges */

/* Verde Acento - Acciones */
--accent-lime: #7CB342;            /* Botones principales, CTAs */
--accent-sage: #8BC34A;            /* Links, elementos interactivos */
--accent-mint: #A5D6A7;            /* Badges suaves, tags */
--accent-light: #C8E6C9;           /* Bordes, separadores */

/* Verde Fondo */
--background-green: #F1F8F4;       /* Fondo general de la app */
--background-green-light: #F8FBF9; /* Fondos alternos */
```

### ⚪ Neutrales

```css
/* Blancos y Grises */
--white: #FFFFFF;                  /* Blanco puro - cards, modals */
--gray-50: #FAFAFA;                /* Gris muy claro */
--gray-100: #F5F5F5;               /* Fondos alternos */
--gray-200: #EEEEEE;               /* Bordes suaves */
--gray-300: #E0E0E0;               /* Divisores */
--gray-400: #BDBDBD;               /* Iconos deshabilitados */
--gray-500: #9E9E9E;               /* Texto placeholder */
--gray-600: #757575;               /* Texto secundario */
--gray-700: #616161;               /* Texto normal */
--gray-800: #424242;               /* Texto enfatizado */
--gray-900: #212121;               /* Texto principal (alternativa) */
```

### 🔴 Colores de Estado

```css
/* Éxito (Verde) */
--success: #43A047;                /* Mensajes de éxito */
--success-light: #66BB6A;          /* Fondo de alertas éxito */
--success-bg: #E8F5E9;             /* Fondo suave */

/* Advertencia (Amarillo/Naranja) */
--warning: #FBC02D;                /* Alertas de advertencia */
--warning-light: #FDD835;          /* Fondo de alertas */
--warning-bg: #FFFDE7;             /* Fondo suave */

/* Error (Rojo) */
--error: #E53935;                  /* Mensajes de error */
--error-light: #EF5350;            /* Botones de eliminar */
--error-bg: #FFEBEE;               /* Fondo suave */

/* Información (Azul) */
--info: #039BE5;                   /* Mensajes informativos */
--info-light: #29B6F6;             /* Fondo de alertas info */
--info-bg: #E1F5FE;                /* Fondo suave */
```

### 💰 Colores Financieros

```css
/* Dinero/Ingresos (Verde brillante) */
--cash-green: #4CAF50;             /* Ingresos, ventas */
--cash-green-light: #81C784;       /* Gráficos de ingresos */
--cash-green-bg: #E8F5E9;          /* Fondos de totales positivos */

/* Gastos/Egresos (Rojo suave) */
--expense-red: #EF5350;            /* Egresos, compras */
--expense-red-light: #E57373;      /* Gráficos de gastos */
--expense-red-bg: #FFEBEE;         /* Fondos de totales negativos */

/* Ganancias/Utilidad (Dorado) */
--profit-gold: #FFB300;            /* Utilidades, ganancias */
--profit-gold-light: #FFCA28;      /* Highlights de profit */
--profit-gold-bg: #FFF8E1;         /* Fondos de utilidades */
```

---

## 🖌️ Guía de Uso por Componente

### 1. 🧭 **Sidebar/Navigation**

```css
.sidebar {
  background: linear-gradient(180deg, #2D5A2D 0%, #1B3A1B 100%);
  color: #FFFFFF;
  border-right: 1px solid #3D7A3D;
}

.sidebar-item {
  color: #C8E6C9;
  transition: all 0.3s ease;
}

.sidebar-item:hover {
  background-color: rgba(125, 179, 66, 0.2);
  color: #FFFFFF;
}

.sidebar-item.active {
  background-color: #3D7A3D;
  color: #FFFFFF;
  border-left: 4px solid #7CB342;
}

.sidebar-icon {
  color: #A5D6A7;
}
```

**Vista Previa:**
```
╔════════════════════════════════════╗
║ 🏪 EstancoPro     (#2D5A2D)      ║
╠════════════════════════════════════╣
║  📊 Dashboard     (#C8E6C9)      ║
║  💰 POS           (#C8E6C9)      ║ ← Normal
║ ▌🛒 Compras       (#FFFFFF)      ║ ← Activo (#3D7A3D + border)
║  📦 Inventario    (#C8E6C9)      ║
║  💵 Caja          (#C8E6C9)      ║
╚════════════════════════════════════╝
```

---

### 2. 🎛️ **Header/TopBar**

```css
.header {
  background: #FFFFFF;
  border-bottom: 2px solid #C8E6C9;
  box-shadow: 0 2px 8px rgba(45, 90, 45, 0.1);
}

.header-title {
  color: #2D5A2D;
  font-weight: 600;
}

.header-user {
  color: #546E7A;
}

.header-badge {
  background-color: #7CB342;
  color: #FFFFFF;
  border-radius: 12px;
  padding: 2px 8px;
}
```

**Vista Previa:**
```
╔═══════════════════════════════════════════════════════════════╗
║  Punto de Venta (#2D5A2D)              Cajero: Juan  [3]     ║
║  Sesión: #25 | Apertura: $50,000       (#546E7A)   (#7CB342) ║
╚═══════════════════════════════════════════════════════════════╝
```

---

### 3. 🔘 **Botones**

```css
/* Botón Principal */
.btn-primary {
  background-color: #7CB342;
  color: #FFFFFF;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  font-weight: 500;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(124, 179, 66, 0.3);
}

.btn-primary:hover {
  background-color: #689F38;
  box-shadow: 0 4px 8px rgba(124, 179, 66, 0.4);
  transform: translateY(-1px);
}

.btn-primary:active {
  transform: translateY(0);
}

/* Botón Secundario */
.btn-secondary {
  background-color: #FFFFFF;
  color: #3D7A3D;
  border: 2px solid #C8E6C9;
  padding: 10px 20px;
  border-radius: 6px;
  transition: all 0.3s ease;
}

.btn-secondary:hover {
  background-color: #F1F8F4;
  border-color: #7CB342;
  color: #2D5A2D;
}

/* Botón Peligro */
.btn-danger {
  background-color: #EF5350;
  color: #FFFFFF;
  border: none;
}

.btn-danger:hover {
  background-color: #E53935;
}

/* Botón Éxito */
.btn-success {
  background-color: #43A047;
  color: #FFFFFF;
  border: none;
}

.btn-success:hover {
  background-color: #388E3C;
}
```

**Vista Previa:**
```
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│   COBRAR    │  │  CANCELAR   │  │  ELIMINAR   │
│  (#7CB342)  │  │  (#FFFFFF)  │  │  (#EF5350)  │
└─────────────┘  └─────────────┘  └─────────────┘
```

---

### 4. 🃏 **Cards/Paneles**

```css
.card {
  background-color: #FFFFFF;
  border: 1px solid #C8E6C9;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(45, 90, 45, 0.08);
  transition: all 0.3s ease;
}

.card:hover {
  box-shadow: 0 4px 16px rgba(45, 90, 45, 0.12);
  transform: translateY(-2px);
}

.card-header {
  background: linear-gradient(90deg, #3D7A3D 0%, #4A9A4A 100%);
  color: #FFFFFF;
  padding: 12px 20px;
  border-radius: 8px 8px 0 0;
  font-weight: 600;
}

.card-footer {
  background-color: #F1F8F4;
  border-top: 1px solid #C8E6C9;
  padding: 12px 20px;
  border-radius: 0 0 8px 8px;
}
```

**Vista Previa:**
```
┌───────────────────────────────────────┐
│  Producto: Coca-Cola 1.5L (#FFFFFF)  │ ← card-header (#3D7A3D)
├───────────────────────────────────────┤
│  Precio: $3,500                       │
│  Stock: 48 unidades                   │ ← card-body (#FFFFFF)
│  Categoría: Bebidas                   │
├───────────────────────────────────────┤
│  [Editar]  [Eliminar]                 │ ← card-footer (#F1F8F4)
└───────────────────────────────────────┘
```

---

### 5. 📊 **Dashboard Stats Cards**

```css
/* Card de Ventas */
.stat-card-sales {
  background: linear-gradient(135deg, #4CAF50 0%, #66BB6A 100%);
  color: #FFFFFF;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 4px 12px rgba(76, 175, 80, 0.3);
}

/* Card de Gastos */
.stat-card-expenses {
  background: linear-gradient(135deg, #EF5350 0%, #E57373 100%);
  color: #FFFFFF;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 4px 12px rgba(239, 83, 80, 0.3);
}

/* Card de Utilidad */
.stat-card-profit {
  background: linear-gradient(135deg, #FFB300 0%, #FFCA28 100%);
  color: #FFFFFF;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 4px 12px rgba(255, 179, 0, 0.3);
}

/* Card de Stock */
.stat-card-stock {
  background: linear-gradient(135deg, #039BE5 0%, #29B6F6 100%);
  color: #FFFFFF;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 4px 12px rgba(3, 155, 229, 0.3);
}
```

**Vista Previa:**
```
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│ 💰 VENTAS       │  │ 📤 GASTOS       │  │ 💎 UTILIDAD     │  │ 📦 STOCK        │
│ (#4CAF50)       │  │ (#EF5350)       │  │ (#FFB300)       │  │ (#039BE5)       │
│                 │  │                 │  │                 │  │                 │
│ $1,234,500      │  │ $456,000        │  │ $778,500        │  │ 348 productos   │
│ +12.5%          │  │ -5.3%           │  │ +18.2%          │  │ 23 bajos        │
└─────────────────┘  └─────────────────┘  └─────────────────┘  └─────────────────┘
```

---

### 6. 📋 **Tablas**

```css
.table {
  width: 100%;
  background-color: #FFFFFF;
  border: 1px solid #C8E6C9;
  border-radius: 8px;
  overflow: hidden;
}

.table-header {
  background-color: #3D7A3D;
  color: #FFFFFF;
  font-weight: 600;
  text-transform: uppercase;
  font-size: 12px;
  letter-spacing: 0.5px;
}

.table-header th {
  padding: 12px 16px;
  text-align: left;
  border-bottom: 2px solid #2D5A2D;
}

.table-row {
  border-bottom: 1px solid #C8E6C9;
  transition: background-color 0.2s ease;
}

.table-row:hover {
  background-color: #F1F8F4;
}

.table-row:nth-child(even) {
  background-color: #F8FBF9;
}

.table-row td {
  padding: 12px 16px;
  color: #424242;
}

.table-row-actions {
  display: flex;
  gap: 8px;
}
```

**Vista Previa:**
```
╔═══════════════════════════════════════════════════════════════╗
║ ID  │ PRODUCTO          │ STOCK  │ PRECIO     │ ACCIONES     ║ ← (#3D7A3D)
╠═══════════════════════════════════════════════════════════════╣
║ 001 │ Coca-Cola 1.5L    │ 48     │ $3,500     │ 📝 🗑️       ║
║ 002 │ Marlboro Rojo     │ 120    │ $5,000     │ 📝 🗑️       ║ ← (#F8FBF9)
║ 003 │ Agua Cristal      │ 32     │ $1,200     │ 📝 🗑️       ║
╚═══════════════════════════════════════════════════════════════╝
```

---

### 7. 🏷️ **Badges y Tags**

```css
/* Badge Éxito (Stock alto) */
.badge-success {
  background-color: #E8F5E9;
  color: #2E7D32;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

/* Badge Advertencia (Stock bajo) */
.badge-warning {
  background-color: #FFF8E1;
  color: #F57F17;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

/* Badge Error (Sin stock) */
.badge-error {
  background-color: #FFEBEE;
  color: #C62828;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

/* Badge Info */
.badge-info {
  background-color: #E1F5FE;
  color: #01579B;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

/* Badge Principal */
.badge-primary {
  background-color: #A5D6A7;
  color: #1B5E20;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}
```

**Vista Previa:**
```
[Stock Alto]    [Stock Bajo]    [Agotado]      [Activo]       [Cajero]
(#E8F5E9)       (#FFF8E1)       (#FFEBEE)      (#E1F5FE)      (#A5D6A7)
```

---

### 8. 📝 **Formularios e Inputs**

```css
/* Input Normal */
.input {
  width: 100%;
  padding: 10px 14px;
  border: 2px solid #C8E6C9;
  border-radius: 6px;
  background-color: #FFFFFF;
  color: #424242;
  font-size: 14px;
  transition: all 0.3s ease;
}

.input:focus {
  outline: none;
  border-color: #7CB342;
  box-shadow: 0 0 0 3px rgba(124, 179, 66, 0.1);
}

.input::placeholder {
  color: #9E9E9E;
}

/* Input con Error */
.input-error {
  border-color: #EF5350;
}

.input-error:focus {
  border-color: #E53935;
  box-shadow: 0 0 0 3px rgba(239, 83, 80, 0.1);
}

/* Label */
.label {
  display: block;
  margin-bottom: 6px;
  color: #2D5A2D;
  font-weight: 500;
  font-size: 14px;
}

/* Select */
.select {
  width: 100%;
  padding: 10px 14px;
  border: 2px solid #C8E6C9;
  border-radius: 6px;
  background-color: #FFFFFF;
  color: #424242;
  cursor: pointer;
}

.select:focus {
  outline: none;
  border-color: #7CB342;
}
```

---

### 9. 🔔 **Alertas y Notificaciones**

```css
/* Alerta Éxito */
.alert-success {
  background-color: #E8F5E9;
  border-left: 4px solid #43A047;
  color: #2E7D32;
  padding: 16px;
  border-radius: 6px;
  margin-bottom: 16px;
}

/* Alerta Advertencia */
.alert-warning {
  background-color: #FFFDE7;
  border-left: 4px solid #FBC02D;
  color: #F57F17;
  padding: 16px;
  border-radius: 6px;
  margin-bottom: 16px;
}

/* Alerta Error */
.alert-error {
  background-color: #FFEBEE;
  border-left: 4px solid #E53935;
  color: #C62828;
  padding: 16px;
  border-radius: 6px;
  margin-bottom: 16px;
}

/* Alerta Info */
.alert-info {
  background-color: #E1F5FE;
  border-left: 4px solid #039BE5;
  color: #01579B;
  padding: 16px;
  border-radius: 6px;
  margin-bottom: 16px;
}
```

**Vista Previa:**
```
┌────────────────────────────────────────────────┐
│ ✅ Venta finalizada exitosamente               │ (#E8F5E9)
└────────────────────────────────────────────────┘

┌────────────────────────────────────────────────┐
│ ⚠️ El stock del producto está bajo (5 unidades)│ (#FFFDE7)
└────────────────────────────────────────────────┘

┌────────────────────────────────────────────────┐
│ ❌ Error al procesar la venta                  │ (#FFEBEE)
└────────────────────────────────────────────────┘
```

---

## 🎨 Configuración Tailwind CSS

```javascript
// tailwind.config.js
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        // Primarios
        'estanco-dark': '#1B3A1B',
        'estanco': '#2D5A2D',
        'estanco-light': '#3D7A3D',
        'estanco-lighter': '#4A9A4A',

        // Acentos
        'lime': '#7CB342',
        'sage': '#8BC34A',
        'mint': '#A5D6A7',
        'border-green': '#C8E6C9',

        // Fondos
        'bg-green': '#F1F8F4',
        'bg-green-light': '#F8FBF9',

        // Estados
        'success': {
          DEFAULT: '#43A047',
          light: '#66BB6A',
          bg: '#E8F5E9',
        },
        'warning': {
          DEFAULT: '#FBC02D',
          light: '#FDD835',
          bg: '#FFFDE7',
        },
        'error': {
          DEFAULT: '#E53935',
          light: '#EF5350',
          bg: '#FFEBEE',
        },
        'info': {
          DEFAULT: '#039BE5',
          light: '#29B6F6',
          bg: '#E1F5FE',
        },

        // Financieros
        'cash': {
          green: '#4CAF50',
          'green-light': '#81C784',
          'green-bg': '#E8F5E9',
        },
        'expense': {
          red: '#EF5350',
          'red-light': '#E57373',
          'red-bg': '#FFEBEE',
        },
        'profit': {
          gold: '#FFB300',
          'gold-light': '#FFCA28',
          'gold-bg': '#FFF8E1',
        },
      },
      boxShadow: {
        'green-sm': '0 2px 4px rgba(45, 90, 45, 0.1)',
        'green-md': '0 4px 8px rgba(45, 90, 45, 0.12)',
        'green-lg': '0 8px 16px rgba(45, 90, 45, 0.15)',
        'lime': '0 4px 12px rgba(124, 179, 66, 0.3)',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        mono: ['Fira Code', 'Courier New', 'monospace'],
      },
    },
  },
  plugins: [],
}
```

---

## 📱 Ejemplos de Uso en Angular

### Ejemplo 1: Card de Producto

```html
<!-- product-card.component.html -->
<div class="bg-white border border-border-green rounded-lg p-5 shadow-green-sm hover:shadow-green-md transition-all duration-300">
  <div class="flex justify-between items-start mb-4">
    <h3 class="text-lg font-semibold text-estanco-dark">{{ product.name }}</h3>
    <span [ngClass]="{
      'badge-success': product.stockOnHand > product.reorderPoint,
      'badge-warning': product.stockOnHand <= product.reorderPoint && product.stockOnHand > 0,
      'badge-error': product.stockOnHand === 0
    }">
      {{ product.stockOnHand > product.reorderPoint ? 'Stock Alto' :
         product.stockOnHand > 0 ? 'Stock Bajo' : 'Agotado' }}
    </span>
  </div>

  <div class="space-y-2 text-gray-700">
    <p><span class="font-medium">Precio:</span> {{ product.unitPrice | currency }}</p>
    <p><span class="font-medium">Stock:</span> {{ product.stockOnHand }} unidades</p>
    <p><span class="font-medium">Categoría:</span> {{ product.category.name }}</p>
  </div>

  <div class="flex gap-2 mt-4 pt-4 border-t border-border-green">
    <button class="btn-primary">Editar</button>
    <button class="btn-danger">Eliminar</button>
  </div>
</div>
```

### Ejemplo 2: Stat Card de Dashboard

```html
<!-- dashboard-stats.component.html -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
  <!-- Ventas -->
  <div class="stat-card-sales">
    <div class="flex items-center justify-between mb-2">
      <span class="text-white/80 text-sm font-medium">VENTAS DEL DÍA</span>
      <svg class="w-8 h-8 text-white/60"><!-- icon --></svg>
    </div>
    <h2 class="text-3xl font-bold text-white mb-1">{{ sales | currency }}</h2>
    <p class="text-white/80 text-sm">+12.5% vs ayer</p>
  </div>

  <!-- Gastos -->
  <div class="stat-card-expenses">
    <div class="flex items-center justify-between mb-2">
      <span class="text-white/80 text-sm font-medium">GASTOS</span>
      <svg class="w-8 h-8 text-white/60"><!-- icon --></svg>
    </div>
    <h2 class="text-3xl font-bold text-white mb-1">{{ expenses | currency }}</h2>
    <p class="text-white/80 text-sm">-5.3% vs ayer</p>
  </div>

  <!-- Utilidad -->
  <div class="stat-card-profit">
    <div class="flex items-center justify-between mb-2">
      <span class="text-white/80 text-sm font-medium">UTILIDAD</span>
      <svg class="w-8 h-8 text-white/60"><!-- icon --></svg>
    </div>
    <h2 class="text-3xl font-bold text-white mb-1">{{ profit | currency }}</h2>
    <p class="text-white/80 text-sm">+18.2% vs ayer</p>
  </div>

  <!-- Stock -->
  <div class="stat-card-stock">
    <div class="flex items-center justify-between mb-2">
      <span class="text-white/80 text-sm font-medium">PRODUCTOS</span>
      <svg class="w-8 h-8 text-white/60"><!-- icon --></svg>
    </div>
    <h2 class="text-3xl font-bold text-white mb-1">{{ totalProducts }}</h2>
    <p class="text-white/80 text-sm">{{ lowStockCount }} con stock bajo</p>
  </div>
</div>
```

### Ejemplo 3: Formulario de Login

```html
<!-- login.component.html -->
<div class="min-h-screen bg-bg-green flex items-center justify-center p-4">
  <div class="bg-white rounded-lg shadow-green-lg p-8 w-full max-w-md">
    <!-- Logo -->
    <div class="text-center mb-8">
      <div class="inline-flex items-center justify-center w-16 h-16 bg-estanco rounded-full mb-4">
        <span class="text-3xl">🏪</span>
      </div>
      <h1 class="text-2xl font-bold text-estanco-dark">EstancoPro</h1>
      <p class="text-gray-600 text-sm mt-2">Ingresa para continuar</p>
    </div>

    <!-- Form -->
    <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
      <div class="mb-4">
        <label class="label">Email</label>
        <input type="email" formControlName="email" class="input" placeholder="usuario@ejemplo.com">
      </div>

      <div class="mb-6">
        <label class="label">Contraseña</label>
        <input type="password" formControlName="password" class="input" placeholder="••••••••">
      </div>

      <button type="submit" class="btn-primary w-full" [disabled]="!loginForm.valid">
        Iniciar Sesión
      </button>
    </form>
  </div>
</div>
```

---

## 🎨 Archivo CSS Global (styles.css)

```css
/* styles.css - Variables globales y clases de utilidad */

:root {
  /* Primarios */
  --primary-dark: #1B3A1B;
  --primary: #2D5A2D;
  --primary-light: #3D7A3D;
  --primary-lighter: #4A9A4A;

  /* Acentos */
  --accent-lime: #7CB342;
  --accent-sage: #8BC34A;
  --accent-mint: #A5D6A7;
  --border-green: #C8E6C9;

  /* Fondos */
  --bg-green: #F1F8F4;
  --bg-green-light: #F8FBF9;
  --white: #FFFFFF;

  /* Estados */
  --success: #43A047;
  --warning: #FBC02D;
  --error: #E53935;
  --info: #039BE5;

  /* Financieros */
  --cash-green: #4CAF50;
  --expense-red: #EF5350;
  --profit-gold: #FFB300;
}

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Inter', system-ui, -apple-system, sans-serif;
  background-color: var(--bg-green);
  color: #424242;
  line-height: 1.6;
}

/* Botones */
.btn-primary {
  background-color: var(--accent-lime);
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(124, 179, 66, 0.3);
}

.btn-primary:hover:not(:disabled) {
  background-color: #689F38;
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(124, 179, 66, 0.4);
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-secondary {
  background-color: white;
  color: var(--primary-light);
  border: 2px solid var(--border-green);
  padding: 10px 20px;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-secondary:hover {
  background-color: var(--bg-green);
  border-color: var(--accent-lime);
}

.btn-danger {
  background-color: var(--error);
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-danger:hover {
  background-color: #D32F2F;
}

/* Cards */
.card {
  background-color: white;
  border: 1px solid var(--border-green);
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(45, 90, 45, 0.08);
  transition: all 0.3s ease;
}

.card:hover {
  box-shadow: 0 4px 16px rgba(45, 90, 45, 0.12);
  transform: translateY(-2px);
}

/* Badges */
.badge-success {
  background-color: #E8F5E9;
  color: #2E7D32;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  display: inline-block;
}

.badge-warning {
  background-color: #FFF8E1;
  color: #F57F17;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  display: inline-block;
}

.badge-error {
  background-color: #FFEBEE;
  color: #C62828;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  display: inline-block;
}

/* Inputs */
.input {
  width: 100%;
  padding: 10px 14px;
  border: 2px solid var(--border-green);
  border-radius: 6px;
  background-color: white;
  color: #424242;
  font-size: 14px;
  transition: all 0.3s ease;
}

.input:focus {
  outline: none;
  border-color: var(--accent-lime);
  box-shadow: 0 0 0 3px rgba(124, 179, 66, 0.1);
}

.label {
  display: block;
  margin-bottom: 6px;
  color: var(--primary);
  font-weight: 500;
  font-size: 14px;
}

/* Alertas */
.alert-success {
  background-color: #E8F5E9;
  border-left: 4px solid var(--success);
  color: #2E7D32;
  padding: 16px;
  border-radius: 6px;
  margin-bottom: 16px;
}

.alert-warning {
  background-color: #FFFDE7;
  border-left: 4px solid var(--warning);
  color: #F57F17;
  padding: 16px;
  border-radius: 6px;
  margin-bottom: 16px;
}

.alert-error {
  background-color: #FFEBEE;
  border-left: 4px solid var(--error);
  color: #C62828;
  padding: 16px;
  border-radius: 6px;
  margin-bottom: 16px;
}
```

---

## ✅ Checklist de Implementación

### Fase 1: Configuración Base
- [ ] Instalar Tailwind CSS
- [ ] Configurar `tailwind.config.js` con paleta verde
- [ ] Crear `styles.css` con variables CSS
- [ ] Importar fuente Inter de Google Fonts

### Fase 2: Componentes Base
- [ ] Crear componente Sidebar con gradiente verde
- [ ] Crear componente Header/TopBar
- [ ] Crear componentes de botones (primary, secondary, danger)
- [ ] Crear componente Card base

### Fase 3: Componentes de Formulario
- [ ] Crear input component con estilos verdes
- [ ] Crear select component
- [ ] Crear checkbox/radio components
- [ ] Crear form validation styles

### Fase 4: Componentes de Feedback
- [ ] Crear alert components (success, warning, error, info)
- [ ] Crear badge components
- [ ] Crear toast notifications
- [ ] Crear loading spinners con color verde

### Fase 5: Dashboard
- [ ] Crear stat cards con gradientes
- [ ] Crear gráficos con colores de la paleta
- [ ] Crear tablas con hover verde claro
- [ ] Implementar responsive design

---

**Última actualización**: 2025-11-14
