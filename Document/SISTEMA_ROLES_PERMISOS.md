# 🔐 Sistema de Roles y Permisos - EstancoPro

## 📋 Resumen

EstancoPro implementa un sistema de permisos **granular** basado en la combinación de:
- **Roles** (grupos de usuarios)
- **Formularios/Pantallas** (secciones del sistema)
- **Permisos** (acciones específicas)

Este sistema permite control preciso sobre qué puede hacer cada usuario en cada parte del sistema.

---

## 🏗️ Arquitectura del Sistema

### Diagrama de Relaciones

```
┌──────────┐         ┌──────────┐         ┌──────┐
│  User    │────N:N──│ UserRol  │────N:1──│ Rol  │
└──────────┘         └──────────┘         └──────┘
                                               │
                                               │ 1:N
                                               ▼
                                    ┌──────────────────────┐
                                    │ RolFormPermission    │
                                    └──────────────────────┘
                                         │          │
                                      1:N│          │1:N
                    ┌────────────────────┘          └────────────────┐
                    ▼                                                ▼
              ┌──────────┐                                    ┌────────────┐
              │   Form   │                                    │ Permission │
              └──────────┘                                    └────────────┘
                    │
                    │ N:N
                    ▼
              ┌──────────┐
              │  Module  │
              └──────────┘
```

---

## 📊 Entidades del Sistema

### 1. **User** (Usuario)
```csharp
public class User : Base
{
    public string Email { get; set; }           // Email único para login
    public string Password { get; set; }        // Hash BCrypt
    public int RolId { get; set; }              // Rol principal
    public int PersonId { get; set; }           // Datos personales
    public Rol rol { get; set; }
    public Person person { get; set; }
    public ICollection<UserRol> userrols { get; set; }
}
```

**Campos heredados de Base:**
- `Id` (PK)
- `Active` (bool)
- `CreateAt`, `UpdateAt`, `DeleteAt` (DateTime?)

### 2. **Rol** (Role)
```csharp
public class Rol : Base
{
    public string TypeRol { get; set; }         // Nombre del rol
    public string Description { get; set; }
    public ICollection<UserRol> userrols { get; set; }
    public ICollection<RolFormPermission> rolFormPermissions { get; set; }
}
```

**Ejemplos de TypeRol:**
- "Administrador"
- "Cajero"
- "Vendedor"
- "Inventario"
- "Gerente"

### 3. **Permission** (Permiso)
```csharp
public class Permission : Base
{
    public string TypePermission { get; set; }  // Acción permitida
    public string Description { get; set; }
    public ICollection<RolFormPermission> rolFormPermissions { get; set; }
}
```

**TypePermission estándar (CRUD):**
- `"Create"` - Crear registros
- `"Read"` - Ver/Consultar
- `"Update"` - Editar
- `"Delete"` - Eliminar

**Permisos especiales:**
- `"Execute"` - Ejecutar acciones especiales (ej: finalizar venta, cerrar caja)
- `"Export"` - Exportar reportes
- `"Print"` - Imprimir documentos
- `"Approve"` - Aprobar transacciones

### 4. **Form** (Formulario/Pantalla)
```csharp
public class Form : Base
{
    public string Name { get; set; }            // Nombre de la pantalla
    public string Description { get; set; }
    public string Path { get; set; }            // Ruta en el frontend
    public ICollection<FormModule> formModules { get; set; }
    public ICollection<RolFormPermission> rolFormPermissions { get; set; }
}
```

**Ejemplos:**
| Name | Path | Description |
|------|------|-------------|
| Dashboard | `/dashboard` | Pantalla principal |
| POS | `/pos` | Punto de venta |
| Productos | `/products` | Gestión de productos |
| Compras | `/purchases` | Gestión de compras |
| Caja | `/cash` | Control de caja |
| Usuarios | `/admin/users` | Gestión de usuarios |
| Roles | `/admin/roles` | Gestión de roles |

### 5. **Module** (Módulo)
```csharp
public class Module : Base
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }            // Icono para UI
    public int Order { get; set; }              // Orden en menú
    public ICollection<FormModule> formModules { get; set; }
}
```

**Ejemplos de módulos:**
- **Ventas** (Dashboard, POS, Reportes de ventas)
- **Inventario** (Productos, Categorías, Ajustes)
- **Compras** (Proveedores, Órdenes, Recepciones)
- **Caja** (Sesiones, Movimientos, Arqueos)
- **Administración** (Usuarios, Roles, Configuración)

### 6. **RolFormPermission** (Tabla Pivote Central)
```csharp
public class RolFormPermission : Base
{
    public int RolId { get; set; }
    public int FormId { get; set; }
    public int PermissionId { get; set; }
    public Rol rol { get; set; }
    public Form form { get; set; }
    public Permission permission { get; set; }
}
```

**Interpretación:**
> "El ROL X tiene el PERMISO Y en el FORMULARIO Z"

**Ejemplo:**
```
RolId=2 (Cajero) + FormId=5 (Productos) + PermissionId=2 (Read)
= "El Cajero puede VER productos"
```

---

## 🎭 Roles Predefinidos

### 1. 👑 **Administrador**

**Descripción:** Control total del sistema

**Permisos completos en:**
- ✅ Todos los formularios
- ✅ Todas las acciones (CRUD + Execute + Export)
- ✅ Gestión de usuarios y roles
- ✅ Configuración del sistema

**Matriz de Permisos:**
| Formulario | Create | Read | Update | Delete | Execute |
|------------|--------|------|--------|--------|---------|
| Dashboard | ✅ | ✅ | ✅ | ✅ | ✅ |
| POS | ✅ | ✅ | ✅ | ✅ | ✅ |
| Productos | ✅ | ✅ | ✅ | ✅ | ✅ |
| Compras | ✅ | ✅ | ✅ | ✅ | ✅ |
| Caja | ✅ | ✅ | ✅ | ✅ | ✅ |
| Usuarios | ✅ | ✅ | ✅ | ✅ | ❌ |
| Roles | ✅ | ✅ | ✅ | ✅ | ❌ |
| Reportes | ❌ | ✅ | ❌ | ❌ | ✅ |

---

### 2. 💰 **Cajero**

**Descripción:** Responsable de ventas y manejo de caja

**Puede:**
- ✅ Abrir/cerrar su sesión de caja
- ✅ Realizar ventas completas (POS)
- ✅ Ver productos (para vender)
- ✅ Ver su propio historial de ventas
- ✅ Ver balance de su caja actual
- ❌ NO puede modificar inventario
- ❌ NO puede ver sesiones de otros cajeros
- ❌ NO puede acceder a administración

**Matriz de Permisos:**
| Formulario | Create | Read | Update | Delete | Execute |
|------------|--------|------|--------|--------|---------|
| Dashboard | ❌ | ✅ | ❌ | ❌ | ❌ |
| POS | ✅ | ✅ | ✅ | ✅ | ✅ |
| Productos | ❌ | ✅ | ❌ | ❌ | ❌ |
| Compras | ❌ | ❌ | ❌ | ❌ | ❌ |
| Caja | ✅ | ✅ | ❌ | ❌ | ✅ |
| Usuarios | ❌ | ❌ | ❌ | ❌ | ❌ |
| Roles | ❌ | ❌ | ❌ | ❌ | ❌ |
| Reportes | ❌ | ✅* | ❌ | ❌ | ❌ |

*Solo sus propias ventas

**Acciones Execute permitidas:**
- `POST /api/Sale/{id}/finalize` - Finalizar venta
- `POST /api/CashSession/open` - Abrir caja
- `POST /api/CashSession/{id}/close` - Cerrar su caja

---

### 3. 🛒 **Vendedor**

**Descripción:** Realiza ventas pero NO maneja caja

**Puede:**
- ✅ Realizar ventas (si hay caja abierta)
- ✅ Ver productos
- ✅ Ver sus propias ventas
- ❌ NO puede abrir/cerrar caja
- ❌ NO puede ver movimientos de caja
- ❌ NO puede modificar inventario

**Matriz de Permisos:**
| Formulario | Create | Read | Update | Delete | Execute |
|------------|--------|------|--------|--------|---------|
| Dashboard | ❌ | ✅ | ❌ | ❌ | ❌ |
| POS | ✅ | ✅ | ✅ | ✅ | ✅* |
| Productos | ❌ | ✅ | ❌ | ❌ | ❌ |
| Compras | ❌ | ❌ | ❌ | ❌ | ❌ |
| Caja | ❌ | ❌ | ❌ | ❌ | ❌ |
| Usuarios | ❌ | ❌ | ❌ | ❌ | ❌ |
| Roles | ❌ | ❌ | ❌ | ❌ | ❌ |
| Reportes | ❌ | ✅* | ❌ | ❌ | ❌ |

*Solo puede finalizar ventas, no abrir/cerrar caja

---

### 4. 📦 **Inventario/Bodega**

**Descripción:** Gestiona productos y recibe compras

**Puede:**
- ✅ CRUD completo de productos
- ✅ CRUD de categorías y unidades
- ✅ Crear y recibir compras
- ✅ Ajustes de inventario
- ✅ Ver reportes de stock
- ❌ NO puede realizar ventas
- ❌ NO puede acceder a caja
- ❌ NO puede ver información financiera

**Matriz de Permisos:**
| Formulario | Create | Read | Update | Delete | Execute |
|------------|--------|------|--------|--------|---------|
| Dashboard | ❌ | ✅* | ❌ | ❌ | ❌ |
| POS | ❌ | ❌ | ❌ | ❌ | ❌ |
| Productos | ✅ | ✅ | ✅ | ✅ | ❌ |
| Categorías | ✅ | ✅ | ✅ | ✅ | ❌ |
| Compras | ✅ | ✅ | ✅ | ❌ | ✅ |
| Proveedores | ✅ | ✅ | ✅ | ✅ | ❌ |
| Caja | ❌ | ❌ | ❌ | ❌ | ❌ |
| Usuarios | ❌ | ❌ | ❌ | ❌ | ❌ |
| Reportes | ❌ | ✅* | ❌ | ❌ | ✅ |

*Solo dashboard de inventario y reportes de stock

**Acciones Execute permitidas:**
- `POST /api/Purchase/{id}/receive` - Recibir compra (actualiza stock)

---

### 5. 📊 **Gerente**

**Descripción:** Supervisión y análisis, sin modificar datos

**Puede:**
- ✅ Ver TODO (modo lectura)
- ✅ Acceder a todos los reportes
- ✅ Exportar información
- ✅ Ver todas las sesiones de caja
- ✅ Ver todas las ventas y compras
- ❌ NO puede crear/modificar/eliminar nada
- ❌ NO puede realizar ventas directamente
- ❌ NO puede abrir/cerrar caja

**Matriz de Permisos:**
| Formulario | Create | Read | Update | Delete | Execute | Export |
|------------|--------|------|--------|--------|---------|--------|
| Dashboard | ❌ | ✅ | ❌ | ❌ | ❌ | ✅ |
| POS | ❌ | ✅ | ❌ | ❌ | ❌ | ✅ |
| Productos | ❌ | ✅ | ❌ | ❌ | ❌ | ✅ |
| Compras | ❌ | ✅ | ❌ | ❌ | ❌ | ✅ |
| Proveedores | ❌ | ✅ | ❌ | ❌ | ❌ | ✅ |
| Caja | ❌ | ✅ | ❌ | ❌ | ❌ | ✅ |
| Usuarios | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Reportes | ❌ | ✅ | ❌ | ❌ | ✅ | ✅ |

---

## 🔧 Implementación en Backend

### Script SQL de Inicialización

```sql
-- 1. Crear Permisos
INSERT INTO permissions (TypePermission, Description, Active, CreateAt) VALUES
('Create', 'Crear nuevos registros', 1, GETDATE()),
('Read', 'Ver y consultar información', 1, GETDATE()),
('Update', 'Modificar registros existentes', 1, GETDATE()),
('Delete', 'Eliminar registros', 1, GETDATE()),
('Execute', 'Ejecutar acciones especiales', 1, GETDATE()),
('Export', 'Exportar información', 1, GETDATE());

-- 2. Crear Roles
INSERT INTO rols (TypeRol, Description, Active, CreateAt) VALUES
('Administrador', 'Control total del sistema', 1, GETDATE()),
('Cajero', 'Ventas y manejo de caja', 1, GETDATE()),
('Vendedor', 'Solo realiza ventas', 1, GETDATE()),
('Inventario', 'Gestión de productos y compras', 1, GETDATE()),
('Gerente', 'Supervisión y reportes', 1, GETDATE());

-- 3. Crear Módulos
INSERT INTO modules (Name, Description, Icon, [Order], Active, CreateAt) VALUES
('Ventas', 'Módulo de ventas y POS', 'shopping-cart', 1, 1, GETDATE()),
('Inventario', 'Gestión de productos', 'box', 2, 1, GETDATE()),
('Compras', 'Gestión de compras', 'truck', 3, 1, GETDATE()),
('Caja', 'Control de caja', 'dollar-sign', 4, 1, GETDATE()),
('Administración', 'Configuración del sistema', 'settings', 5, 1, GETDATE());

-- 4. Crear Formularios
INSERT INTO forms (Name, Description, Path, Active, CreateAt) VALUES
('Dashboard', 'Pantalla principal', '/dashboard', 1, GETDATE()),
('POS', 'Punto de venta', '/pos', 1, GETDATE()),
('Productos', 'Gestión de productos', '/products', 1, GETDATE()),
('Categorías', 'Gestión de categorías', '/categories', 1, GETDATE()),
('Compras', 'Gestión de compras', '/purchases', 1, GETDATE()),
('Proveedores', 'Gestión de proveedores', '/suppliers', 1, GETDATE()),
('Caja', 'Control de caja', '/cash', 1, GETDATE()),
('Usuarios', 'Gestión de usuarios', '/admin/users', 1, GETDATE()),
('Roles', 'Gestión de roles', '/admin/roles', 1, GETDATE()),
('Reportes', 'Reportes del sistema', '/reports', 1, GETDATE());

-- 5. Relacionar Forms con Modules
INSERT INTO form_modules (FormId, ModuleId, Active, CreateAt)
SELECT f.Id, m.Id, 1, GETDATE()
FROM forms f, modules m
WHERE
  (f.Name IN ('Dashboard', 'POS', 'Reportes') AND m.Name = 'Ventas')
  OR (f.Name IN ('Productos', 'Categorías') AND m.Name = 'Inventario')
  OR (f.Name IN ('Compras', 'Proveedores') AND m.Name = 'Compras')
  OR (f.Name = 'Caja' AND m.Name = 'Caja')
  OR (f.Name IN ('Usuarios', 'Roles') AND m.Name = 'Administración');

-- 6. Asignar permisos a ADMINISTRADOR (RolId=1)
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT 1, f.Id, p.Id, 1, GETDATE()
FROM forms f
CROSS JOIN permissions p;

-- 7. Asignar permisos a CAJERO (RolId=2)
-- Dashboard: Read
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT 2, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Dashboard' AND p.TypePermission = 'Read';

-- POS: Create, Read, Update, Delete, Execute
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT 2, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'POS' AND p.TypePermission IN ('Create', 'Read', 'Update', 'Delete', 'Execute');

-- Productos: Read
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT 2, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Productos' AND p.TypePermission = 'Read';

-- Caja: Create, Read, Execute
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT 2, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Caja' AND p.TypePermission IN ('Create', 'Read', 'Execute');

-- ... (continuar con otros roles)
```

### Middleware de Autorización (Opcional)

```csharp
// AuthorizationMiddleware.cs
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _context;

    public PermissionAuthorizationHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userId = context.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId)) return;

        var user = await _context.users
            .Include(u => u.rol)
                .ThenInclude(r => r.rolFormPermissions)
                    .ThenInclude(rfp => rfp.form)
            .Include(u => u.rol)
                .ThenInclude(r => r.rolFormPermissions)
                    .ThenInclude(rfp => rfp.permission)
            .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

        if (user == null) return;

        var hasPermission = user.rol.rolFormPermissions.Any(rfp =>
            rfp.form.Path == requirement.FormPath &&
            rfp.permission.TypePermission == requirement.PermissionType);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}

// Uso en controlador
[Authorize(Policy = "POS.Execute")]
[HttpPost("{id}/finalize")]
public async Task<IActionResult> FinalizeSale(int id) { ... }
```

---

## 💻 Implementación en Frontend (Angular)

### 1. **AuthGuard con Permisos**

```typescript
// permission.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class PermissionGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredPermission = route.data['permission'] as string;
    const formPath = route.data['form'] as string;

    if (!requiredPermission || !formPath) return true;

    const hasPermission = this.authService.hasPermission(formPath, requiredPermission);

    if (!hasPermission) {
      this.router.navigate(['/unauthorized']);
      return false;
    }

    return true;
  }
}
```

### 2. **AuthService con Gestión de Permisos**

```typescript
// auth.service.ts
export interface UserPermissions {
  form: string;
  permissions: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private permissionsCache: UserPermissions[] = [];

  login(credentials: LoginDto): Observable<LoginResponse> {
    return this.http.post<LoginResponse>('/api/Auth/login', credentials).pipe(
      tap(response => {
        localStorage.setItem('token', response.token);
        this.loadUserPermissions(response.userId);
      })
    );
  }

  private loadUserPermissions(userId: number): void {
    this.http.get<UserPermissions[]>(`/api/User/${userId}/permissions`).subscribe(
      permissions => {
        this.permissionsCache = permissions;
      }
    );
  }

  hasPermission(formPath: string, permission: string): boolean {
    const formPermissions = this.permissionsCache.find(p => p.form === formPath);
    return formPermissions?.permissions.includes(permission) ?? false;
  }

  canCreate(formPath: string): boolean {
    return this.hasPermission(formPath, 'Create');
  }

  canRead(formPath: string): boolean {
    return this.hasPermission(formPath, 'Read');
  }

  canUpdate(formPath: string): boolean {
    return this.hasPermission(formPath, 'Update');
  }

  canDelete(formPath: string): boolean {
    return this.hasPermission(formPath, 'Delete');
  }

  canExecute(formPath: string): boolean {
    return this.hasPermission(formPath, 'Execute');
  }
}
```

### 3. **Directiva Estructural para Permisos**

```typescript
// has-permission.directive.ts
import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Directive({
  selector: '[hasPermission]'
})
export class HasPermissionDirective {
  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authService: AuthService
  ) {}

  @Input() set hasPermission(value: { form: string; permission: string }) {
    if (this.authService.hasPermission(value.form, value.permission)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }
}
```

### 4. **Uso en Componentes**

```typescript
// products.component.html
<div class="product-list">
  <!-- Botón crear solo si tiene permiso -->
  <button
    *hasPermission="{ form: '/products', permission: 'Create' }"
    (click)="createProduct()">
    Nuevo Producto
  </button>

  <table>
    <tbody>
      <tr *ngFor="let product of products">
        <td>{{ product.name }}</td>

        <!-- Botón editar solo si tiene permiso -->
        <td *hasPermission="{ form: '/products', permission: 'Update' }">
          <button (click)="editProduct(product)">Editar</button>
        </td>

        <!-- Botón eliminar solo si tiene permiso -->
        <td *hasPermission="{ form: '/products', permission: 'Delete' }">
          <button (click)="deleteProduct(product)">Eliminar</button>
        </td>
      </tr>
    </tbody>
  </table>
</div>
```

### 5. **Configuración de Rutas**

```typescript
// app-routing.module.ts
const routes: Routes = [
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { form: '/dashboard', permission: 'Read' }
  },
  {
    path: 'pos',
    component: PosComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { form: '/pos', permission: 'Read' }
  },
  {
    path: 'products',
    component: ProductsComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { form: '/products', permission: 'Read' }
  },
  {
    path: 'admin/users',
    component: UsersComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { form: '/admin/users', permission: 'Read' }
  }
];
```

---

## 📱 Ejemplo de Flujo Completo

### Escenario: Cajero intenta finalizar una venta

```
1. Frontend
   ├─ Usuario "Juan" (Cajero) hace login
   ├─ Recibe token JWT con claim: "role": "Cajero"
   └─ Frontend carga permisos del cajero

2. Frontend - Pantalla POS
   ├─ *ngIf con hasPermission muestra botón [COBRAR]
   ├─ Usuario hace click en [COBRAR]
   └─ Llama: POST /api/Sale/123/finalize

3. Backend - SaleController
   ├─ [Authorize] valida que tenga token válido ✅
   ├─ (Opcional) [Authorize(Policy = "POS.Execute")] valida permiso específico ✅
   ├─ Ejecuta FinalizeSaleAsync()
   └─ Retorna 200 OK

4. Frontend
   ├─ Recibe confirmación
   ├─ Muestra mensaje de éxito
   └─ Imprime ticket
```

### Escenario: Vendedor intenta abrir caja

```
1. Frontend
   ├─ Usuario "María" (Vendedor) hace login
   └─ Frontend carga permisos del vendedor

2. Frontend - Sidebar
   ├─ Menú "Caja" NO se muestra (no tiene permiso Read en /cash)
   └─ Usuario no puede acceder

3. Si intenta URL directa: /cash
   ├─ PermissionGuard detecta falta de permiso
   ├─ Redirige a /unauthorized
   └─ Muestra: "No tienes permiso para acceder a esta sección"
```

---

## ✅ Checklist de Implementación

### Backend
- [ ] Crear script SQL para Permissions, Roles, Forms
- [ ] Poblar RolFormPermissions para cada rol
- [ ] Crear endpoint: `GET /api/User/{id}/permissions`
- [ ] (Opcional) Implementar PermissionAuthorizationHandler
- [ ] Agregar políticas de autorización en Startup.cs

### Frontend
- [ ] Crear AuthService con gestión de permisos
- [ ] Implementar PermissionGuard
- [ ] Crear directiva *hasPermission
- [ ] Configurar rutas con data: { form, permission }
- [ ] Ocultar botones según permisos
- [ ] Crear página /unauthorized

### Testing
- [ ] Probar acceso como Administrador (debe ver todo)
- [ ] Probar acceso como Cajero (solo POS y Caja)
- [ ] Probar acceso como Vendedor (solo POS)
- [ ] Probar acceso como Inventario (solo Productos y Compras)
- [ ] Probar acceso como Gerente (todo en modo lectura)
- [ ] Verificar que rutas protegidas redirigen correctamente

---

## 📞 Endpoints Necesarios (Faltantes)

```csharp
// UserController.cs - AGREGAR ESTE ENDPOINT
/// <summary>
/// Obtiene los permisos de un usuario agrupados por formulario
/// GET: api/User/{userId}/permissions
/// </summary>
[HttpGet("{userId:int}/permissions")]
public async Task<IActionResult> GetUserPermissions(int userId)
{
    var user = await _context.users
        .Include(u => u.rol)
            .ThenInclude(r => r.rolFormPermissions)
                .ThenInclude(rfp => rfp.form)
        .Include(u => u.rol)
            .ThenInclude(r => r.rolFormPermissions)
                .ThenInclude(rfp => rfp.permission)
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null) return NotFound();

    var permissions = user.rol.rolFormPermissions
        .GroupBy(rfp => rfp.form.Path)
        .Select(g => new
        {
            form = g.Key,
            permissions = g.Select(rfp => rfp.permission.TypePermission).ToList()
        })
        .ToList();

    return Ok(permissions);
}
```

**Respuesta esperada:**
```json
[
  {
    "form": "/dashboard",
    "permissions": ["Read"]
  },
  {
    "form": "/pos",
    "permissions": ["Create", "Read", "Update", "Delete", "Execute"]
  },
  {
    "form": "/products",
    "permissions": ["Read"]
  },
  {
    "form": "/cash",
    "permissions": ["Create", "Read", "Execute"]
  }
]
```

---

**Última actualización**: 2025-11-14
