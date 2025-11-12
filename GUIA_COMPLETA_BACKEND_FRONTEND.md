# 🚀 Guía Completa: Backend + Frontend - EstancoPro

## 📋 Índice
1. [Visión General del Sistema](#visión-general-del-sistema)
2. [Backend (ASP.NET Core)](#backend-aspnet-core)
3. [Frontend (Angular)](#frontend-angular)
4. [Comunicación Backend ↔ Frontend](#comunicación-backend--frontend)
5. [Flujo Completo de Autenticación](#flujo-completo-de-autenticación)
6. [Flujo de Operaciones CRUD](#flujo-de-operaciones-crud)
7. [Cómo Funciona en Tiempo Real](#cómo-funciona-en-tiempo-real)
8. [Ejecutar el Sistema Completo](#ejecutar-el-sistema-completo)

---

## 🎯 Visión General del Sistema

**EstancoPro** es un sistema completo de gestión de estancos que consta de dos partes principales:

```
┌─────────────────────────────────────────────────────────────┐
│                    SISTEMA ESTANCOPRO                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────────┐              ┌──────────────────┐    │
│  │   FRONTEND       │◄────HTTP────►│    BACKEND       │    │
│  │   Angular 20     │              │  ASP.NET Core 9  │    │
│  │  Port: 4200      │              │  Port: 5170      │    │
│  └──────────────────┘              └──────────────────┘    │
│          │                                  │               │
│          │                                  │               │
│   ┌──────▼──────┐                  ┌───────▼────────┐     │
│   │  Browser    │                  │  SQL Server    │     │
│   │ LocalStorage│                  │  Database      │     │
│   └─────────────┘                  └────────────────┘     │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Componentes del Sistema:

| Componente | Tecnología | Puerto | Función |
|------------|-----------|--------|---------|
| **Frontend** | Angular 20 | 4200 | Interfaz de usuario (UI/UX) |
| **Backend** | ASP.NET Core 9 | 5170 | API REST, Lógica de negocio |
| **Base de Datos** | SQL Server | - | Almacenamiento persistente |

---

## 🔧 Backend (ASP.NET Core)

### Arquitectura en Capas

```
Backend/
│
├── Web (Presentation Layer) ─────────┐
│   ├── Controllers/                  │  ← Endpoints de la API
│   │   ├── AuthController.cs         │     POST /api/auth/login
│   │   ├── UserController.cs         │     GET  /api/user
│   │   ├── ProductController.cs      │     GET  /api/product
│   │   └── ...                       │
│   ├── Services/                     │
│   │   └── DatabaseInitializer.cs    │  ← Inicialización automática
│   └── Program.cs                    │  ← Configuración principal
│                                      │
├── Business (Business Logic Layer) ──┤
│   ├── Interfaces/                   │  ← Interfaces de negocio
│   │   ├── IUserBusiness.cs          │
│   │   └── ...                       │
│   └── Implementations/              │  ← Lógica de negocio
│       ├── UserBusiness.cs           │
│       └── ...                       │
│                                      │
├── Data (Data Access Layer) ─────────┤
│   ├── Interfaces/                   │  ← Interfaces de datos
│   │   ├── IUserData.cs              │
│   │   └── ...                       │
│   └── Implementations/              │  ← Acceso a datos
│       ├── UserData.cs               │
│       └── ...                       │
│                                      │
├── Entity (Domain Layer) ────────────┤
│   ├── Model/                        │  ← Entidades de dominio
│   │   ├── User.cs                   │
│   │   ├── Product.cs                │
│   │   └── ...                       │
│   ├── Dto/                          │  ← Data Transfer Objects
│   │   ├── LoginDto.cs               │
│   │   └── ...                       │
│   ├── Context/                      │
│   │   └── ApplicationDbContext.cs   │  ← Configuración EF Core
│   └── Migrations/                   │  ← Migraciones de BD
│                                      │
└── Utilities ────────────────────────┘
    ├── Services/
    │   └── JwtService.cs             ← Generación de tokens JWT
    └── Mapper/
        └── AutoMapperProfile.cs      ← Mapeo de entidades a DTOs
```

### Flujo de una Petición Backend

```
1. Cliente HTTP ────────────────────────────────────────┐
                                                        │
2. Controller (AuthController)                          │
   ├─ Recibe petición HTTP POST /api/auth/login         │
   ├─ Valida los datos con ModelState                   │
   └─ Llama al Business Layer                           │
                                                        │
3. Business Layer (UserBusiness)                        │
   ├─ Aplica lógica de negocio                         │
   ├─ Validaciones adicionales                         │
   └─ Llama al Data Layer                              │
                                                        │
4. Data Layer (UserData)                                │
   ├─ Construye queries con LINQ                       │
   ├─ Ejecuta consultas a BD                           │
   └─ Retorna entidades                                │
                                                        │
5. Entity Framework Core                                │
   ├─ Traduce LINQ a SQL                               │
   ├─ Ejecuta SQL en la base de datos                  │
   └─ Mapea resultados a entidades                     │
                                                        │
6. SQL Server Database                                  │
   ├─ Ejecuta query SQL                                │
   └─ Retorna resultados                               │
                                                        │
7. Response ◄───────────────────────────────────────────┘
   └─ JSON con los datos solicitados
```

### Inicialización Automática del Backend

**¿Cuándo ocurre?**
Al ejecutar `dotnet run`, **antes** de que la API esté disponible.

**¿Dónde está el código?**
`Backend/Web/Program.cs` (líneas 153-177):

```csharp
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    // 1️⃣ APLICAR MIGRACIONES
    logger.LogInformation("Aplicando migraciones pendientes...");
    context.Database.Migrate();  // ← Crea/actualiza tablas automáticamente

    // 2️⃣ INICIALIZAR DATOS
    logger.LogInformation("Inicializando datos del sistema...");
    var initializer = new DatabaseInitializer(context, ...);
    await initializer.InitializeAsync();  // ← Crea usuarios de prueba
    logger.LogInformation("Inicialización completada.");
}
```

**¿Qué hace `DatabaseInitializer`?**
`Backend/Web/Services/DatabaseInitializer.cs`:

```csharp
public async Task InitializeAsync()
{
    // 1. Crear rol Administrador si no existe
    var adminRole = await _context.rols.FirstOrDefaultAsync(r => r.TypeRol == "Administrador");
    if (adminRole == null)
    {
        adminRole = new Rol { TypeRol = "Administrador", ... };
        _context.rols.Add(adminRole);
        await _context.SaveChangesAsync();
    }

    // 2. Crear usuario admin si no existe
    var adminUser = await _context.users.FirstOrDefaultAsync(u => u.Email == "admin@estancopro.com");
    if (adminUser == null)
    {
        // Crear persona
        var adminPerson = new Person { FullName = "Admin", ... };
        _context.persons.Add(adminPerson);
        await _context.SaveChangesAsync();

        // Crear usuario con contraseña hasheada
        adminUser = new User
        {
            Email = "admin@estancopro.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),  // ← Contraseña segura
            PersonId = adminPerson.Id,
            RolId = adminRole.Id
        };
        _context.users.Add(adminUser);
        await _context.SaveChangesAsync();
    }

    // 3. Crear roles adicionales (Cajero, Vendedor, Supervisor)
    await SeedRolesAndUsersAsync();

    // 4. Crear datos de prueba (categorías, productos, etc.)
    await SeedTestDataAsync();
}
```

### Endpoints Principales del Backend

| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Iniciar sesión | ❌ No |
| POST | `/api/auth/refresh` | Renovar token | ❌ No |
| POST | `/api/auth/logout` | Cerrar sesión | ✅ Sí |
| GET | `/api/user` | Listar usuarios | ✅ Sí |
| GET | `/api/product` | Listar productos | ✅ Sí |
| POST | `/api/sale` | Registrar venta | ✅ Sí |
| GET | `/api/cashsession` | Sesiones de caja | ✅ Sí |

---

## 🎨 Frontend (Angular)

### Arquitectura del Frontend

```
Frontend/src/app/
│
├── core/ ──────────────────────────┐
│   ├── services/                   │  ← Servicios HTTP
│   │   ├── auth.service.ts         │     - Login/Logout
│   │   ├── api.service.ts          │     - Cliente HTTP base
│   │   ├── user.service.ts         │     - CRUD usuarios
│   │   ├── product.service.ts      │     - CRUD productos
│   │   ├── sale.service.ts         │     - Ventas
│   │   └── ...                     │
│   ├── interceptors/               │
│   │   └── auth.interceptor.ts     │  ← Agrega token a peticiones
│   ├── guards/                     │
│   │   └── auth.guard.ts           │  ← Protege rutas
│   └── models/                     │  ← Interfaces TypeScript
│       ├── auth.model.ts           │
│       ├── user.model.ts           │
│       └── ...                     │
│                                    │
├── features/ ──────────────────────┤
│   ├── auth/                       │  ← Módulo de autenticación
│   │   └── pages/                  │
│   │       └── login.component.ts  │     Página de login
│   ├── dashboard/                  │  ← Dashboard principal
│   │   └── dashboard.component.ts  │
│   ├── admin/                      │  ← Administración
│   │   ├── admin-users.component.ts│     Gestión de usuarios
│   │   ├── admin-roles.component.ts│     Gestión de roles
│   │   ├── admin-products.component.ts│  Gestión de productos
│   │   ├── admin-categories.component.ts│
│   │   ├── admin-suppliers.component.ts│
│   │   └── ...                     │
│   ├── sales/                      │  ← Punto de venta
│   │   ├── pos.component.ts        │     POS (Point of Sale)
│   │   └── sales-list.component.ts │     Historial de ventas
│   ├── purchases/                  │  ← Compras
│   │   └── purchases.component.ts  │
│   └── cash/                       │  ← Control de caja
│       └── cash-sessions.component.ts│
│                                    │
├── shared/ ────────────────────────┤
│   └── components/                 │  ← Componentes reutilizables
│       ├── button.component.ts     │
│       ├── input.component.ts      │
│       └── icon.component.ts       │
│                                    │
├── app.routes.ts ──────────────────┤  ← Configuración de rutas
└── app.config.ts ──────────────────┘  ← Configuración de la app
```

### Servicios HTTP del Frontend

#### 1. **ApiService** (Cliente HTTP Base)
`Frontend/src/app/core/services/api.service.ts`:

```typescript
@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;  // http://localhost:5170/api

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}/${endpoint}`);
  }

  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${endpoint}`, data);
  }

  put<T>(endpoint: string, data: any): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${endpoint}`, data);
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}/${endpoint}`);
  }
}
```

#### 2. **AuthService** (Autenticación)
`Frontend/src/app/core/services/auth.service.ts`:

```typescript
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Signals para manejo reactivo del estado
  isAuthenticated = signal<boolean>(this.hasToken());
  currentUser = signal<any>(this.getUserFromStorage());

  login(credentials: LoginDto): Observable<LoginResponseDto> {
    return this.apiService.post<LoginResponseDto>('Auth/login', credentials).pipe(
      tap(response => {
        // Guardar token en localStorage
        localStorage.setItem('access_token', response.token);
        localStorage.setItem('refresh_token', response.refreshToken);

        // Actualizar estado
        this.isAuthenticated.set(true);
        this.currentUser.set({
          userId: response.userId,
          email: response.email,
          roleName: response.roleName
        });
      })
    );
  }

  logout(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }
}
```

#### 3. **Interceptor HTTP** (Agregar Token Automáticamente)
`Frontend/src/app/core/interceptors/auth.interceptor.ts`:

```typescript
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  // Si hay token, agregarlo al header Authorization
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};
```

### Flujo de una Petición Frontend

```
1. Usuario hace clic en "Listar Productos"
   │
2. Componente (admin-products.component.ts)
   ├─ Llama: this.productService.getAll()
   │
3. Servicio (product.service.ts)
   ├─ Llama: this.apiService.get<Product[]>('Product')
   │
4. ApiService
   ├─ Construye URL: http://localhost:5170/api/Product
   ├─ HttpClient hace petición GET
   │
5. Interceptor (auth.interceptor.ts)
   ├─ Intercepta la petición
   ├─ Obtiene token de localStorage
   ├─ Agrega header: Authorization: Bearer eyJhbGc...
   ├─ Envía petición modificada
   │
6. Backend recibe petición ──────────────►
   ├─ Middleware JWT valida el token
   ├─ ProductController.GetAll()
   ├─ ProductBusiness.GetAll()
   ├─ ProductData.GetAll()
   ├─ EF Core → SQL Server
   │
7. Backend retorna JSON ◄─────────────────
   {
     "id": 1,
     "name": "Cerveza Poker",
     "unitPrice": 2500,
     ...
   }
   │
8. Frontend recibe respuesta
   ├─ Observable emite los datos
   ├─ Componente actualiza la vista
   │
9. Usuario ve la lista de productos en pantalla
```

---

## 🔗 Comunicación Backend ↔ Frontend

### Configuración de Puertos

**Backend:**
- **Puerto:** 5170
- **URL Base:** `http://localhost:5170`
- **Swagger:** `http://localhost:5170/swagger`

**Frontend:**
- **Puerto:** 4200
- **URL Base:** `http://localhost:4200`

**Configuración en Frontend:**
`Frontend/src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5170/api'  // ← Apunta al backend
};
```

### CORS (Cross-Origin Resource Sharing)

**¿Por qué es necesario?**
El frontend (puerto 4200) hace peticiones al backend (puerto 5170), esto se considera "cross-origin" y por defecto está bloqueado por el navegador.

**Solución en el Backend:**
`Backend/Web/Program.cs` (líneas 56-63):

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()      // ← Permite cualquier origen
            .AllowAnyMethod()      // ← Permite GET, POST, PUT, DELETE
            .AllowAnyHeader());    // ← Permite cualquier header
});

// Más adelante...
app.UseCors("AllowAll");  // ← Aplica la política de CORS
```

### Formato de Datos (JSON)

**Petición del Frontend al Backend:**
```http
POST http://localhost:5170/api/auth/login
Content-Type: application/json

{
  "email": "admin@estancopro.com",
  "password": "Admin123!"
}
```

**Respuesta del Backend al Frontend:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0...",
  "email": "admin@estancopro.com",
  "roleName": "Administrador",
  "userId": 1,
  "expiresAt": "2025-11-10T00:47:00Z"
}
```

---

## 🔐 Flujo Completo de Autenticación

### 1. Login (Frontend → Backend)

```
┌─────────────────────────────────────────────────────────────────────┐
│                         FLUJO DE LOGIN                               │
└─────────────────────────────────────────────────────────────────────┘

👤 Usuario
  │
  │ 1. Ingresa email y password en el formulario
  │    - Email: admin@estancopro.com
  │    - Password: Admin123!
  │
  ▼
📱 Login Component (login.component.ts)
  │
  │ 2. Llama: this.authService.login(credentials)
  │
  ▼
🔧 AuthService (auth.service.ts)
  │
  │ 3. POST /api/auth/login
  │    Body: { email, password }
  │
  ▼
🌐 HTTP Request
  │
  │ 4. Viaja por la red...
  │
  ▼
🖥️ Backend - AuthController.Login()
  │
  │ 5. Busca usuario en BD por email
  │    SELECT * FROM users WHERE email = 'admin@estancopro.com'
  │
  │ 6. Verifica contraseña con BCrypt
  │    BCrypt.Verify("Admin123!", hash_guardado_en_bd)
  │    ✅ Contraseña correcta
  │
  │ 7. Genera JWT Token
  │    - userId: 1
  │    - email: admin@estancopro.com
  │    - role: Administrador
  │    - Expira en: 1 hora
  │
  │ 8. Genera Refresh Token
  │    - Token aleatorio de 64 caracteres
  │    - Expira en: 7 días
  │    - Guarda en tabla refresh_tokens
  │
  │ 9. Retorna respuesta JSON
  │
  ▼
📦 Response JSON
  {
    "token": "eyJhbGc...",
    "refreshToken": "a1b2c3...",
    "email": "admin@estancopro.com",
    "roleName": "Administrador",
    "userId": 1
  }
  │
  ▼
🔧 AuthService (auth.service.ts)
  │
  │ 10. Guarda en localStorage:
  │     - access_token: eyJhbGc...
  │     - refresh_token: a1b2c3...
  │
  │ 11. Actualiza estado:
  │     - isAuthenticated.set(true)
  │     - currentUser.set({ userId, email, roleName })
  │
  ▼
📱 Login Component
  │
  │ 12. Redirige al dashboard
  │     this.router.navigate(['/dashboard'])
  │
  ▼
🎉 Usuario autenticado y en el dashboard
```

### 2. Petición Autenticada (Frontend → Backend)

```
┌─────────────────────────────────────────────────────────────────────┐
│                    PETICIÓN AUTENTICADA                              │
└─────────────────────────────────────────────────────────────────────┘

👤 Usuario
  │
  │ 1. Hace clic en "Productos"
  │
  ▼
📱 Products Component
  │
  │ 2. Llama: this.productService.getAll()
  │
  ▼
🔧 ProductService
  │
  │ 3. GET /api/product
  │
  ▼
🛡️ Auth Interceptor
  │
  │ 4. Intercepta la petición
  │ 5. Obtiene token: localStorage.getItem('access_token')
  │ 6. Agrega header:
  │    Authorization: Bearer eyJhbGc...
  │
  ▼
🌐 HTTP Request con Token
  │
  │ GET http://localhost:5170/api/product
  │ Headers:
  │   Authorization: Bearer eyJhbGc...
  │
  ▼
🖥️ Backend - JWT Middleware
  │
  │ 7. Valida el token JWT:
  │    ✅ Firma válida
  │    ✅ No expirado
  │    ✅ Issuer correcto
  │    ✅ Audience correcto
  │
  │ 8. Extrae información del token:
  │    - userId: 1
  │    - email: admin@estancopro.com
  │    - role: Administrador
  │
  ▼
🖥️ Backend - ProductController.GetAll()
  │
  │ 9. Usuario autenticado ✅
  │ 10. Llama al Business Layer
  │ 11. Obtiene productos de la BD
  │
  ▼
📦 Response JSON
  [
    { "id": 1, "name": "Cerveza Poker", ... },
    { "id": 2, "name": "Aguila", ... },
    ...
  ]
  │
  ▼
📱 Products Component
  │
  │ 12. Recibe los datos
  │ 13. Actualiza la vista
  │
  ▼
🎉 Usuario ve la lista de productos
```

### 3. Token Expirado - Refresh Token

```
┌─────────────────────────────────────────────────────────────────────┐
│                    RENOVACIÓN DE TOKEN                               │
└─────────────────────────────────────────────────────────────────────┘

👤 Usuario
  │
  │ 1. Hace una petición después de 1 hora
  │
  ▼
🛡️ Auth Interceptor
  │
  │ 2. Envía petición con token expirado
  │
  ▼
🖥️ Backend - JWT Middleware
  │
  │ 3. Valida el token
  │    ❌ Token expirado
  │    Retorna: 401 Unauthorized
  │
  ▼
📦 Response: 401 Unauthorized
  │
  ▼
🛡️ Auth Interceptor
  │
  │ 4. Detecta error 401
  │ 5. Llama: this.authService.refreshToken()
  │
  ▼
🔧 AuthService
  │
  │ 6. POST /api/auth/refresh
  │    Body: {
  │      "token": "expired_token",
  │      "refreshToken": "a1b2c3..."
  │    }
  │
  ▼
🖥️ Backend - AuthController.RefreshToken()
  │
  │ 7. Valida refresh token:
  │    ✅ Existe en BD
  │    ✅ No ha sido usado
  │    ✅ No ha expirado
  │
  │ 8. Marca refresh token como usado
  │ 9. Genera nuevo JWT token
  │ 10. Genera nuevo refresh token
  │
  ▼
📦 Response JSON
  {
    "token": "nuevo_jwt_token",
    "refreshToken": "nuevo_refresh_token",
    ...
  }
  │
  ▼
🔧 AuthService
  │
  │ 11. Actualiza localStorage con nuevos tokens
  │
  ▼
🛡️ Auth Interceptor
  │
  │ 12. Reintenta la petición original con nuevo token
  │
  ▼
🎉 Petición exitosa con nuevo token
```

---

## 📝 Flujo de Operaciones CRUD

### Ejemplo: Crear un Producto

```
┌─────────────────────────────────────────────────────────────────────┐
│                    CREAR PRODUCTO (CRUD)                             │
└─────────────────────────────────────────────────────────────────────┘

👤 Usuario (Administrador)
  │
  │ 1. Va a "Administración" → "Productos"
  │ 2. Hace clic en "Nuevo Producto"
  │
  ▼
📱 Admin Products Component (admin-products.component.ts)
  │
  │ 3. Muestra formulario:
  │    - Nombre: Gaseosa Postobon
  │    - Categoría: Bebidas
  │    - Precio Costo: $1,000
  │    - Precio Venta: $1,800
  │    - Stock: 100
  │    - etc.
  │
  │ 4. Usuario llena el formulario y hace clic en "Guardar"
  │
  ▼
📱 Component - onSubmit()
  │
  │ 5. Valida el formulario
  │    ✅ Todos los campos requeridos completos
  │
  │ 6. Llama: this.productService.create(productData)
  │
  ▼
🔧 ProductService (product.service.ts)
  │
  │ 7. POST /api/product
  │    Body: {
  │      "name": "Gaseosa Postobon",
  │      "categoryId": 1,
  │      "unitCost": 1000,
  │      "unitPrice": 1800,
  │      "stockOnHand": 100,
  │      "taxRate": 19,
  │      ...
  │    }
  │
  ▼
🛡️ Auth Interceptor
  │
  │ 8. Agrega token de autenticación
  │
  ▼
🌐 HTTP POST Request
  │
  │ POST http://localhost:5170/api/product
  │ Headers:
  │   Authorization: Bearer eyJhbGc...
  │   Content-Type: application/json
  │ Body: { producto... }
  │
  ▼
🖥️ Backend - ProductController.Create()
  │
  │ 9. Valida ModelState
  │    ✅ Datos válidos
  │
  │ 10. Llama: _productBusiness.Create(productDto)
  │
  ▼
🖥️ Business Layer - ProductBusiness.Create()
  │
  │ 11. Validaciones de negocio:
  │     - ✅ Precio de venta > Precio de costo
  │     - ✅ Stock >= 0
  │     - ✅ Categoría existe
  │
  │ 12. Mapea DTO → Entity
  │     ProductDto → Product
  │
  │ 13. Llama: _productData.Create(product)
  │
  ▼
🖥️ Data Layer - ProductData.Create()
  │
  │ 14. _context.products.Add(product)
  │ 15. _context.SaveChangesAsync()
  │
  ▼
💾 Entity Framework Core
  │
  │ 16. Genera SQL:
  │     INSERT INTO products (Name, CategoryId, UnitCost, UnitPrice, ...)
  │     VALUES ('Gaseosa Postobon', 1, 1000, 1800, ...)
  │
  ▼
🗄️ SQL Server
  │
  │ 17. Ejecuta INSERT
  │ 18. Retorna ID del nuevo producto: 11
  │
  ▼
🖥️ Backend - Retorna respuesta
  │
  │ 19. Mapea Entity → DTO
  │ 20. Retorna JSON
  │
  ▼
📦 Response JSON
  {
    "id": 11,
    "name": "Gaseosa Postobon",
    "categoryId": 1,
    "unitCost": 1000,
    "unitPrice": 1800,
    "active": true,
    ...
  }
  │
  ▼
📱 Component - Recibe respuesta
  │
  │ 21. Muestra notificación: "Producto creado exitosamente"
  │ 22. Actualiza la lista de productos
  │ 23. Cierra el formulario
  │
  ▼
🎉 Producto creado y visible en la lista
```

---

## ⚡ Cómo Funciona en Tiempo Real

### Escenario: Cajero realizando una venta

```
┌──────────────────────────────────────────────────────────────────────────┐
│                    FLUJO DE VENTA EN TIEMPO REAL                          │
└──────────────────────────────────────────────────────────────────────────┘

👤 Cajero (Juan Pérez)
  │
  │ 1. Inicia sesión
  │    Email: cajero@estancopro.com
  │    Password: Cajero123!
  │
  ▼
📱 Login Component
  │
  │ POST /api/auth/login
  │ ✅ Login exitoso
  │ Token guardado en localStorage
  │
  ▼
📱 Dashboard
  │
  │ 2. Va a "Ventas" → "Punto de Venta (POS)"
  │
  ▼
📱 POS Component (pos.component.ts)
  │
  │ 3. Component se inicializa:
  │    - Carga lista de productos
  │    - Carga sesión de caja activa
  │    - Inicializa carrito vacío
  │
  ▼
🔧 ProductService.getAll()
  │
  │ GET /api/product
  │
  ▼
🖥️ Backend
  │
  │ SELECT * FROM products WHERE Active = 1
  │
  ▼
📦 Response: Lista de 10 productos
  │
  ▼
📱 POS Component
  │
  │ 4. Muestra productos disponibles
  │
  ▼
👤 Cajero
  │
  │ 5. Escanea código de barras / Busca producto
  │    Producto: Cerveza Poker (ID: 1)
  │    Cantidad: 2
  │
  ▼
📱 POS Component
  │
  │ 6. Agrega al carrito:
  │    - Producto: Cerveza Poker
  │    - Cantidad: 2
  │    - Precio unitario: $2,500
  │    - Subtotal: $5,000
  │    - IVA (19%): $950
  │    - Total: $5,950
  │
  │ 7. Actualiza vista en tiempo real
  │
  ▼
👤 Cajero
  │
  │ 8. Agrega más productos...
  │    - Marlboro x 1 = $5,000
  │    - Coca Cola x 3 = $6,000
  │
  │ TOTAL VENTA: $16,950
  │
  │ 9. Hace clic en "Finalizar Venta"
  │
  ▼
📱 POS Component
  │
  │ 10. Construye objeto de venta:
  │     {
  │       "userId": 2,  // ID del cajero
  │       "cashSessionId": 1,
  │       "subtotal": 14237.29,
  │       "taxTotal": 2712.71,
  │       "grandTotal": 16950,
  │       "details": [
  │         { "productId": 1, "quantity": 2, "unitPrice": 2500, ... },
  │         { "productId": 5, "quantity": 1, "unitPrice": 5000, ... },
  │         { "productId": 4, "quantity": 3, "unitPrice": 2000, ... }
  │       ]
  │     }
  │
  │ 11. Llama: this.saleService.create(sale)
  │
  ▼
🔧 SaleService
  │
  │ POST /api/sale
  │
  ▼
🖥️ Backend - SaleController.Create()
  │
  │ 12. Valida datos
  │ 13. Inicia transacción de BD
  │
  ▼
🖥️ Business Layer - SaleBusiness.Create()
  │
  │ 14. Validaciones:
  │     ✅ Sesión de caja activa
  │     ✅ Productos existen
  │     ✅ Hay stock suficiente
  │
  │ 15. Para cada producto vendido:
  │     - Actualiza stock
  │     - Calcula impuestos
  │     - Crea detalle de venta
  │
  ▼
🗄️ SQL Server
  │
  │ BEGIN TRANSACTION
  │
  │ INSERT INTO sales (UserId, CashSessionId, Subtotal, TaxTotal, ...)
  │ → Sale ID: 42
  │
  │ INSERT INTO sale_product_details (SaleId, ProductId, Quantity, ...)
  │ → 3 registros insertados
  │
  │ UPDATE products SET StockOnHand = StockOnHand - 2 WHERE Id = 1
  │ UPDATE products SET StockOnHand = StockOnHand - 1 WHERE Id = 5
  │ UPDATE products SET StockOnHand = StockOnHand - 3 WHERE Id = 4
  │
  │ UPDATE cash_sessions SET TotalSales = TotalSales + 16950 WHERE Id = 1
  │
  │ COMMIT TRANSACTION
  │
  ▼
🖥️ Backend
  │
  │ 16. Transacción exitosa ✅
  │ 17. Retorna venta creada
  │
  ▼
📦 Response JSON
  {
    "id": 42,
    "saleDate": "2025-11-09T23:00:00Z",
    "grandTotal": 16950,
    "status": "completed",
    ...
  }
  │
  ▼
📱 POS Component
  │
  │ 18. Muestra ticket de venta
  │ 19. Opción de imprimir
  │ 20. Limpia el carrito
  │ 21. Listo para siguiente venta
  │
  ▼
🎉 Venta completada
   - Stock actualizado en BD
   - Registro de venta creado
   - Sesión de caja actualizada
```

---

## 🚀 Ejecutar el Sistema Completo

### Requisitos Previos

- ✅ .NET 9.0 SDK
- ✅ Node.js 18+ y npm
- ✅ SQL Server (LocalDB o SQL Server Express)
- ✅ Git (opcional)

### Paso 1: Iniciar el Backend

```bash
# Abrir una terminal

# Navegar a la carpeta del backend
cd C:\Users\jsola\Desktop\ADSO\EstancoPro\Backend\Web

# Ejecutar el backend
dotnet run
```

**Salida esperada:**
```
info: Program[0]
      Aplicando migraciones pendientes...
info: Program[0]
      Migraciones aplicadas exitosamente.
info: Program[0]
      Inicializando datos del sistema...
info: Presentation.Services.DatabaseInitializer[0]
      ====================================
info: Presentation.Services.DatabaseInitializer[0]
      USUARIOS DE PRUEBA CREADOS:
info: Presentation.Services.DatabaseInitializer[0]
      1. Admin - Email: admin@estancopro.com - Password: Admin123!
info: Presentation.Services.DatabaseInitializer[0]
      2. Cajero - Email: cajero@estancopro.com - Password: Cajero123!
...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5170
```

✅ **Backend ejecutándose en:** http://localhost:5170
✅ **Swagger disponible en:** http://localhost:5170/swagger

### Paso 2: Iniciar el Frontend

```bash
# Abrir OTRA terminal (dejar el backend corriendo)

# Navegar a la carpeta del frontend
cd C:\Users\jsola\Desktop\ADSO\EstancoPro\Frontend

# Ejecutar el frontend
npm start
```

**Salida esperada:**
```
> frotend@0.0.0 start
> ng serve

✔ Building...
Initial chunk files | Names         |  Raw size
main.js             | main          | 506.24 kB
styles.css          | styles        |   2.53 kB

Application bundle generation complete. [15.229 seconds]

Watch mode enabled. Watching for file changes...
  ➜  Local:   http://localhost:4200/
```

✅ **Frontend ejecutándose en:** http://localhost:4200

### Paso 3: Usar el Sistema

#### Opción A: Probar con Swagger (Backend)

1. Abre http://localhost:5170/swagger
2. Busca el endpoint **POST /api/auth/login**
3. Click en "Try it out"
4. Ingresa:
   ```json
   {
     "email": "admin@estancopro.com",
     "password": "Admin123!"
   }
   ```
5. Click en "Execute"
6. Copia el `token` de la respuesta
7. Click en "Authorize" (candado verde)
8. Ingresa: `Bearer {token}`
9. Ahora puedes probar todos los endpoints

#### Opción B: Usar la Aplicación Web (Frontend)

1. Abre http://localhost:4200
2. Verás la página de login
3. Ingresa credenciales:
   - **Email:** admin@estancopro.com
   - **Password:** Admin123!
4. Click en "Iniciar Sesión"
5. Serás redirigido al dashboard
6. Explora las diferentes secciones:
   - **Dashboard:** Resumen del sistema
   - **Ventas:** Punto de venta (POS)
   - **Productos:** Gestión de inventario
   - **Administración:** Usuarios, roles, categorías, etc.

### Usuarios de Prueba Disponibles

| Usuario | Email | Contraseña | Rol |
|---------|-------|-----------|-----|
| Admin | admin@estancopro.com | Admin123! | Administrador |
| Juan Pérez | cajero@estancopro.com | Cajero123! | Cajero |
| María García | vendedor@estancopro.com | Vendedor123! | Vendedor |
| Carlos Rodríguez | supervisor@estancopro.com | Supervisor123! | Supervisor |

### Arquitectura en Ejecución

```
┌─────────────────────────────────────────────────────────────────┐
│                   SISTEMA EN EJECUCIÓN                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Terminal 1                           Terminal 2                │
│  ┌────────────────┐                  ┌──────────────────┐      │
│  │ Backend        │                  │ Frontend         │      │
│  │ dotnet run     │◄────HTTP────────►│ npm start        │      │
│  │ Port: 5170     │                  │ Port: 4200       │      │
│  └────────────────┘                  └──────────────────┘      │
│         │                                     │                 │
│         │                                     │                 │
│         ▼                                     ▼                 │
│  ┌────────────────┐                  ┌──────────────────┐      │
│  │ SQL Server     │                  │ Browser          │      │
│  │ EstancoProDB   │                  │ localhost:4200   │      │
│  └────────────────┘                  └──────────────────┘      │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📊 Resumen de Tecnologías

### Backend

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| ASP.NET Core | 9.0 | Framework web |
| Entity Framework Core | 9.0 | ORM para acceso a datos |
| SQL Server | - | Base de datos |
| BCrypt.Net-Next | 4.0.3 | Hasheo de contraseñas |
| AutoMapper | 12.0 | Mapeo de objetos |
| JWT Bearer | 9.0 | Autenticación con tokens |
| Swagger | 9.0.6 | Documentación de API |

### Frontend

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| Angular | 20.3 | Framework SPA |
| TypeScript | 5.9 | Lenguaje tipado |
| RxJS | 7.8 | Programación reactiva |
| Signals | - | Gestión de estado reactivo |
| Tabler Icons | 3.35 | Iconos |

---

## 🎯 Conclusión

El sistema **EstancoPro** es una aplicación completa que integra un backend robusto con ASP.NET Core y un frontend moderno con Angular. La comunicación entre ambos se realiza mediante API REST con autenticación JWT, garantizando seguridad y escalabilidad.

**Características principales:**
- ✅ Inicialización automática de datos
- ✅ Autenticación y autorización con JWT
- ✅ Seguridad con BCrypt para contraseñas
- ✅ Arquitectura en capas (Backend)
- ✅ Programación reactiva con Signals (Frontend)
- ✅ CRUD completo para todas las entidades
- ✅ Punto de venta (POS) funcional
- ✅ Control de inventario en tiempo real

¡El sistema está listo para ser usado y extendido! 🚀
