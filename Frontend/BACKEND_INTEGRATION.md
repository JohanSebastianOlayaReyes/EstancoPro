# Backend-Frontend Integration Guide

## Overview
This document describes how the Angular frontend is connected to the ASP.NET Core backend.

## Backend Configuration

### API URL
- **Development**: `http://localhost:5000/api`
- **Production**: `https://api.estancopro.com/api`

### CORS Configuration
The backend is configured to allow all origins in development (Program.cs:60-67):
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
```

### JWT Authentication
- Configured in `appsettings.json`
- Uses Bearer token authentication
- Tokens are validated on each request

## Frontend Configuration

### Environment Files
Environment configuration files are located in `src/environments/`:

- `environment.ts` - Default development settings
- `environment.development.ts` - Development settings
- `environment.prod.ts` - Production settings

### HTTP Client Setup
The Angular app is configured with HTTP client and interceptors in `src/app/app.config.ts`:

```typescript
provideHttpClient(
  withFetch(),
  withInterceptors([authInterceptor, errorInterceptor])
)
```

### Core Services

#### ApiService (`src/app/core/services/api.service.ts`)
Generic HTTP service for making API calls:
- `get<T>(endpoint, params?)`
- `post<T>(endpoint, body)`
- `put<T>(endpoint, body)`
- `delete<T>(endpoint)`
- `patch<T>(endpoint, body)`

#### AuthService (`src/app/core/services/auth.service.ts`)
Handles authentication and JWT token management:
- `login(credentials): Observable<LoginResponseDto>`
- `logout(): void`
- `refreshToken(): Observable<LoginResponseDto>`
- `getToken(): string | null`
- `isTokenExpired(): boolean`

Stores tokens and user data in localStorage:
- `access_token`
- `refresh_token`
- `current_user`

### Interceptors

#### Auth Interceptor (`src/app/core/interceptors/auth.interceptor.ts`)
Automatically adds the JWT Bearer token to all outgoing requests (except login and refresh-token endpoints).

#### Error Interceptor (`src/app/core/interceptors/error.interceptor.ts`)
Handles HTTP errors globally:
- Catches 401 errors (unauthorized)
- Logs out user and redirects to login
- Logs errors to console

## Testing the Connection

### Test Component
A test component has been created at `src/app/features/test-connection/test-connection.component.ts` to verify the backend connection.

### Access the Test Page
1. Start the backend: `cd Backend/Web && dotnet run --urls "http://localhost:5000"`
2. Start the frontend: `npm start`
3. Navigate to `http://localhost:4200/test-connection`

### Test Login
Use the test component to verify authentication:
1. Enter username and password
2. Click "Test Login"
3. If successful, you'll see the JWT token and user information

## Starting the Application

### Backend
```bash
cd Backend/Web
dotnet run --urls "http://localhost:5000"
```

The API will be available at:
- API: `http://localhost:5000/api`
- Swagger UI: `http://localhost:5000` (development only)

### Frontend
```bash
npm start
```

The application will be available at `http://localhost:4200`

## API Endpoints

All API endpoints are prefixed with `/api/`. Example endpoints:

- `POST /api/Auth/login` - Authenticate user
- `POST /api/Auth/refresh-token` - Refresh JWT token
- `GET /api/User` - Get all users
- `POST /api/User` - Create new user
- And more...

## Models

### Authentication Models (`src/app/core/models/auth.model.ts`)

```typescript
interface LoginDto {
  username: string;
  password: string;
}

interface LoginResponseDto {
  token: string;
  refreshToken: string;
  expiresIn: number;
  username: string;
  userId: number;
}
```

### API Response Model (`src/app/core/models/api-response.model.ts`)

```typescript
interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}
```

## Security Considerations

1. **Tokens are stored in localStorage** - Consider using httpOnly cookies for production
2. **CORS is open in development** - Restrict origins in production
3. **HTTPS should be used in production** - Never send tokens over HTTP
4. **Token expiration** - Tokens expire after 60 minutes (configurable in appsettings.json)

## Troubleshooting

### CORS Errors
- Verify backend CORS policy is enabled
- Check that frontend URL is allowed
- Ensure API URL in environment files is correct

### 401 Unauthorized
- Token may have expired - try logging in again
- Verify token is being sent in request headers
- Check backend JWT configuration

### Connection Refused
- Verify backend is running on correct port
- Check firewall settings
- Ensure correct API URL in environment files
