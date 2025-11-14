-- ================================================================
-- SCRIPT DE INICIALIZACIÓN COMPLETA - ESTANCOPRO
-- ================================================================
-- Este script inicializa todo el sistema con datos base:
-- 1. Permisos (CRUD + Execute + Export)
-- 2. Roles (Administrador, Cajero, Vendedor, Inventario, Gerente)
-- 3. Módulos (Ventas, Inventario, Compras, Caja, Administración)
-- 4. Formularios (Pantallas del sistema)
-- 5. FormModule (Relación formularios-módulos)
-- 6. RolFormPermission (Asignación de permisos por rol)
-- 7. PaymentMethods (Métodos de pago)
-- 8. Categorías base
-- 9. Unidades de medida
-- ================================================================

USE EstancoPro;
GO

-- ================================================================
-- 1. PERMISOS
-- ================================================================
PRINT '📋 Creando Permisos...';

INSERT INTO permissions (TypePermission, Description, Active, CreateAt) VALUES
('Create', 'Crear nuevos registros', 1, GETDATE()),
('Read', 'Ver y consultar información', 1, GETDATE()),
('Update', 'Modificar registros existentes', 1, GETDATE()),
('Delete', 'Eliminar registros (soft delete)', 1, GETDATE()),
('Execute', 'Ejecutar acciones especiales (finalizar venta, cerrar caja, etc.)', 1, GETDATE()),
('Export', 'Exportar información a Excel, PDF, etc.', 1, GETDATE());

PRINT '✅ Permisos creados: 6';
GO

-- ================================================================
-- 2. ROLES
-- ================================================================
PRINT '👥 Creando Roles...';

INSERT INTO rols (TypeRol, Description, Active, CreateAt) VALUES
('Administrador', 'Control total del sistema. Acceso a todas las funcionalidades.', 1, GETDATE()),
('Cajero', 'Responsable de ventas y manejo de caja. Puede abrir/cerrar sesiones.', 1, GETDATE()),
('Vendedor', 'Realiza ventas pero NO maneja caja. Depende de sesión abierta.', 1, GETDATE()),
('Inventario', 'Gestiona productos, categorías, compras y stock. NO acceso a ventas.', 1, GETDATE()),
('Gerente', 'Supervisión y reportes. Solo lectura en todo el sistema.', 1, GETDATE());

PRINT '✅ Roles creados: 5';
GO

-- ================================================================
-- 3. MÓDULOS
-- ================================================================
PRINT '📦 Creando Módulos...';

INSERT INTO modules (Name, Description, Icon, [Order], Active, CreateAt) VALUES
('Ventas', 'Módulo de punto de venta y gestión de ventas', 'shopping-cart', 1, 1, GETDATE()),
('Inventario', 'Gestión de productos, categorías y stock', 'box', 2, 1, GETDATE()),
('Compras', 'Gestión de compras y proveedores', 'truck', 3, 1, GETDATE()),
('Caja', 'Control de caja y movimientos de efectivo', 'dollar-sign', 4, 1, GETDATE()),
('Administración', 'Configuración, usuarios y roles', 'settings', 5, 1, GETDATE()),
('Reportes', 'Reportes y estadísticas del negocio', 'bar-chart', 6, 1, GETDATE());

PRINT '✅ Módulos creados: 6';
GO

-- ================================================================
-- 4. FORMULARIOS (Pantallas del sistema)
-- ================================================================
PRINT '🖥️  Creando Formularios...';

INSERT INTO forms (Name, Description, Path, Active, CreateAt) VALUES
-- Módulo Ventas
('Dashboard', 'Pantalla principal con resumen del día', '/dashboard', 1, GETDATE()),
('POS', 'Punto de venta (pantalla crítica)', '/pos', 1, GETDATE()),

-- Módulo Inventario
('Productos', 'Gestión de productos', '/products', 1, GETDATE()),
('Categorías', 'Gestión de categorías', '/categories', 1, GETDATE()),
('Ajustes de Inventario', 'Ajustes manuales de stock', '/inventory-adjustments', 1, GETDATE()),
('Alertas de Stock', 'Alertas de productos con stock bajo', '/product-alerts', 1, GETDATE()),

-- Módulo Compras
('Compras', 'Gestión de órdenes de compra', '/purchases', 1, GETDATE()),
('Proveedores', 'Gestión de proveedores', '/suppliers', 1, GETDATE()),

-- Módulo Caja
('Caja', 'Apertura/cierre de sesiones de caja', '/cash', 1, GETDATE()),
('Movimientos de Caja', 'Consulta de movimientos de efectivo', '/cash-movements', 1, GETDATE()),
('Gastos', 'Registro de gastos operativos', '/expenses', 1, GETDATE()),

-- Módulo Administración
('Usuarios', 'Gestión de usuarios del sistema', '/admin/users', 1, GETDATE()),
('Roles', 'Gestión de roles y permisos', '/admin/roles', 1, GETDATE()),
('Métodos de Pago', 'Configuración de métodos de pago', '/admin/payment-methods', 1, GETDATE()),
('Clientes', 'Gestión de clientes', '/customers', 1, GETDATE()),

-- Módulo Reportes
('Reportes de Ventas', 'Reportes y gráficos de ventas', '/reports/sales', 1, GETDATE()),
('Reportes de Inventario', 'Reportes de stock y movimientos', '/reports/inventory', 1, GETDATE()),
('Reportes de Caja', 'Reportes de arqueos y cierre', '/reports/cash', 1, GETDATE()),
('Reportes Financieros', 'Utilidades, gastos e ingresos', '/reports/financial', 1, GETDATE());

PRINT '✅ Formularios creados: 19';
GO

-- ================================================================
-- 5. FORM_MODULES (Relación Formularios-Módulos)
-- ================================================================
PRINT '🔗 Relacionando Formularios con Módulos...';

-- Módulo Ventas
INSERT INTO form_modules (FormId, ModuleId, Active, CreateAt)
SELECT f.Id, m.Id, 1, GETDATE()
FROM forms f, modules m
WHERE f.Name IN ('Dashboard', 'POS') AND m.Name = 'Ventas';

-- Módulo Inventario
INSERT INTO form_modules (FormId, ModuleId, Active, CreateAt)
SELECT f.Id, m.Id, 1, GETDATE()
FROM forms f, modules m
WHERE f.Name IN ('Productos', 'Categorías', 'Ajustes de Inventario', 'Alertas de Stock') AND m.Name = 'Inventario';

-- Módulo Compras
INSERT INTO form_modules (FormId, ModuleId, Active, CreateAt)
SELECT f.Id, m.Id, 1, GETDATE()
FROM forms f, modules m
WHERE f.Name IN ('Compras', 'Proveedores') AND m.Name = 'Compras';

-- Módulo Caja
INSERT INTO form_modules (FormId, ModuleId, Active, CreateAt)
SELECT f.Id, m.Id, 1, GETDATE()
FROM forms f, modules m
WHERE f.Name IN ('Caja', 'Movimientos de Caja', 'Gastos') AND m.Name = 'Caja';

-- Módulo Administración
INSERT INTO form_modules (FormId, ModuleId, Active, CreateAt)
SELECT f.Id, m.Id, 1, GETDATE()
FROM forms f, modules m
WHERE f.Name IN ('Usuarios', 'Roles', 'Métodos de Pago', 'Clientes') AND m.Name = 'Administración';

-- Módulo Reportes
INSERT INTO form_modules (FormId, ModuleId, Active, CreateAt)
SELECT f.Id, m.Id, 1, GETDATE()
FROM forms f, modules m
WHERE f.Name IN ('Reportes de Ventas', 'Reportes de Inventario', 'Reportes de Caja', 'Reportes Financieros') AND m.Name = 'Reportes';

PRINT '✅ Relaciones Form-Module creadas';
GO

-- ================================================================
-- 6. ROL_FORM_PERMISSIONS - ADMINISTRADOR (TODO)
-- ================================================================
PRINT '🔐 Asignando permisos a ADMINISTRADOR...';

-- Administrador tiene TODOS los permisos en TODOS los formularios
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT r.Id, f.Id, p.Id, 1, GETDATE()
FROM rols r
CROSS JOIN forms f
CROSS JOIN permissions p
WHERE r.TypeRol = 'Administrador';

PRINT '✅ Administrador configurado (acceso total)';
GO

-- ================================================================
-- 6.2 ROL_FORM_PERMISSIONS - CAJERO
-- ================================================================
PRINT '🔐 Asignando permisos a CAJERO...';

DECLARE @CajeroId INT = (SELECT Id FROM rols WHERE TypeRol = 'Cajero');

-- Dashboard: Read
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @CajeroId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Dashboard' AND p.TypePermission = 'Read';

-- POS: Create, Read, Update, Delete, Execute
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @CajeroId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'POS' AND p.TypePermission IN ('Create', 'Read', 'Update', 'Delete', 'Execute');

-- Productos: Read (solo para consultar al vender)
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @CajeroId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Productos' AND p.TypePermission = 'Read';

-- Caja: Create, Read, Execute (abrir/cerrar)
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @CajeroId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Caja' AND p.TypePermission IN ('Create', 'Read', 'Execute');

-- Movimientos de Caja: Read
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @CajeroId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Movimientos de Caja' AND p.TypePermission = 'Read';

-- Clientes: Create, Read, Update (para registrar clientes al vender)
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @CajeroId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Clientes' AND p.TypePermission IN ('Create', 'Read', 'Update');

-- Reportes de Ventas: Read
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @CajeroId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Reportes de Ventas' AND p.TypePermission = 'Read';

PRINT '✅ Cajero configurado';
GO

-- ================================================================
-- 6.3 ROL_FORM_PERMISSIONS - VENDEDOR
-- ================================================================
PRINT '🔐 Asignando permisos a VENDEDOR...';

DECLARE @VendedorId INT = (SELECT Id FROM rols WHERE TypeRol = 'Vendedor');

-- Dashboard: Read
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @VendedorId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Dashboard' AND p.TypePermission = 'Read';

-- POS: Create, Read, Update, Delete, Execute (puede vender pero NO manejar caja)
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @VendedorId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'POS' AND p.TypePermission IN ('Create', 'Read', 'Update', 'Delete', 'Execute');

-- Productos: Read
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @VendedorId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Productos' AND p.TypePermission = 'Read';

-- Clientes: Create, Read, Update
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @VendedorId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Clientes' AND p.TypePermission IN ('Create', 'Read', 'Update');

-- Reportes de Ventas: Read (solo sus ventas)
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @VendedorId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Reportes de Ventas' AND p.TypePermission = 'Read';

PRINT '✅ Vendedor configurado';
GO

-- ================================================================
-- 6.4 ROL_FORM_PERMISSIONS - INVENTARIO
-- ================================================================
PRINT '🔐 Asignando permisos a INVENTARIO...';

DECLARE @InventarioId INT = (SELECT Id FROM rols WHERE TypeRol = 'Inventario');

-- Dashboard: Read
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @InventarioId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Dashboard' AND p.TypePermission = 'Read';

-- Productos: CRUD completo
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @InventarioId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Productos' AND p.TypePermission IN ('Create', 'Read', 'Update', 'Delete');

-- Categorías: CRUD completo
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @InventarioId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Categorías' AND p.TypePermission IN ('Create', 'Read', 'Update', 'Delete');

-- Ajustes de Inventario: CRUD + Execute
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @InventarioId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Ajustes de Inventario' AND p.TypePermission IN ('Create', 'Read', 'Update', 'Delete', 'Execute');

-- Alertas de Stock: Read, Update (marcar como leída)
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @InventarioId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Alertas de Stock' AND p.TypePermission IN ('Read', 'Update');

-- Compras: CRUD + Execute (recibir compra)
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @InventarioId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Compras' AND p.TypePermission IN ('Create', 'Read', 'Update', 'Delete', 'Execute');

-- Proveedores: CRUD completo
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @InventarioId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Proveedores' AND p.TypePermission IN ('Create', 'Read', 'Update', 'Delete');

-- Reportes de Inventario: Read, Export
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @InventarioId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name = 'Reportes de Inventario' AND p.TypePermission IN ('Read', 'Export');

PRINT '✅ Inventario configurado';
GO

-- ================================================================
-- 6.5 ROL_FORM_PERMISSIONS - GERENTE
-- ================================================================
PRINT '🔐 Asignando permisos a GERENTE...';

DECLARE @GerenteId INT = (SELECT Id FROM rols WHERE TypeRol = 'Gerente');

-- Gerente tiene Read y Export en TODOS los formularios (excepto administración de usuarios/roles)
INSERT INTO rol_form_permissions (RolId, FormId, PermissionId, Active, CreateAt)
SELECT @GerenteId, f.Id, p.Id, 1, GETDATE()
FROM forms f, permissions p
WHERE f.Name NOT IN ('Usuarios', 'Roles', 'Métodos de Pago')
  AND p.TypePermission IN ('Read', 'Export');

PRINT '✅ Gerente configurado (solo lectura y exportación)';
GO

-- ================================================================
-- 7. MÉTODOS DE PAGO
-- ================================================================
PRINT '💳 Creando Métodos de Pago...';

INSERT INTO payment_methods (Name, Type, RequiresReference, IsActive, Active, CreateAt) VALUES
('Efectivo', 'Cash', 0, 1, 1, GETDATE()),
('Tarjeta Débito', 'Card', 1, 1, 1, GETDATE()),
('Tarjeta Crédito', 'Card', 1, 1, 1, GETDATE()),
('Nequi', 'DigitalWallet', 1, 1, 1, GETDATE()),
('Daviplata', 'DigitalWallet', 1, 1, 1, GETDATE()),
('Transferencia Bancaria', 'Transfer', 1, 1, 1, GETDATE()),
('QR Bancolombia', 'DigitalWallet', 1, 1, 1, GETDATE());

PRINT '✅ Métodos de pago creados: 7';
GO

-- ================================================================
-- 8. CATEGORÍAS BASE
-- ================================================================
PRINT '📂 Creando Categorías base...';

INSERT INTO categories (Name, Description, Active, CreateAt) VALUES
('Bebidas', 'Gaseosas, jugos, agua, energizantes', 1, GETDATE()),
('Cigarrillos', 'Cigarrillos y tabaco', 1, GETDATE()),
('Snacks', 'Papas, dulces, galletas, chocolates', 1, GETDATE()),
('Aseo Personal', 'Jabones, shampoo, cepillos', 1, GETDATE()),
('Aseo Hogar', 'Detergentes, limpiadores', 1, GETDATE()),
('Panadería', 'Pan, pasteles, arepas', 1, GETDATE()),
('Lácteos', 'Leche, queso, yogurt', 1, GETDATE()),
('Abarrotes', 'Arroz, pasta, enlatados', 1, GETDATE()),
('Otros', 'Productos varios', 1, GETDATE());

PRINT '✅ Categorías creadas: 9';
GO

-- ================================================================
-- 9. UNIDADES DE MEDIDA
-- ================================================================
PRINT '📏 Creando Unidades de Medida...';

INSERT INTO unit_measures (Abbreviation, Name, Active, CreateAt) VALUES
('UN', 'Unidad', 1, GETDATE()),
('KG', 'Kilogramo', 1, GETDATE()),
('GR', 'Gramo', 1, GETDATE()),
('LT', 'Litro', 1, GETDATE()),
('ML', 'Mililitro', 1, GETDATE()),
('PQ', 'Paquete', 1, GETDATE()),
('CJ', 'Caja', 1, GETDATE()),
('DOC', 'Docena', 1, GETDATE()),
('BL', 'Bulto', 1, GETDATE());

PRINT '✅ Unidades de medida creadas: 9';
GO

-- ================================================================
-- RESUMEN FINAL
-- ================================================================
PRINT '';
PRINT '╔════════════════════════════════════════════════════════════╗';
PRINT '║  ✅ INICIALIZACIÓN COMPLETA - ESTANCOPRO                  ║';
PRINT '╠════════════════════════════════════════════════════════════╣';
PRINT '║  📋 Permisos: 6                                            ║';
PRINT '║  👥 Roles: 5                                               ║';
PRINT '║  📦 Módulos: 6                                             ║';
PRINT '║  🖥️  Formularios: 19                                        ║';
PRINT '║  🔗 Relaciones Form-Module creadas                         ║';
PRINT '║  🔐 Permisos asignados a todos los roles                   ║';
PRINT '║  💳 Métodos de pago: 7                                     ║';
PRINT '║  📂 Categorías: 9                                          ║';
PRINT '║  📏 Unidades de medida: 9                                  ║';
PRINT '╠════════════════════════════════════════════════════════════╣';
PRINT '║  🎯 Sistema listo para usar                                ║';
PRINT '║  📝 Recuerda ejecutar init-admin.sql para crear admin     ║';
PRINT '╚════════════════════════════════════════════════════════════╝';
PRINT '';
GO
