# 🚀 Guía del Sistema Automático - EstancoPro

## 📋 Tabla de Contenidos
1. [Resumen del Sistema](#resumen-del-sistema)
2. [Inicialización Automática](#inicialización-automática)
3. [Usuarios de Prueba](#usuarios-de-prueba)
4. [Datos de Prueba del Negocio](#datos-de-prueba-del-negocio)
5. [Seguridad con BCrypt](#seguridad-con-bcrypt)
6. [Arquitectura Backend](#arquitectura-backend)
7. [Arquitectura Frontend](#arquitectura-frontend)
8. [Flujo de Autenticación](#flujo-de-autenticación)
9. [Cómo Usar el Sistema](#cómo-usar-el-sistema)
10. [API Endpoints Disponibles](#api-endpoints-disponibles)

---

## 🎯 Resumen del Sistema

**EstancoPro** es un sistema completo para la gestión de un estanco (tienda de cigarrillos, bebidas y otros productos). El sistema cuenta con:

- ✅ **Backend API REST** con ASP.NET Core 9.0
- ✅ **Autenticación JWT** con tokens de acceso y refresh tokens
- ✅ **Base de datos SQL Server** con Entity Framework Core
- ✅ **Inicialización automática** de datos de prueba
- ✅ **Seguridad con BCrypt** para hasheo de contraseñas
- ✅ **Sistema de roles** (Administrador, Cajero, Vendedor, Supervisor)
- ✅ **Gestión de inventario** con productos, categorías, proveedores
- ✅ **Sistema de ventas** y compras
- ✅ **Control de caja** con sesiones y movimientos

---

## 🔄 Inicialización Automática

### ¿Cómo funciona?

Cuando ejecutas la aplicación con `dotnet run`, el sistema **automáticamente**:

1. **Aplica las migraciones pendientes** a la base de datos
2. **Crea el rol de Administrador** si no existe
3. **Crea el usuario admin** si no existe
4. **Crea roles adicionales** (Cajero, Vendedor, Supervisor)
5. **Crea usuarios de prueba** con diferentes roles
6. **Crea datos de prueba** (categorías, productos, proveedores, etc.)

### ¿Dónde ocurre esto?

La inicialización automática ocurre en **`Program.cs`** (líneas 153-177):

```csharp
// 🗄️ APLICAR MIGRACIONES AUTOMÁTICAMENTE
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Aplicando migraciones pendientes...");
        context.Database.Migrate(); // ← Aplica migraciones automáticamente
        logger.LogInformation("Migraciones aplicadas exitosamente.");

        // Inicializar usuario administrador y datos de prueba
        logger.LogInformation("Inicializando datos del sistema...");
        var initializer = new DatabaseInitializer(context, ...);
        await initializer.InitializeAsync(); // ← Crea usuarios y datos
        logger.LogInformation("Inicialización completada.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al aplicar migraciones a la base de datos.");
        throw;
    }
}
```

### ¿Quién lo hace?

El archivo **`DatabaseInitializer.cs`** (`Backend/Web/Services/DatabaseInitializer.cs`) es el responsable de:

1. **`InitializeAsync()`** - Método principal que orquesta todo
2. **`SeedRolesAndUsersAsync()`** - Crea roles y usuarios de prueba
3. **`SeedTestDataAsync()`** - Crea categorías, productos, proveedores, etc.

---

## 👥 Usuarios de Prueba

El sistema crea automáticamente **4 usuarios de prueba** con contraseñas hasheadas usando BCrypt:

| # | Rol | Email | Contraseña | Nombre | Teléfono | Cédula |
|---|-----|-------|-----------|---------|----------|---------|
| 1 | **Administrador** | admin@estancopro.com | Admin123! | Admin | N/A | N/A |
| 2 | **Cajero** | cajero@estancopro.com | Cajero123! | Juan Pérez | 300123456 | 12345678 |
| 3 | **Vendedor** | vendedor@estancopro.com | Vendedor123! | María García | 310234567 | 23456789 |
| 4 | **Supervisor** | supervisor@estancopro.com | Supervisor123! | Carlos Rodríguez | 320345678 | 34567890 |

### ¿Cómo se crean?

En **`DatabaseInitializer.cs`** → método `SeedRolesAndUsersAsync()`:

```csharp
// Crear usuarios de prueba si no existen
var usersToCreate = new List<(string Email, string Password, string RolType, string FullName, int Phone, int IdNumber)>
{
    ("cajero@estancopro.com", "Cajero123!", "Cajero", "Juan Pérez", 300123456, 12345678),
    ("vendedor@estancopro.com", "Vendedor123!", "Vendedor", "María García", 310234567, 23456789),
    ("supervisor@estancopro.com", "Supervisor123!", "Supervisor", "Carlos Rodríguez", 320345678, 34567890)
};

foreach (var (email, password, rolType, fullName, phone, idNumber) in usersToCreate)
{
    // 1. Crear la persona
    var person = new Person
    {
        FullName = fullName,
        PhoneNumber = phone,
        NumberIdentification = idNumber,
        Active = true,
        CreateAt = DateTime.UtcNow,
        UpdateAt = DateTime.UtcNow
    };
    _context.persons.Add(person);
    await _context.SaveChangesAsync();

    // 2. Crear el usuario con contraseña hasheada
    var user = new User
    {
        Email = email,
        Password = BCrypt.Net.BCrypt.HashPassword(password), // ← Contraseña hasheada
        PersonId = person.Id,
        RolId = role.Id,
        Active = true,
        CreateAt = DateTime.UtcNow,
        UpdateAt = DateTime.UtcNow
    };
    _context.users.Add(user);
    await _context.SaveChangesAsync();
}
```

---

## 📦 Datos de Prueba del Negocio

### Categorías (5)
1. **Bebidas** - Bebidas alcohólicas y no alcohólicas
2. **Cigarrillos** - Productos de tabaco
3. **Snacks** - Snacks y aperitivos
4. **Dulces** - Dulces y golosinas
5. **Otros** - Otros productos

### Unidades de Medida (4)
1. Unidad
2. Caja
3. Paquete
4. Botella

### Proveedores (3)
1. **Distribuidora Central** - Tel: 3001234567
2. **Licores del Valle** - Tel: 3107654321
3. **Tabacalera Nacional** - Tel: 3209876543

### Productos (10)

#### Bebidas:
- **Cerveza Poker** - Costo: $1,500 | Precio: $2,500 | Stock: 100
- **Aguila** - Costo: $1,500 | Precio: $2,500 | Stock: 80
- **Ron Medellín** - Costo: $25,000 | Precio: $35,000 | Stock: 30
- **Coca Cola** - Costo: $1,200 | Precio: $2,000 | Stock: 150

#### Cigarrillos:
- **Marlboro** - Costo: $3,500 | Precio: $5,000 | Stock: 200
- **Lucky Strike** - Costo: $3,000 | Precio: $4,500 | Stock: 150

#### Snacks:
- **Papas Margarita** - Costo: $800 | Precio: $1,500 | Stock: 120
- **Doritos** - Costo: $1,200 | Precio: $2,000 | Stock: 100

#### Dulces:
- **Chocolatina Jet** - Costo: $500 | Precio: $1,000 | Stock: 200
- **Bon Bon Bum** - Costo: $300 | Precio: $500 | Stock: 300

### Precios por Presentación

Cada producto tiene precios configurados para diferentes unidades de medida:
- **Por unidad** (precio base)
- **Por caja** (24 unidades con 10% de descuento para bebidas y cigarrillos)

---

## 🔒 Seguridad con BCrypt

### ¿Qué es BCrypt?

**BCrypt** es un algoritmo de hasheo de contraseñas que:
- ✅ Es **irreversible** (no se puede desencriptar)
- ✅ Usa **salt** automático (protege contra ataques de diccionario)
- ✅ Es **lento** por diseño (protege contra fuerza bruta)

### Instalación

El paquete **BCrypt.Net-Next v4.0.3** se instaló automáticamente en el proyecto:

```bash
dotnet add package BCrypt.Net-Next
```

### Uso en el Código

#### 1. Al crear un usuario (hashear contraseña):

```csharp
// En DatabaseInitializer.cs
var user = new User
{
    Email = "admin@estancopro.com",
    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // ← Hashea la contraseña
    // ...
};
```

#### 2. Al hacer login (verificar contraseña):

```csharp
// En AuthController.cs
if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
{
    return Unauthorized(new { message = "Email o contraseña incorrectos" });
}
```

### Ejemplo de Hash

```
Contraseña original: "Admin123!"
Hash guardado en DB: "$2a$11$vJKxW3QbZq5X9Z7Y8T6U1eN..."
                      ↑
                      Imposible de revertir
```

---

## 🏗️ Arquitectura Backend

### Estructura del Proyecto

```
EstancoPro/Backend/
├── Entity/              # Modelos de datos y contexto de BD
│   ├── Model/          # Entidades (User, Person, Product, etc.)
│   ├── Dto/            # DTOs para transferencia de datos
│   ├── Context/        # ApplicationDbContext
│   └── Migrations/     # Migraciones de EF Core
│
├── Data/               # Capa de acceso a datos (DAL)
│   ├── Interfaces/     # Interfaces IData
│   └── Implementations/# Implementaciones de acceso a datos
│
├── Business/           # Lógica de negocio (BLL)
│   ├── Interfaces/     # Interfaces IBusiness
│   └── Implementations/# Lógica de negocio
│
├── Utilities/          # Servicios auxiliares
│   ├── Services/       # JwtService, etc.
│   └── Mapper/         # AutoMapper profiles
│
└── Web/                # API REST (Presentation Layer)
    ├── Controllers/    # Endpoints de la API
    ├── Services/       # DatabaseInitializer
    └── Program.cs      # Configuración de la app
```

### Flujo de una Petición

```
1. Cliente (Frontend)
   ↓
2. Controller (AuthController, ProductController, etc.)
   ↓
3. Business Layer (IUserBusiness → UserBusiness)
   ↓
4. Data Layer (IUserData → UserData)
   ↓
5. Entity Framework Core
   ↓
6. SQL Server Database
```

### Configuración de la Base de Datos

**`appsettings.json`**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EstancoProDB;..."
  }
}
```

**`Program.cs`** (línea 20-21):
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

## 🎨 Arquitectura Frontend

### Estructura del Proyecto (Angular - Estimada)

```
EstancoPro/Frontend/
├── src/
│   ├── app/
│   │   ├── auth/               # Módulo de autenticación
│   │   │   ├── login/
│   │   │   ├── services/
│   │   │   └── guards/
│   │   │
│   │   ├── dashboard/          # Dashboard principal
│   │   ├── products/           # Gestión de productos
│   │   ├── sales/              # Ventas
│   │   ├── purchases/          # Compras
│   │   ├── cash-session/       # Control de caja
│   │   ├── reports/            # Reportes
│   │   │
│   │   ├── core/               # Servicios core
│   │   │   ├── services/       # HTTP services
│   │   │   ├── interceptors/   # JWT interceptor
│   │   │   └── models/         # Interfaces TypeScript
│   │   │
│   │   └── shared/             # Componentes compartidos
│   │
│   ├── environments/           # Configuración de entornos
│   └── assets/                 # Recursos estáticos
```

### Servicios HTTP (Ejemplo)

**`auth.service.ts`**:
```typescript
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl + '/api/auth';

  constructor(private http: HttpClient) {}

  login(credentials: LoginDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.apiUrl}/login`, credentials);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
```

### Interceptor JWT

**`jwt.interceptor.ts`**:
```typescript
export class JwtInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();

    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(req);
  }
}
```

---

## 🔐 Flujo de Autenticación

### 1. Login

```
┌─────────────┐         ┌──────────────┐         ┌──────────────┐
│   Frontend  │         │   Backend    │         │   Database   │
└──────┬──────┘         └──────┬───────┘         └──────┬───────┘
       │                       │                        │
       │ POST /api/auth/login  │                        │
       │ { email, password }   │                        │
       ├──────────────────────>│                        │
       │                       │                        │
       │                       │ SELECT user WHERE      │
       │                       │ email = 'admin@...'    │
       │                       ├───────────────────────>│
       │                       │                        │
       │                       │<───────────────────────┤
       │                       │ User encontrado        │
       │                       │                        │
       │                       │ BCrypt.Verify(         │
       │                       │   password, hash)      │
       │                       │ ✅ Válido              │
       │                       │                        │
       │                       │ Generar JWT Token      │
       │                       │ Generar Refresh Token  │
       │                       │                        │
       │                       │ INSERT refresh_token   │
       │                       ├───────────────────────>│
       │                       │                        │
       │<──────────────────────┤                        │
       │ {                     │                        │
       │   token: "eyJhbGc...", │                       │
       │   refreshToken: "...", │                       │
       │   email: "admin@...",  │                       │
       │   roleName: "Admin",   │                       │
       │   expiresAt: "..."     │                       │
       │ }                     │                        │
       │                       │                        │
       │ localStorage.setItem( │                        │
       │   'token', token)     │                        │
       │                       │                        │
```

### 2. Peticiones Protegidas

```
┌─────────────┐         ┌──────────────┐
│   Frontend  │         │   Backend    │
└──────┬──────┘         └──────┬───────┘
       │                       │
       │ GET /api/products     │
       │ Headers:              │
       │   Authorization:      │
       │   Bearer eyJhbGc...   │
       ├──────────────────────>│
       │                       │
       │                       │ JWT Middleware
       │                       │ ✅ Token válido
       │                       │ ✅ No expirado
       │                       │ ✅ Firma correcta
       │                       │
       │<──────────────────────┤
       │ [ productos... ]      │
       │                       │
```

### 3. Refresh Token

```
┌─────────────┐         ┌──────────────┐
│   Frontend  │         │   Backend    │
└──────┬──────┘         └──────┬───────┘
       │                       │
       │ POST /api/auth/refresh│
       │ {                     │
       │   token: "expired",   │
       │   refreshToken: "..." │
       │ }                     │
       ├──────────────────────>│
       │                       │
       │                       │ Validar refreshToken
       │                       │ ✅ Válido y no usado
       │                       │ ✅ No expirado
       │                       │
       │                       │ Marcar como usado
       │                       │ Generar nuevo JWT
       │                       │ Generar nuevo Refresh
       │                       │
       │<──────────────────────┤
       │ {                     │
       │   token: "new_token", │
       │   refreshToken: "new" │
       │ }                     │
       │                       │
```

---

## 🚀 Cómo Usar el Sistema

### 1. Iniciar el Backend

```bash
# Navegar a la carpeta del backend
cd C:\Users\jsola\Desktop\ADSO\EstancoPro\Backend\Web

# Ejecutar la aplicación
dotnet run
```

La aplicación estará disponible en: **http://localhost:5170**

### 2. Acceder a Swagger

Abre tu navegador en: **http://localhost:5170**

Swagger te mostrará todos los endpoints disponibles de la API.

### 3. Hacer Login con Swagger

1. En Swagger, busca el endpoint **POST /api/auth/login**
2. Click en "Try it out"
3. Ingresa las credenciales:
   ```json
   {
     "email": "admin@estancopro.com",
     "password": "Admin123!"
   }
   ```
4. Click en "Execute"
5. Copia el `token` de la respuesta

### 4. Autenticar en Swagger

1. Click en el botón "Authorize" (candado verde) en la parte superior
2. Ingresa: `Bearer {token}` (reemplaza {token} con el token copiado)
3. Click en "Authorize"
4. Ahora puedes probar todos los endpoints protegidos

### 5. Iniciar el Frontend (si existe)

```bash
# Navegar a la carpeta del frontend
cd C:\Users\jsola\Desktop\ADSO\EstancoPro\Frontend

# Instalar dependencias (solo la primera vez)
npm install

# Ejecutar la aplicación
ng serve
```

El frontend estará disponible en: **http://localhost:4200**

---

## 📡 API Endpoints Disponibles

### 🔐 Autenticación (`/api/auth`)

| Método | Endpoint | Descripción | Requiere Auth |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Iniciar sesión | ❌ No |
| POST | `/api/auth/refresh` | Renovar token | ❌ No |
| POST | `/api/auth/logout` | Cerrar sesión | ✅ Sí |
| POST | `/api/auth/logout-all` | Cerrar todas las sesiones | ✅ Sí |
| GET | `/api/auth/validate` | Validar token actual | ✅ Sí |

### 👤 Usuarios (`/api/user`)

| Método | Endpoint | Descripción | Requiere Auth |
|--------|----------|-------------|---------------|
| GET | `/api/user` | Listar usuarios | ✅ Sí |
| GET | `/api/user/{id}` | Obtener usuario por ID | ✅ Sí |
| POST | `/api/user` | Crear usuario | ✅ Sí |
| PUT | `/api/user/{id}` | Actualizar usuario | ✅ Sí |
| DELETE | `/api/user/{id}` | Eliminar usuario | ✅ Sí |

### 🎭 Roles (`/api/rol`)

| Método | Endpoint | Descripción | Requiere Auth |
|--------|----------|-------------|---------------|
| GET | `/api/rol` | Listar roles | ✅ Sí |
| GET | `/api/rol/{id}` | Obtener rol por ID | ✅ Sí |
| POST | `/api/rol` | Crear rol | ✅ Sí |
| PUT | `/api/rol/{id}` | Actualizar rol | ✅ Sí |
| DELETE | `/api/rol/{id}` | Eliminar rol | ✅ Sí |

### 📦 Productos (`/api/product`)

| Método | Endpoint | Descripción | Requiere Auth |
|--------|----------|-------------|---------------|
| GET | `/api/product` | Listar productos | ✅ Sí |
| GET | `/api/product/{id}` | Obtener producto por ID | ✅ Sí |
| POST | `/api/product` | Crear producto | ✅ Sí |
| PUT | `/api/product/{id}` | Actualizar producto | ✅ Sí |
| DELETE | `/api/product/{id}` | Eliminar producto | ✅ Sí |

### 🏷️ Categorías, Proveedores, etc.

Los siguientes endpoints también están disponibles:
- `/api/category` - Gestión de categorías
- `/api/supplier` - Gestión de proveedores
- `/api/unitmeasure` - Gestión de unidades de medida
- `/api/sale` - Gestión de ventas
- `/api/purchase` - Gestión de compras
- `/api/cashsession` - Control de caja

---

## 🔍 Ejemplo de Uso Completo

### 1. Login

**Request:**
```http
POST http://localhost:5170/api/auth/login
Content-Type: application/json

{
  "email": "admin@estancopro.com",
  "password": "Admin123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0...",
  "email": "admin@estancopro.com",
  "roleName": "Administrador",
  "userId": 1,
  "expiresAt": "2025-11-10T00:38:00Z",
  "refreshTokenExpiresAt": "2025-11-16T22:38:00Z"
}
```

### 2. Listar Productos

**Request:**
```http
GET http://localhost:5170/api/product
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Cerveza Poker",
    "unitCost": 1500,
    "unitPrice": 2500,
    "taxRate": 19,
    "stockOnHand": 100,
    "reorderPoint": 20,
    "categoryId": 1,
    "unitMeasureId": 1,
    "active": true
  },
  // ... más productos
]
```

### 3. Crear una Venta

**Request:**
```http
POST http://localhost:5170/api/sale
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "userId": 1,
  "cashSessionId": 1,
  "subtotal": 5000,
  "taxTotal": 950,
  "grandTotal": 5950,
  "details": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPrice": 2500,
      "taxRate": 19,
      "lineSubtotal": 5000,
      "lineTax": 950,
      "lineTotal": 5950
    }
  ]
}
```

---

## 📝 Notas Importantes

### ⚠️ Seguridad en Producción

Las credenciales de prueba están en texto plano en los logs por conveniencia de desarrollo. En producción:

1. **NO loguees contraseñas** en ningún formato
2. **Usa variables de entorno** para secretos
3. **Cambia la clave secreta JWT** en `appsettings.json`
4. **Habilita HTTPS** en producción
5. **Configura CORS** correctamente

### 💾 Base de Datos

- La cadena de conexión está en `appsettings.json`
- Las migraciones se aplican automáticamente al iniciar
- Los datos de prueba solo se crean una vez
- Para resetear, elimina la base de datos y vuelve a ejecutar

### 🔧 Configuración JWT

**`appsettings.json`**:
```json
{
  "JwtSettings": {
    "SecretKey": "TuClaveSecretaMuyLargaYSegura123456789",
    "Issuer": "EstancoProAPI",
    "Audience": "EstancoProClients"
  }
}
```

---

## 🎓 Recursos Adicionales

- [Documentación ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [JWT.io](https://jwt.io) - Decodificar tokens JWT
- [BCrypt Explained](https://en.wikipedia.org/wiki/Bcrypt)
- [Angular Documentation](https://angular.io/docs)

---

## 📧 Soporte

Si tienes problemas o preguntas, revisa:
1. Los logs de la consola del backend
2. La consola del navegador (F12) para el frontend
3. Las respuestas de error de la API en Swagger

---

**¡Listo para usar! 🎉**

El sistema está completamente configurado y listo para desarrollo. Todos los usuarios de prueba y datos están disponibles.
