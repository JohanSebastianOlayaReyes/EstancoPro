using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Presentation.Services
{
    public class DatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(ApplicationDbContext context, ILogger<DatabaseInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Verificar si existe el rol de Administrador
                var adminRole = await _context.rols.FirstOrDefaultAsync(r => r.TypeRol == "Administrador");

                if (adminRole == null)
                {
                    _logger.LogInformation("Creando rol de Administrador...");
                    adminRole = new Rol
                    {
                        TypeRol = "Administrador",
                        Description = "Administrador del sistema con acceso completo",
                        Active = true,
                        CreateAt = DateTime.UtcNow,
                        UpdateAt = DateTime.UtcNow
                    };
                    _context.rols.Add(adminRole);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Rol Administrador creado exitosamente con ID: {RoleId}", adminRole.Id);
                }

                // Verificar si existe el usuario admin
                var adminUser = await _context.users
                    .Include(u => u.person)
                    .FirstOrDefaultAsync(u => u.Email == "admin@estancopro.com");

                if (adminUser == null)
                {
                    _logger.LogInformation("Creando usuario Administrador...");

                    // Crear persona para el admin
                    var adminPerson = new Person
                    {
                        FullName = "Admin",
                        PhoneNumber = 0,
                        NumberIdentification = 0,
                        Active = true,
                        CreateAt = DateTime.UtcNow,
                        UpdateAt = DateTime.UtcNow
                    };
                    _context.persons.Add(adminPerson);
                    await _context.SaveChangesAsync();

                    // Crear usuario admin con contraseña hasheada
                    adminUser = new User
                    {
                        Email = "admin@estancopro.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        PersonId = adminPerson.Id,
                        RolId = adminRole.Id,
                        Active = true,
                        CreateAt = DateTime.UtcNow,
                        UpdateAt = DateTime.UtcNow
                    };
                    _context.users.Add(adminUser);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("====================================");
                    _logger.LogInformation("Usuario Administrador creado exitosamente");
                    _logger.LogInformation("Email: admin@estancopro.com");
                    _logger.LogInformation("Password: Admin123!");
                    _logger.LogInformation("====================================");
                }
                else
                {
                    _logger.LogInformation("El usuario administrador ya existe");
                }

                // Crear roles y usuarios de prueba
                await SeedRolesAndUsersAsync();

                // Crear datos de prueba
                await SeedTestDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar la base de datos");
                throw;
            }
        }

        private async Task SeedRolesAndUsersAsync()
        {
            try
            {
                _logger.LogInformation("Verificando roles y usuarios de prueba...");

                // Crear roles adicionales si no existen
                var rolesToCreate = new List<(string TypeRol, string Description)>
                {
                    ("Cajero", "Usuario encargado de la caja y ventas"),
                    ("Vendedor", "Usuario con permisos de venta"),
                    ("Supervisor", "Usuario con permisos de supervisión")
                };

                foreach (var (typeRol, description) in rolesToCreate)
                {
                    var existingRole = await _context.rols.FirstOrDefaultAsync(r => r.TypeRol == typeRol);
                    if (existingRole == null)
                    {
                        _logger.LogInformation("Creando rol: {TypeRol}", typeRol);
                        var newRole = new Rol
                        {
                            TypeRol = typeRol,
                            Description = description,
                            Active = true,
                            CreateAt = DateTime.UtcNow,
                            UpdateAt = DateTime.UtcNow
                        };
                        _context.rols.Add(newRole);
                    }
                }
                await _context.SaveChangesAsync();

                // Crear usuarios de prueba si no existen
                var usersToCreate = new List<(string Email, string Password, string RolType, string FullName, int Phone, int IdNumber)>
                {
                    ("cajero@estancopro.com", "Cajero123!", "Cajero", "Juan Pérez", 300123456, 12345678),
                    ("vendedor@estancopro.com", "Vendedor123!", "Vendedor", "María García", 310234567, 23456789),
                    ("supervisor@estancopro.com", "Supervisor123!", "Supervisor", "Carlos Rodríguez", 320345678, 34567890)
                };

                foreach (var (email, password, rolType, fullName, phone, idNumber) in usersToCreate)
                {
                    var existingUser = await _context.users.FirstOrDefaultAsync(u => u.Email == email);
                    if (existingUser == null)
                    {
                        var role = await _context.rols.FirstOrDefaultAsync(r => r.TypeRol == rolType);
                        if (role != null)
                        {
                            _logger.LogInformation("Creando usuario de prueba: {Email}", email);

                            // Crear persona
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

                            // Crear usuario con contraseña hasheada
                            var user = new User
                            {
                                Email = email,
                                Password = BCrypt.Net.BCrypt.HashPassword(password),
                                PersonId = person.Id,
                                RolId = role.Id,
                                Active = true,
                                CreateAt = DateTime.UtcNow,
                                UpdateAt = DateTime.UtcNow
                            };
                            _context.users.Add(user);
                            await _context.SaveChangesAsync();

                            _logger.LogInformation("Usuario {Email} creado con rol {RolType} - Password: {Password}", email, rolType, password);
                        }
                    }
                }

                _logger.LogInformation("====================================");
                _logger.LogInformation("USUARIOS DE PRUEBA CREADOS:");
                _logger.LogInformation("1. Admin - Email: admin@estancopro.com - Password: Admin123!");
                _logger.LogInformation("2. Cajero - Email: cajero@estancopro.com - Password: Cajero123!");
                _logger.LogInformation("3. Vendedor - Email: vendedor@estancopro.com - Password: Vendedor123!");
                _logger.LogInformation("4. Supervisor - Email: supervisor@estancopro.com - Password: Supervisor123!");
                _logger.LogInformation("====================================");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear roles y usuarios de prueba");
                throw;
            }
        }

        private async Task SeedTestDataAsync()
        {
            // Verificar si ya existen datos
            if (await _context.categories.AnyAsync())
            {
                _logger.LogInformation("Los datos de prueba ya existen");
                return;
            }

            _logger.LogInformation("Creando datos de prueba...");

            // Crear Categorías
            var categorias = new List<Category>
            {
                new Category { Name = "Bebidas", Description = "Bebidas alcohólicas y no alcohólicas", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Category { Name = "Cigarrillos", Description = "Productos de tabaco", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Category { Name = "Snacks", Description = "Snacks y aperitivos", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Category { Name = "Dulces", Description = "Dulces y golosinas", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Category { Name = "Otros", Description = "Otros productos", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
            };
            _context.categories.AddRange(categorias);
            await _context.SaveChangesAsync();

            // Crear Unidades de Medida
            var unidades = new List<UnitMeasure>
            {
                new UnitMeasure { Name = "Unidad", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new UnitMeasure { Name = "Caja", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new UnitMeasure { Name = "Paquete", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new UnitMeasure { Name = "Botella", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
            };
            _context.unitMeasures.AddRange(unidades);
            await _context.SaveChangesAsync();

            // Crear Proveedores
            var proveedores = new List<Supplier>
            {
                new Supplier { Name = "Distribuidora Central", Phone = "3001234567", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Supplier { Name = "Licores del Valle", Phone = "3107654321", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Supplier { Name = "Tabacalera Nacional", Phone = "3209876543", Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
            };
            _context.suppliers.AddRange(proveedores);
            await _context.SaveChangesAsync();

            // Crear Productos
            var productos = new List<Product>
            {
                // Bebidas
                new Product { Name = "Cerveza Poker", UnitCost = 1500, UnitPrice = 2500, TaxRate = 19, StockOnHand = 100, ReorderPoint = 20, CategoryId = categorias[0].Id, UnitMeasureId = unidades[0].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Product { Name = "Aguila", UnitCost = 1500, UnitPrice = 2500, TaxRate = 19, StockOnHand = 80, ReorderPoint = 20, CategoryId = categorias[0].Id, UnitMeasureId = unidades[0].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Product { Name = "Ron Medellín", UnitCost = 25000, UnitPrice = 35000, TaxRate = 19, StockOnHand = 30, ReorderPoint = 5, CategoryId = categorias[0].Id, UnitMeasureId = unidades[3].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Product { Name = "Coca Cola", UnitCost = 1200, UnitPrice = 2000, TaxRate = 19, StockOnHand = 150, ReorderPoint = 30, CategoryId = categorias[0].Id, UnitMeasureId = unidades[0].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },

                // Cigarrillos
                new Product { Name = "Marlboro", UnitCost = 3500, UnitPrice = 5000, TaxRate = 19, StockOnHand = 200, ReorderPoint = 50, CategoryId = categorias[1].Id, UnitMeasureId = unidades[0].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Product { Name = "Lucky Strike", UnitCost = 3000, UnitPrice = 4500, TaxRate = 19, StockOnHand = 150, ReorderPoint = 40, CategoryId = categorias[1].Id, UnitMeasureId = unidades[0].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },

                // Snacks
                new Product { Name = "Papas Margarita", UnitCost = 800, UnitPrice = 1500, TaxRate = 19, StockOnHand = 120, ReorderPoint = 25, CategoryId = categorias[2].Id, UnitMeasureId = unidades[0].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Product { Name = "Doritos", UnitCost = 1200, UnitPrice = 2000, TaxRate = 19, StockOnHand = 100, ReorderPoint = 20, CategoryId = categorias[2].Id, UnitMeasureId = unidades[0].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },

                // Dulces
                new Product { Name = "Chocolatina Jet", UnitCost = 500, UnitPrice = 1000, TaxRate = 19, StockOnHand = 200, ReorderPoint = 40, CategoryId = categorias[3].Id, UnitMeasureId = unidades[0].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                new Product { Name = "Bon Bon Bum", UnitCost = 300, UnitPrice = 500, TaxRate = 19, StockOnHand = 300, ReorderPoint = 60, CategoryId = categorias[3].Id, UnitMeasureId = unidades[0].Id, Active = true, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
            };
            _context.products.AddRange(productos);
            await _context.SaveChangesAsync();

            // Crear ProductUnitPrices (precios por presentación)
            var productUnitPrices = new List<ProductUnitPrice>();
            foreach (var producto in productos)
            {
                // Precio por unidad (mismo que el precio base)
                productUnitPrices.Add(new ProductUnitPrice
                {
                    ProductId = producto.Id,
                    UnitMeasureId = unidades[0].Id, // Unidad
                    UnitPrice = producto.UnitPrice,
                    UnitCost = producto.UnitCost,
                    ConversionFactor = 1 // 1 unidad = 1 unidad base
                });

                // Precio por caja (descuento del 10% por volumen)
                if (producto.CategoryId == categorias[0].Id || producto.CategoryId == categorias[1].Id) // Bebidas y cigarrillos
                {
                    productUnitPrices.Add(new ProductUnitPrice
                    {
                        ProductId = producto.Id,
                        UnitMeasureId = unidades[1].Id, // Caja
                        UnitPrice = producto.UnitPrice * 24 * 0.9m, // 24 unidades con 10% descuento
                        UnitCost = producto.UnitCost * 24 * 0.9m,
                        ConversionFactor = 24 // 1 caja = 24 unidades
                    });
                }
            }
            _context.productUnitPrices.AddRange(productUnitPrices);
            await _context.SaveChangesAsync();

            _logger.LogInformation("====================================");
            _logger.LogInformation("Datos de prueba creados exitosamente:");
            _logger.LogInformation("- {CategoriaCount} Categorías", categorias.Count);
            _logger.LogInformation("- {UnidadCount} Unidades de Medida", unidades.Count);
            _logger.LogInformation("- {ProveedorCount} Proveedores", proveedores.Count);
            _logger.LogInformation("- {ProductoCount} Productos", productos.Count);
            _logger.LogInformation("- {PrecioCount} Precios por Presentación", productUnitPrices.Count);
            _logger.LogInformation("====================================");
        }
    }
}
