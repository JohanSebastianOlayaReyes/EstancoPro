-- Script para eliminar usuarios existentes y permitir que se recreen con contraseñas hasheadas

USE SecurityModel;
GO

-- Eliminar refresh tokens
DELETE FROM refresh_tokens;

-- Eliminar relaciones UserRol
DELETE FROM user_rols;

-- Eliminar usuarios
DELETE FROM users;

-- Eliminar personas (excepto las que tengan otras relaciones)
DELETE FROM persons WHERE Id IN (
    SELECT p.Id
    FROM persons p
    LEFT JOIN users u ON p.Id = u.PersonId
    WHERE u.Id IS NULL
);

PRINT 'Usuarios eliminados exitosamente. Reinicia el backend para recrearlos con contraseñas hasheadas.';
