# 🎨 Guía de Estilos - EstancoPro

## 📋 Resumen

Se han agregado estilos modernos y profesionales al proyecto EstancoPro usando Angular y SCSS. El sistema cuenta con un diseño completamente funcional y atractivo.

---

## 🎨 Sistema de Diseño

### Paleta de Colores

```scss
// Colores Principales
--color-primary: #0F4C5C      (Azul oscuro - Principal)
--color-primary-hover: #0a3643 (Hover del principal)
--color-primary-light: #1a6073 (Versión clara)

// Colores de Estado
--color-success: #2A9D8F       (Verde - Éxito)
--color-accent: #E9C46A        (Amarillo - Acento)
--color-error: #E63946         (Rojo - Error)
--color-warning: #f4a261       (Naranja - Advertencia)

// Colores de Fondo
--color-background: #F4F4F4    (Fondo general)
--color-surface: #FFFFFF       (Tarjetas y superficies)

// Colores de Texto
--color-text-primary: #1A1A1A   (Texto principal)
--color-text-secondary: #515151 (Texto secundario)
--color-text-muted: #757575     (Texto atenuado)
```

### Tipografía

```scss
// Fuentes
--font-ui: 'Inter'            (Interfaz de usuario)
--font-mono: 'Roboto Mono'    (Código y datos)

// Tamaños
--font-size-xs: 0.75rem       (12px)
--font-size-sm: 0.875rem      (14px)
--font-size-base: 1rem        (16px)
--font-size-lg: 1.125rem      (18px)
--font-size-xl: 1.25rem       (20px)
--font-size-2xl: 1.5rem       (24px)
--font-size-3xl: 1.875rem     (30px)

// Pesos
--font-weight-normal: 400
--font-weight-medium: 500
--font-weight-semibold: 600
--font-weight-bold: 700
```

### Espaciado

```scss
--space-1: 0.25rem     (4px)
--space-2: 0.5rem      (8px)
--space-3: 0.75rem     (12px)
--space-4: 1rem        (16px)
--space-6: 1.5rem      (24px)
--space-8: 2rem        (32px)
```

### Bordes y Sombras

```scss
// Radios
--radius-sm: 0.25rem   (4px)
--radius-md: 0.5rem    (8px)
--radius-lg: 0.75rem   (12px)

// Sombras
--shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05)
--shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1)
--shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1)
```

---

## 🧩 Componentes Estilizados

### 1. **Página de Login** ✅

#### Características:
- Fondo degradado atractivo
- Tarjeta centrada con sombra
- Logo animado
- Inputs con foco destacado
- Mensajes de error con iconos
- Diseño responsive

#### Vista Previa:
```
┌────────────────────────────────┐
│  Fondo degradado azul          │
│                                │
│  ┌──────────────────────────┐ │
│  │ 💎 Logo EstancoPro       │ │
│  │                          │ │
│  │ [ Email ]                │ │
│  │ [ Contraseña ]           │ │
│  │                          │ │
│  │ ❌ Error (si existe)     │ │
│  │                          │ │
│  │ [Iniciar Sesión]         │ │
│  └──────────────────────────┘ │
│                                │
└────────────────────────────────┘
```

### 2. **Dashboard Principal** ✅

#### Características:
- Header con información del usuario
- Tarjeta de bienvenida
- Indicadores de estado del sistema
- Lista de módulos disponibles
- Botón de cerrar sesión
- Responsive design

### 3. **Panel de Administración** ✅

#### Características:
- Header sticky con logo y navegación
- Tarjetas de estadísticas con iconos
- Grid responsive de cards
- Efectos hover animados
- Separación por secciones:
  - Gestión del Sistema
  - Inventario y Productos
  - Operaciones Comerciales
  - Reportes y Análisis

#### Vista Previa:
```
┌─────────────────────────────────────────────────────┐
│ Header: Logo | Panel de Administración | [Salir]   │
├─────────────────────────────────────────────────────┤
│                                                     │
│  ┌─────┐  ┌─────┐  ┌─────┐  ┌─────┐             │
│  │👤   │  │📦   │  │💰   │  │🏪   │             │
│  │Usr  │  │Prod │  │Vtas │  │Prov │             │
│  └─────┘  └─────┘  └─────┘  └─────┘             │
│                                                     │
│  🔧 Gestión del Sistema                            │
│  ┌──────────┐  ┌──────────┐                      │
│  │ Usuarios │  │  Roles   │                      │
│  └──────────┘  └──────────┘                      │
│                                                     │
│  📦 Inventario y Productos                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐       │
│  │Productos │  │Categorías│  │Unidades  │       │
│  └──────────┘  └──────────┘  └──────────┘       │
│                                                     │
└─────────────────────────────────────────────────────┘
```

---

## 📦 Clases Utilitarias Disponibles

### Contenedores
```html
<!-- Contenedor centrado con ancho máximo -->
<div class="container">...</div>

<!-- Contenedor de ancho completo -->
<div class="container-fluid">...</div>
```

### Grid System
```html
<!-- Grid de 2 columnas -->
<div class="grid grid-cols-2">
  <div>Columna 1</div>
  <div>Columna 2</div>
</div>

<!-- También disponibles: grid-cols-3, grid-cols-4 -->
```

### Flexbox
```html
<!-- Flex horizontal -->
<div class="flex items-center justify-between gap-4">
  <div>Izquierda</div>
  <div>Derecha</div>
</div>

<!-- Flex vertical -->
<div class="flex-col gap-2">
  <div>Arriba</div>
  <div>Abajo</div>
</div>
```

### Tarjetas (Cards)
```html
<!-- Tarjeta básica -->
<div class="card">
  <div class="card-header">
    <h3>Título</h3>
  </div>
  <div class="card-body">
    Contenido
  </div>
  <div class="card-footer">
    Pie
  </div>
</div>
```

### Badges (Etiquetas)
```html
<!-- Diferentes variantes -->
<span class="badge badge-primary">Primary</span>
<span class="badge badge-success">Success</span>
<span class="badge badge-warning">Warning</span>
<span class="badge badge-error">Error</span>
```

### Alertas
```html
<!-- Alerta de éxito -->
<div class="alert alert-success">
  ✅ Operación exitosa
</div>

<!-- Otras variantes: alert-info, alert-warning, alert-error -->
```

### Tablas
```html
<table class="table">
  <thead>
    <tr>
      <th>Columna 1</th>
      <th>Columna 2</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>Dato 1</td>
      <td>Dato 2</td>
    </tr>
  </tbody>
</table>
```

### Botones
```html
<!-- Botón primario -->
<button class="btn btn-primary">Primario</button>

<!-- Botón con borde -->
<button class="btn btn-outline">Outlined</button>

<!-- Tamaños -->
<button class="btn btn-sm">Pequeño</button>
<button class="btn btn-lg">Grande</button>
```

### Formularios
```html
<div class="form-group">
  <label class="form-label">Nombre</label>
  <input type="text" class="form-input" placeholder="Ingresa tu nombre">
</div>
```

### Espaciado
```html
<!-- Margin Top -->
<div class="mt-2">...</div>  <!-- 8px -->
<div class="mt-4">...</div>  <!-- 16px -->
<div class="mt-8">...</div>  <!-- 32px -->

<!-- Margin Bottom -->
<div class="mb-2">...</div>
<div class="mb-4">...</div>
<div class="mb-8">...</div>

<!-- Padding -->
<div class="p-4">...</div>   <!-- Padding de 16px -->
<div class="p-6">...</div>   <!-- Padding de 24px -->
```

### Texto
```html
<!-- Alineación -->
<div class="text-center">Centrado</div>
<div class="text-right">Derecha</div>

<!-- Colores -->
<span class="text-primary">Primario</span>
<span class="text-success">Éxito</span>
<span class="text-error">Error</span>
<span class="text-muted">Atenuado</span>

<!-- Peso -->
<span class="font-bold">Negrita</span>
<span class="font-semibold">Semi-negrita</span>
```

---

## 🎬 Animaciones

### Fade In
```html
<div class="fade-in">
  Este elemento aparece con animación
</div>
```

### Loading Spinner
```html
<div class="loading-spinner"></div>
```

---

## 📱 Diseño Responsive

### Breakpoints

```scss
// Móvil: por defecto
// Tablet: 768px
// Desktop: 1024px+

@media (max-width: 768px) {
  // Los grids se convierten en 1 columna
  .grid-cols-2,
  .grid-cols-3,
  .grid-cols-4 {
    grid-template-columns: 1fr;
  }
}
```

### Comportamiento Móvil

- **Login**: Tarjeta a ancho completo con padding reducido
- **Dashboard**: Stack vertical de elementos
- **Admin Panel**: Header en columna, grid de una sola columna

---

## 🎯 Mejores Prácticas

### 1. Uso de Variables CSS
Siempre usa las variables CSS definidas en lugar de valores hardcodeados:

```scss
// ✅ Correcto
color: var(--color-primary);
padding: var(--space-4);

// ❌ Incorrecto
color: #0F4C5C;
padding: 16px;
```

### 2. Spacing Consistente
Usa el sistema de espaciado predefinido:

```html
<!-- ✅ Correcto -->
<div class="mb-4">...</div>

<!-- ❌ Incorrecto -->
<div style="margin-bottom: 15px">...</div>
```

### 3. Componentes Reutilizables
Usa las clases utilitarias y componentes en lugar de estilos inline:

```html
<!-- ✅ Correcto -->
<div class="card p-6 mb-4">...</div>

<!-- ❌ Incorrecto -->
<div style="background: white; padding: 24px; margin-bottom: 16px">...</div>
```

---

## 🚀 Cómo Usar los Estilos

### 1. En Templates de Componentes
```typescript
@Component({
  template: `
    <div class="container">
      <div class="card">
        <h2 class="text-primary">Título</h2>
        <button class="btn btn-primary">Click</button>
      </div>
    </div>
  `
})
```

### 2. Con ngClass
```typescript
<div [ngClass]="{
  'card': true,
  'fade-in': isVisible,
  'p-4': isSmall,
  'p-8': !isSmall
}">
</div>
```

### 3. Estilos Scoped en Componentes
```typescript
@Component({
  styles: [`
    .my-component {
      background: var(--color-surface);
      padding: var(--space-4);
      border-radius: var(--radius-md);
    }
  `]
})
```

---

## ✨ Características Destacadas

### 1. **Modo Oscuro Ready**
Los estilos están preparados para soportar modo oscuro fácilmente modificando las variables CSS.

### 2. **Accesibilidad**
- Focus visible en todos los elementos interactivos
- Contraste de colores adecuado
- Tamaños de texto legibles

### 3. **Performance**
- Uso de CSS Grid y Flexbox nativo
- Transiciones y animaciones optimizadas
- Sin dependencias pesadas

### 4. **Consistencia**
- Sistema de diseño unificado
- Variables CSS reutilizables
- Nomenclatura predecible

---

## 🎨 Paleta Completa de Componentes

| Componente | Clases Principales | Uso |
|------------|-------------------|-----|
| **Contenedor** | `.container`, `.container-fluid` | Layout general |
| **Grid** | `.grid`, `.grid-cols-*` | Layouts en columnas |
| **Flex** | `.flex`, `.flex-col`, `.items-center` | Layouts flexibles |
| **Card** | `.card`, `.card-header`, `.card-body` | Contenedores de contenido |
| **Badge** | `.badge`, `.badge-primary` | Etiquetas y estados |
| **Alert** | `.alert`, `.alert-success` | Mensajes y notificaciones |
| **Table** | `.table` | Tablas de datos |
| **Button** | `.btn`, `.btn-primary` | Botones |
| **Form** | `.form-group`, `.form-input` | Formularios |

---

## 📖 Ejemplos de Uso Completos

### Página de Listado
```html
<div class="container">
  <div class="flex justify-between items-center mb-6">
    <h1 class="text-primary">Productos</h1>
    <button class="btn btn-primary">
      Nuevo Producto
    </button>
  </div>

  <div class="card">
    <table class="table">
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Precio</th>
          <th>Stock</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>Cerveza Poker</td>
          <td>$2,500</td>
          <td><span class="badge badge-success">100</span></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
```

### Formulario
```html
<div class="container">
  <div class="card p-8">
    <h2 class="text-primary mb-6">Nuevo Usuario</h2>

    <div class="form-group">
      <label class="form-label">Nombre Completo</label>
      <input type="text" class="form-input" placeholder="Juan Pérez">
    </div>

    <div class="form-group">
      <label class="form-label">Email</label>
      <input type="email" class="form-input" placeholder="juan@ejemplo.com">
    </div>

    <div class="flex gap-4 mt-6">
      <button class="btn btn-primary">Guardar</button>
      <button class="btn btn-outline">Cancelar</button>
    </div>
  </div>
</div>
```

### Dashboard con Stats
```html
<div class="container">
  <div class="grid grid-cols-4 gap-4 mb-8">
    <div class="card">
      <h3 class="text-muted">Ventas Hoy</h3>
      <p class="text-primary font-bold" style="font-size: 2rem">$125,000</p>
      <span class="badge badge-success">+15%</span>
    </div>
    <!-- Más stats... -->
  </div>

  <div class="card">
    <div class="card-header">
      <h2>Últimas Ventas</h2>
    </div>
    <div class="card-body">
      <!-- Contenido -->
    </div>
  </div>
</div>
```

---

## 🎉 Resultado Final

El sistema EstancoPro ahora cuenta con:

✅ **Login moderno** con gradientes y animaciones
✅ **Dashboard funcional** con información del usuario
✅ **Panel de administración** completo con navegación intuitiva
✅ **Sistema de diseño consistente** con variables CSS
✅ **Componentes reutilizables** listos para usar
✅ **Diseño responsive** que funciona en todos los dispositivos
✅ **Animaciones suaves** para mejor UX
✅ **Accesibilidad mejorada** con focus visible

¡El frontend está listo y se ve profesional! 🚀
