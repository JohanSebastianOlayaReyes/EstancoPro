# Informe de pruebas unitarias — Backend

Última actualización: 2025-11-10

Este documento describe cómo están organizadas y por qué se implementaron las pruebas unitarias en este repositorio, qué decisiones técnicas tomé y cómo ejecutar/verificar las pruebas localmente.

## Objetivo

- Tener cobertura de pruebas unitarias para las capas Data, Business y Controller del backend.
- Usar herramientas ligeras y deterministas para que las pruebas se ejecuten rápidamente en CI/local.

## Resumen de la estrategia

- Controller.Tests: pruebas unitarias que NO arrancan el servidor ni la base de datos; se utiliza Moq para simular la capa Business y comprobar respuestas (Ok, NotFound, BadRequest, etc.). Esto mantiene las pruebas rápidas y enfocadas al controlador.
- Business.Tests: combinación de técnicas según la complejidad:
  - Para métodos que delegan y no usan EF internamente, usamos Moq sobre las interfaces de Data.
  - Para métodos que contienen lógica que consulta o modifica el DbContext (por ejemplo, ajustes de stock, sesiones de caja), usamos una instancia en memoria de `ApplicationDbContext`. Dependiendo del test se usó `UseInMemoryDatabase` o SQLite in-memory para cubrir escenarios con comportamiento más cercano a EF real.
- Data.Tests: pruebas de integración ligera de la capa de datos usando `Microsoft.EntityFrameworkCore.InMemory` y una configuración mínima de AutoMapper cuando la implementación lo requiere. Así comprobamos los CRUD y consultas sin tocar SQL Server.

Decisión clave: separar los tipos de pruebas según su responsabilidad. Evitamos arrancar la app Web en tests unitarios para prevenir problemas de locking y mantener las pruebas rápidas y reproducibles.

## Herramientas y paquetes usados

- xUnit: framework de pruebas.
- Moq: mocking de interfaces en Controllers y en Business cuando conviene.
- Microsoft.EntityFrameworkCore.InMemory (versión alineada con el proyecto Entity: 9.0.10): para pruebas de Data y Business que necesitan un DbContext en memoria.
- Microsoft.Data.Sqlite + Microsoft.EntityFrameworkCore.Sqlite (cuando se usa Sqlite in-memory en tests de Business para reproducir mejor ciertas acciones de EF Core).
- AutoMapper (sólo en los tests de Data donde la implementación lo requiere).

## Cómo se organizan los tests

- Backend/Tests/Business.Tests  — pruebas de la capa Business (mix Moq + EF InMemory / Sqlite in-memory).
- Backend/Tests/Data.Tests      — pruebas de la capa Data (EF InMemory + AutoMapper).
- Backend/Tests/Controller.Tests— pruebas de los controladores (Moq para la capa Business).

Cada proyecto de tests tiene su propio .csproj con referencias a los paquetes necesarios. Para reducir ruido repetido, añadí un `GlobalUsings.cs` en `Business.Tests` que exporta tipos comunes (Entity.Model, Entity.Dto, Business.Implementations.Base, EF Core, AutoMapper, Logging).

### Informe de pruebas unitarias — Backend (actualizado)

Última actualización: 2025-11-11

Este documento describe, en detalle y en español, qué herramientas y técnicas se utilizaron en las pruebas del backend (Data, Business, Controller), por qué existen archivos de utilidad como `TestUtilities` o `GlobalUsings`, qué pruebas se añadieron/ajustaron y cómo ejecutar/validar los resultados localmente.

## Resumen ejecutivo

- Objetivo: cobertura automática y reproducible de las capas Data, Business y Controller usando xUnit y herramientas complementarias (Moq, AutoMapper, EF InMemory/Sqlite). Las pruebas se diseñaron para ser rápidas, deterministas y lo menos intrusivas posible con el código productivo.
- Estado actual (local):
  - Data.Tests: 42/42 ✔
  - Business.Tests: 58/58 ✔
  - Controller.Tests: 66/66 ✔

## Qué usamos por tipo de prueba y por qué

1) Data.Tests (capa Data)
  - Objetivo: validar implementaciones de acceso a datos (CRUD, consultas, constraints, soft-delete) sin depender de un servidor de BD externo.
  - Herramientas: xUnit + Microsoft.EntityFrameworkCore.InMemory + AutoMapper (inicializado con los mappings necesarios para DTO↔Model).
  - Por qué InMemory: ejecución rápida y suficiente para comprobar la lógica de consultas y operaciones de datos. Para ciertas entidades con comportamiento relacional complejo se revisó la semántica (p. ej. claves compuestas) y se adaptaron los tests.
  - Patrón recurrente en tests de Data:
    - Crear un `ApplicationDbContext` in-memory con nombre único por test (evita interferencias entre tests).
    - Seed de entidades relacionadas (FKs) antes de crear la entidad bajo prueba (ej: Purchase necesita Supplier; UserRol necesita User + Rol).
    - Verificar Create, GetAll, GetById y DeleteLogicAsync (patrón de borrado lógico usado en el proyecto).
  - Ejemplos añadidos recientemente: `PurchaseDataTests.cs`, `CashSessionDataTests.cs`, `RolUserDataTests.cs` (UserRol con clave compuesta — ver nota especial más abajo).

2) Business.Tests (capa Business / lógica de negocio)
  - Objetivo: validar reglas de negocio, validaciones y orquestación entre repositorios/DAOs y utilidades.
  - Herramientas: xUnit + Moq (para simular la capa Data cuando procede) + EF InMemory o SQLite in-memory en tests que requieren semántica relacional real.
  - Por qué SQLite in-memory: algunos comportamientos de EF (constrained inserts, claves compuestas, tracking y conversiones) se reproducen más fielmente con SQLite que con el proveedor InMemory; se usó sólo cuando era necesario.
  - Patrones:
    - Tests unitarios puros con Moq para métodos que delegan en Data.
    - Tests de integración ligera (InMemory/SQLite) para flujos que realizan varias operaciones EF y donde la semántica importa.

3) Controller.Tests (capa Web / API)
  - Objetivo: validar rutas, códigos de estado HTTP y enlaces con la capa Business sin arrancar el servidor.
  - Herramientas: xUnit + Moq para simular la capa Business; Assert sobre IActionResult y códigos HTTP esperados.
  - Por qué Moq: aislar el controlador permite probar respuestas 200/201/404/400 y el manejo de errores rápidamente, sin dependencias externas.

## Archivos de utilidad y decisiones de diseño

- `TestUtilities` (ubicado en `Backend/Tests/Data.Tests/`):
  - Propósito: fábrica/un helper para crear un `ApplicationDbContext` en memoria con un nombre único y para construir un `IMapper` configurado con todos los mapeos usados en tests.
  - Por qué: evita duplicar la inicialización del contexto y los mappings en cada test; centraliza cambios cuando se añaden DTOs o mappings nuevos.

- `GlobalUsings.cs` (en proyectos de tests, p. ej. `Business.Tests`):
  - Propósito: exportar usings comunes para reducir ruido en los archivos de test (Entity.Model, Entity.Dto, Xunit, Moq, EF Core, etc.).
  - Por qué: mejora la legibilidad y mantiene los archivos de test enfocados en la lógica del test.

- TRX y TestResults:
  - Cuando ejecutas `dotnet test --logger "trx;LogFileName=..."`, los resultados TRX quedan en `Backend\Tests\<Project>\TestResults\`.
  - Incluyo los nombres de TRX generados durante la sesión para que puedas subirlos como artefactos en CI si quieres.

## Notas especiales y ejemplos reales aplicados

- Soft delete: el proyecto usa `DeleteLogicAsync` (borrado lógico). Los tests de Data verifican que, tras llamar a `DeleteLogicAsync`, `GetByIdAsync` lanza una excepción (KeyNotFoundException) o no devuelve la entidad, según la implementación.

- Clave compuesta (UserRol):
  - `UserRol` está definido con propiedades `UserId` y `RolId` y actúa como clave compuesta. La clase `RolUserData` hereda de `BaseData< UserRol, UserRolDto>`; sin embargo, los métodos genéricos de `BaseData` esperan un Id entero para `GetByIdAsync`, `UpdateAsync` y `DeleteLogicAsync`.
  - Decisión: en los tests de Data para `UserRol` se validó `CreateAsync`, `GetAllAsync` y la restricción de duplicados (Entity Framework lanza `InvalidOperationException` si intentas adjuntar dos instancias con la misma clave compuesta). No intentamos forzar `UpdateAsync(id, dto)`/`DeleteLogicAsync(id)` con un id entero porque la entidad no usa esa forma de identificación.

## Lista (selecta) de tests añadidos / modificados

Nota: se listan los archivos más relevantes añadidos/modificados durante el trabajo de pruebas. Hay más tests menores y utilidades en los tres proyectos, pero estos son los más significativos:

- Backend/Tests/Data.Tests/
  - `PurchaseDataTests.cs` — CRUD de `Purchase` (seed `Supplier`).
  - `CashSessionDataTests.cs` — CRUD de `CashSession`.
  - `RolUserDataTests.cs` — Create/GetAll/duplicados para `UserRol` (clave compuesta).

- Backend/Tests/Business.Tests/
  - `SupplierBusinessTests.cs` — pruebas de lógica del negocio de proveedores (consolidación y fixes).
  - `PurchaseBusinessTests.cs` — pruebas sobre recepción/cancelación/validaciones (mix Moq + in-memory cuando aplica).
  - `CashSessionBusinessTests.cs` — reglas de apertura/cierre de sesiones.

- Backend/Tests/Controller.Tests/
  - `UnitMeasureControllerTests.cs` — GetAll/GetById(Create) tests.
  - `ProductUnitPriceControllerTests.cs` — GetAll/GetById/Create y ruta de precios.

Si quieres la lista completa por archivo, puedo generar `TESTS_SUMMARY.md` con cada archivo y su propósito.

## Resultados de ejecución (comandos y local paths)

- Comandos ejecutados localmente (PowerShell):

```powershell
# Data
dotnet test "Backend\Tests\Data.Tests\Data.Tests.csproj" --logger "trx;LogFileName=Data.Tests_add_userrol_final.trx"

# Business
dotnet test "Backend\Tests\Business.Tests\Business.Tests.csproj" --logger "trx;LogFileName=Business.Tests_full.trx"

# Controller
dotnet test "Backend\Tests\Controller.Tests\Controller.Tests.csproj" --logger "trx;LogFileName=Controller.Tests_verify.trx"
```

- Resultados (local, en la sesión):
  - `Backend\Tests\Data.Tests`: 42 passed, 0 failed
  - `Backend\Tests\Business.Tests`: 58 passed, 0 failed
  - `Backend\Tests\Controller.Tests`: 66 passed, 0 failed

Los TRX generados se encuentran en las carpetas `TestResults` de cada proyecto.

## Calidad / Quality Gates (estado rápido)

- Build: PASS (la solución compila localmente durante las ejecuciones de tests).
- Lint / Typecheck: PASS con advertencias — hay múltiples advertencias de nullabilidad en `Entity` y algunos warnings en Utilities (JwtService). No hay errores de compilación.
- Tests: PASS (todos los tests de Data/Business/Controller pasaron en mi ejecución local durante esta sesión).

## Cómo reproducir localmente (PowerShell en Windows)

1. Parar cualquier proceso `dotnet run` que pueda bloquear assemblies (por ejemplo la Web en ejecución).
2. Desde la raíz del repo (o desde `Backend\Tests\<Project>`):

```powershell
# Ejecutar un proyecto de tests
dotnet test "Backend\Tests\Data.Tests\Data.Tests.csproj"

# Ejecutar todos los tests del backend (más lento)
dotnet test "Backend\EstancoPro.sln"
```

3. Para obtener archivos TRX:

```powershell
dotnet test "Backend\Tests\Data.Tests\Data.Tests.csproj" --logger "trx;LogFileName=Data.Tests_full.trx"
```

## Siguientes pasos recomendados (opciones)

1. (Opcional) Generar `TESTS_SUMMARY.md` con la lista completa de tests y enlaces a los TRX — lo puedo generar automáticamente.
2. Añadir cobertura con `coverlet` y reportes (ReportGenerator) en CI.
3. Añadir un pipeline CI (GitHub Actions/Azure Pipelines) que ejecute `dotnet test` y publique artefactos TRX y cobertura.

---

¿Quieres que ahora:

- A) Genere `TESTS_SUMMARY.md` con cada archivo y propósito (recomendado),
- B) Añada pasos CI básicos (workflow de GH Actions) para ejecutar tests y publicar TRX y cobertura, o
- C) haga otra cosa que me indiques?
