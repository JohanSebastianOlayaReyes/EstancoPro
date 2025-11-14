# 🔌 Conexión Frontend - Backend EstancoPro

## ✅ Configuración Completada

### Backend
- **URL**: `http://localhost:5170`
- **API Base**: `http://localhost:5170/api`
- **CORS**: Configurado con `AllowAll` ✅
- **Puerto**: 5170

### Frontend
- **Puerto**: 4200
- **API URL configurada**: `http://localhost:5170/api` ✅
- **Modelos actualizados**: ✅
- **Interceptors configurados**: ✅

## 🚀 Cómo Iniciar

### 1. Iniciar Backend
```bash
cd Backend/Web
dotnet run
```

El backend estará disponible en:
- HTTP: `http://localhost:5170`
- Swagger: `http://localhost:5170/swagger`

### 2. Iniciar Frontend
```bash
cd Frontend
npm start
```

El frontend estará disponible en:
- `http://localhost:4200`

## 🔐 Credenciales de Prueba

Según el archivo `init-admin.sql`, las credenciales por defecto son:

**Usuario Administrador:**
- Email: `admin@gmail.com`
- Password: `Admin123*`

## 📡 Endpoints Configurados

### Autenticación
- **POST** `/api/Auth/login`
  ```json
  {
    "email": "admin@gmail.com",
    "password": "Admin123*"
  }
  ```

  **Respuesta:**
  ```json
  {
    "token": "eyJ...",
    "refreshToken": "...",
    "email": "admin@gmail.com",
    "roleName": "Administrador",
    "userId": 1,
    "expiresAt": "2024-...",
    "refreshTokenExpiresAt": "2024-..."
  }
  ```

### Productos
- **GET** `/api/Product` - Listar todos
- **GET** `/api/Product/{id}` - Obtener por ID
- **POST** `/api/Product` - Crear nuevo
- **PUT** `/api/Product/{id}` - Actualizar
- **DELETE** `/api/Product/{id}` - Eliminar

### Categorías
- **GET** `/api/Category` - Listar todas
- **POST** `/api/Category` - Crear nueva
- **PUT** `/api/Category/{id}` - Actualizar
- **DELETE** `/api/Category/{id}` - Eliminar

## 🔧 Cambios Realizados

### 1. Modelos Actualizados
- ✅ `LoginResponse` - Ahora coincide con el DTO del backend
- ✅ `AuthUser` - Simplificado para usar `roleName` en lugar de objetos anidados
- ✅ Eliminado wrapper `ApiResponse` - El backend devuelve objetos directamente

### 2. Servicios Actualizados
- ✅ `AuthService` - Maneja respuestas directas sin wrapper
- ✅ `ProductService` - Listo para consumir API real
- ✅ `CategoryService` - Listo para consumir API real
- ✅ `UserService` - Listo para consumir API real

### 3. Componentes Actualizados
- ✅ `LoginComponent` - Maneja nueva estructura de respuesta
- ✅ `DashboardComponent` - Usa nuevo modelo de `AuthUser`
- ✅ `MainLayoutComponent` - Usa nuevo modelo de `AuthUser`
- ✅ `ProductsComponent` - CRUD completo listo para usar

## 🎯 Flujo de Autenticación

1. Usuario ingresa credenciales en `/auth/login`
2. Frontend envía POST a `/api/Auth/login`
3. Backend valida y devuelve JWT token
4. Frontend guarda token en `localStorage`
5. `AuthInterceptor` añade token automáticamente a todas las peticiones
6. Usuario es redirigido a `/dashboard`

## 🛡️ Seguridad Implementada

- ✅ JWT Bearer Token
- ✅ Token guardado en localStorage
- ✅ AuthGuard protege rutas privadas
- ✅ PublicGuard redirige si ya está autenticado
- ✅ ErrorInterceptor maneja errores 401 (no autorizado)
- ✅ Verificación de expiración de token

## 📝 Notas Importantes

### Estructura de Respuestas

El backend **NO** usa un wrapper `ApiResponse` estándar. Las respuestas son directas:

**Login (exitoso):**
```json
{
  "token": "eyJhbGc...",
  "refreshToken": "...",
  "email": "admin@gmail.com",
  "roleName": "Administrador",
  "userId": 1,
  "expiresAt": "2024-11-14T05:00:00Z",
  "refreshTokenExpiresAt": "2024-11-21T04:00:00Z"
}
```

**Login (fallido - 401 Unauthorized):**
```json
{
  "message": "Email o contraseña incorrectos"
}
```

### Si necesitas wrapper ApiResponse

Si prefieres usar un wrapper estándar en todas las respuestas, necesitarías:

1. Crear un `ResponseWrapper` en el backend
2. Modificar todos los controladores para usar el wrapper
3. Actualizar los modelos del frontend

Por ahora, el frontend está configurado para trabajar con las respuestas directas del backend.

## ✅ Estado Actual

- ✅ Frontend compila sin errores
- ✅ Backend configurado con CORS
- ✅ URLs configuradas correctamente
- ✅ Modelos sincronizados
- ✅ Interceptors funcionando
- ✅ Guards implementados
- ✅ Ready para pruebas!

## 🧪 Próximos Pasos

1. Iniciar ambos servidores (Backend y Frontend)
2. Probar login con credenciales por defecto
3. Navegar al dashboard
4. Probar CRUD de productos
5. Verificar que los tokens se guarden correctamente
6. Probar logout y re-login

---

**Proyecto listo para desarrollo y pruebas! 🚀**
