using Business.Implementations;
using Business.Interfaces;
using Utilities.Services;
using Utilities.Mapper;
using Data.Implementations;
using Data.Interfaces;
using Entity.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Business;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 🔧 CONFIGURACIÓN GENERAL
// =========================

// Contexto de base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

// Ensure secretKey is not null or empty
if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Servicios JWT
builder.Services.AddScoped<IJwtService, JwtService>();

// CORS (para permitir peticiones desde frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Controladores
builder.Services.AddControllers();

// Swagger con soporte JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Security Model API",
        Version = "v1",
        Description = "API para la gestión de seguridad, roles, permisos y formularios con autenticación JWT"
    });

    // Configuración de seguridad JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// =========================
// 📦 INYECCIÓN DE DEPENDENCIAS
// =========================

// Base genérica - NOTA: Estos necesitarán ser actualizados con tipos específicos
// builder.Services.AddScoped(typeof(IBaseData<,>), typeof(BaseData<,>));
// builder.Services.AddScoped(typeof(IBaseBusiness<,>), typeof(BaseBusiness<,>));

// Entidades específicas - TODO: Actualizar con DTOs específicos
builder.Services.AddScoped<IUserData, UserData>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();

builder.Services.AddScoped<IRolData, RolData>();
builder.Services.AddScoped<IRolBusiness, RolBusiness>();

builder.Services.AddScoped<IPermissionData, PemissionData>();
builder.Services.AddScoped<IPermissionBusiness, PermissionBusiness>();

builder.Services.AddScoped<IFormData, FormData>();
builder.Services.AddScoped<IFormBusiness, FormBusiness>();

builder.Services.AddScoped<IModuleData, ModuleData>();
builder.Services.AddScoped<IModuleBusiness, ModuleBusiness>();

builder.Services.AddScoped<IFormModuleData, FormModuleData>();
builder.Services.AddScoped<IFormModuleBusiness, FormModuleBusiness>();

builder.Services.AddScoped<IPersonData, PersonData>();
builder.Services.AddScoped<IPersonBusiness, PersonBusiness>();

builder.Services.AddScoped<IRolUserData, RolUserData>();
builder.Services.AddScoped<IUserRolBusiness, UserRolBusiness>();

builder.Services.AddScoped<IRolFormPermissionData, RolFormPermissionData>();
builder.Services.AddScoped<IRolFormPermissionBusiness, RolFormPermissionBusiness>();

// Registro de servicios específicos
builder.Services.AddScoped<ISupplierBusiness, SupplierBusiness>();
builder.Services.AddScoped<ISaleBusiness, SaleBusiness>();
builder.Services.AddScoped<IProductBusiness, ProductBusiness>();
builder.Services.AddScoped<ICashSessionBusiness, CashSessionBusiness>();

// Registro de dependencias para las capas de datos
builder.Services.AddScoped<ISupplierData, SupplierData>();
builder.Services.AddScoped<ISaleData, SaleData>();
builder.Services.AddScoped<IProductData, ProductData>();
builder.Services.AddScoped<ICashSessionData, CashSessionData>();

// Registro de AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Registro explícito del perfil AutoMapperProfile
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// =========================
// 🗄️ APLICAR MIGRACIONES AUTOMÁTICAMENTE
// =========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Aplicando migraciones pendientes...");
        context.Database.Migrate();
        logger.LogInformation("Migraciones aplicadas exitosamente.");

        // Inicializar usuario administrador
        logger.LogInformation("Inicializando datos del sistema...");
        var initializer = new Presentation.Services.DatabaseInitializer(context, services.GetRequiredService<ILogger<Presentation.Services.DatabaseInitializer>>());
        await initializer.InitializeAsync();
        logger.LogInformation("Inicialización completada.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al aplicar migraciones a la base de datos.");
        throw; // Re-lanzar la excepción para que la app no inicie con DB en mal estado
    }
}

// =========================
// 🚀 MIDDLEWARE PIPELINE
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Security Model API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz (http://localhost:5000)
    });
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

// IMPORTANTE: El orden es crítico
app.UseAuthentication(); // Primero autenticación
app.UseAuthorization();  // Luego autorización

app.MapControllers();

app.Run();
