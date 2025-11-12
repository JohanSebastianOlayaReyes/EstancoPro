-- Script para crear el usuario Administrador inicial
-- Ejecutar este script en SQL Server Management Studio o Azure Data Studio

USE SecurityModel;
GO

-- 1. Crear el Rol de Administrador
IF NOT EXISTS (SELECT 1 FROM rols WHERE TypeRol = 'Administrador')
BEGIN
    INSERT INTO rols (TypeRol, Description, Active, CreateAt, UpdateAt)
    VALUES ('Administrador', 'Administrador del sistema con acceso completo', 1, GETUTCDATE(), GETUTCDATE());
    PRINT 'Rol Administrador creado exitosamente';
END
ELSE
BEGIN
    PRINT 'El rol Administrador ya existe';
END
GO

-- 2. Crear la Persona para el Administrador
DECLARE @PersonId INT;

IF NOT EXISTS (SELECT 1 FROM persons WHERE FirstName = 'Admin' AND FirstLastName = 'Sistema')
BEGIN
    INSERT INTO persons (FirstName, SecondName, FirstLastName, SecondLastName, PhoneNumber, NumberIdentification, Active, CreateAt, UpdateAt)
    VALUES ('Admin', NULL, 'Sistema', NULL, 0, 0, 1, GETUTCDATE(), GETUTCDATE());

    SET @PersonId = SCOPE_IDENTITY();
    PRINT 'Persona Admin creada exitosamente con ID: ' + CAST(@PersonId AS VARCHAR);
END
ELSE
BEGIN
    SELECT @PersonId = Id FROM persons WHERE FirstName = 'Admin' AND FirstLastName = 'Sistema';
    PRINT 'La persona Admin ya existe con ID: ' + CAST(@PersonId AS VARCHAR);
END
GO

-- 3. Crear el Usuario Administrador
DECLARE @PersonId INT;
DECLARE @RolId INT;

SELECT @PersonId = Id FROM persons WHERE FirstName = 'Admin' AND FirstLastName = 'Sistema';
SELECT @RolId = Id FROM rols WHERE TypeRol = 'Administrador';

IF NOT EXISTS (SELECT 1 FROM users WHERE Email = 'admin@estancopro.com')
BEGIN
    INSERT INTO users (Email, Password, PersonId, RolId, Active, CreateAt, UpdateAt)
    VALUES ('admin@estancopro.com', 'Admin123!', @PersonId, @RolId, 1, GETUTCDATE(), GETUTCDATE());

    PRINT 'Usuario Administrador creado exitosamente';
    PRINT '====================================';
    PRINT 'CREDENCIALES DEL ADMINISTRADOR:';
    PRINT 'Email: admin@estancopro.com';
    PRINT 'Password: Admin123!';
    PRINT '====================================';
END
ELSE
BEGIN
    PRINT 'El usuario admin@estancopro.com ya existe';
END
GO

-- Verificar que todo se creó correctamente
SELECT
    u.Id AS UserId,
    u.Email,
    u.Password,
    p.FirstName + ' ' + p.FirstLastName AS FullName,
    r.TypeRol AS Role,
    u.Active
FROM users u
INNER JOIN persons p ON u.PersonId = p.Id
INNER JOIN rols r ON u.RolId = r.Id
WHERE u.Email = 'admin@estancopro.com';
GO
