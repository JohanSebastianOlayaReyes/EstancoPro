## Resumen rápido — qué falta probar y cómo verificar

Este documento lista las áreas del backend que aún requieren pruebas unitarias (Data / Business / Controller) y muestra los comandos en PowerShell para ejecutar y verificar las pruebas.

--

## Estado actual (breve)
- Los proyectos de pruebas que ya contienen tests: `Backend/Tests/Business.Tests`, `Backend/Tests/Data.Tests`, `Backend/Tests/Controller.Tests`.
- He añadido tests para: Category, Form, Rol (Data + Controller), PurchaseProductDetail (Business + Controller), y otros. Varios tests de Business ya están implementados.

Si quieres un inventario exhaustivo por fichero, lo genero en la próxima iteración.

--

## Prioridad: lista de cosas que faltan (ordenada por impacto / API pública)

1) Business — alta prioridad
  - `PurchaseBusiness` (completitud): aún requiere pruebas end-to-end de sus flujos principales:
    - ReceivePurchaseAsync: happy path (actualiza stock, opcional registro en caja), y errores (compra no encontrada, estado inválido, cash session faltante cuando payInCash=true).
    - CancelPurchaseAsync: happy path + invalid state + not found.
    - GetBySupplierNameAsync / GetByDateRangeAsync / GetByStatusAsync: casos felices y validaciones.

2) Controllers — alta/mediana prioridad (APIs públicas sin tests)
  - `AuthController` (login/refresh) — validar respuestas 200 y 401/400.
  - `CashSessionController` — iniciar/cerrar sesión de caja.
  - `CashMovementController` — crear y listar movimientos en caja.
  - `ProductUnitPriceController` — endpoints de precios y conversiones.
  - `UnitMeasureController`, `SupplierController`, `PersonControllers`, `RolFormPermissionControllers`, `UserRolControllers`, `FormModuleControllers`, `ModuleControllers`, `PermissionControllers`, `SaleProductDetailController` — faltan tests básicos (GetAll, GetById not-found, Create).

3) Data layer — mediana prioridad
  - `RolFormPermissionData`, `UserRolData`, `PermissionData`, `ModuleData`, `FormModuleData`, `CashSessionData` — agregar tests con EF InMemory que cubran Create/GetById/GetAll and constraints (agregar seed de FK cuando aplique).

4) Business layer — otras entidades (lower priority but important)
  - `RolBusiness` — completar casos de Update/Delete/GetAll si faltan.
  - `CashSessionBusiness` & `CashMovementBusiness` — flujos de caja (abrir/cerrar sesión, registrar movimientos) necesitan tests que verifiquen cálculos y efectos en DB.

5) End-to-end test decision (optional)
  - Para los flujos transaccionales que tocan stock y caja (p. ej. `ReceivePurchaseAsync`), usar SQLite in-memory en los tests de Business para verificar FKs y transacciones.

--

## Convenciones y recomendaciones rápidas
- Controllers: usar Moq para el `I*Business` y verificar IActionResult (Ok / NotFound / BadRequest / ObjectResult 201 / NoContent).
- Business: usar Moq para `I*Data` cuando la lógica delegue; si la lógica manipula `DbContext` (transacciones, updates complejos), usar SQLite in-memory o un ApplicationDbContext en memoria con `UseSqlite("DataSource=:memory:")` y abrir la conexión.
- Data: usar `Microsoft.EntityFrameworkCore.InMemory` y `AutoMapper` (ya hay `TestUtilities.CreateInMemoryContext()` y `CreateMapper()` en `Backend/Tests/Data.Tests/TestUtilities.cs`).

--

## Cómo correr las pruebas (PowerShell / pwsh)

1) Ejecutar todas las pruebas de la solución (ejecuta los tres proyectos de tests y cualquier otro test si existe):

```powershell
dotnet test "c:\Users\jsola\Desktop\ADSO\EstancoPro\Backend\EstancoPro.sln"
```

2) Ejecutar solo un proyecto de tests (recomendado mientras desarrollas):

```powershell
# Business
dotnet test "c:\Users\jsola\Desktop\ADSO\EstancoPro\Backend\Tests\Business.Tests\Business.Tests.csproj" --logger "trx;LogFileName=Business.Tests_run.trx"
# Data
dotnet test "c:\Users\jsola\Desktop\ADSO\EstancoPro\Backend\Tests\Data.Tests\Data.Tests.csproj" --logger "trx;LogFileName=Data.Tests_run.trx"
# Controller
dotnet test "c:\Users\jsola\Desktop\ADSO\EstancoPro\Backend\Tests\Controller.Tests\Controller.Tests.csproj" --logger "trx;LogFileName=Controller.Tests_run.trx"
```

3) Ejecutar una sola clase de tests (filtro por nombre totalmente calificado):

```powershell
dotnet test "c:\Users\jsola\Desktop\ADSO\EstancoPro\Backend\Tests\Business.Tests\Business.Tests.csproj" --filter FullyQualifiedName~Business.Tests.PurchaseBusinessTests
```

4) Ejecutar un test específico (por su nombre):

```powershell
dotnet test "c:\Users\jsola\Desktop\ADSO\EstancoPro\Backend\Tests\Controller.Tests\Controller.Tests.csproj" --filter FullyQualifiedName~Controller.Tests.PurchaseControllerTests.ReceivePurchase_Succeeds_ReturnsOk
```

--

## Dónde revisar resultados / artefactos
- Cada `dotnet test` genera un TRX si pasas la opción `--logger "trx;LogFileName=NAME.trx"`.
- Ejemplos de rutas (desde workspace):
  - `Backend/Tests/Business.Tests/TestResults/Business.Tests_run.trx`
  - `Backend/Tests/Data.Tests/TestResults/Data.Tests_run.trx`
  - `Backend/Tests/Controller.Tests/TestResults/Controller.Tests_run.trx`

Abre el `.trx` con el Visual Studio Test Results viewer o examínalo como XML para validar conteos (total/passed/failed).

--

## Siguiente paso recomendado (por mi parte)
- Puedo implementar la batería siguiente (elige una):
  1. Terminar `PurchaseBusiness` (tests completos con SQLite in-memory para flujos transaccionales).  
  2. Añadir tests para los controladores faltantes más usados: `AuthController`, `CashSessionController`, `CashMovementController`, `ProductUnitPriceController`.  
  3. Crear tests Data para `RolFormPermissionData`, `UserRolData`, `PermissionData`.

Indica qué opción prefieres (o pon tu prioridad) y me encargo de implementarlos y ejecutar la suite tras cada lote.

---

Archivo generado: `Backend/TESTS_MISSING.md` — si quieres lo copio también en `TESTS_REPORT.md` y lo actualizo con los TRX de las ejecuciones ya realizadas.
