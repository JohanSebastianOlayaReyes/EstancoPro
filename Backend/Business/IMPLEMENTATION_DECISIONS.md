# Decisiones de implementación: por qué algunas clases NO heredan `BaseBusiness<T,D>`

Este documento explica por qué ciertas clases de la capa `Business` no heredan de la implementación base `BaseBusiness<T,D>` mientras que la mayoría sí lo hacen.

## Resumen

- `BaseBusiness<T,D>` provee una implementación genérica para operaciones CRUD que asume una clave simple `int id` y depende de la interfaz `IBaseData<T,D>` con métodos como `GetByIdAsync(int id)`, `UpdateAsync(int id, D dto)`, etc.
- Algunas entidades del dominio usan claves compuestas (por ejemplo: combinaciones de `saleId`, `productId`, `unitMeasureId`, o `cashSessionId` + `DateTime`). Para esas entidades existen interfaces y data-layer específicas con firmas distintas (por ejemplo `GetByIdAsync(int saleId, int productId, int unitMeasureId)`).

Por ello, las clases que trabajan con llaves compuestas NO pueden heredar directamente `BaseBusiness<T,D>` sin cambiar antes los contratos de datos.

## Casos concretos

- `CashMovementBusiness` — implementa `ICashMovementBusiness` y usa `ICashMovementData`.
  - Motivo: las operaciones usan una llave compuesta `(cashSessionId, DateTime at)`. `BaseBusiness` asume una clave simple `int id`, por lo que no encaja.

- `SaleProductDetailBusiness` — implementa `ISaleProductDetailBusiness` y usa `ISaleProductDetailData`.
  - Motivo: llave compuesta `(saleId, productId, unitMeasureId)`. Firmas y reglas específicas requieren interfaz de datos con métodos compuestos.

- `ProductUnitPriceBusiness` — implementa `IProductUnitPriceBusiness`.
  - Motivos: además de CRUD con clave compuesta `(productId, unitMeasureId)`, esta clase contiene métodos específicos que usan `ApplicationDbContext` para consultas (por ejemplo `GetByProductNameAsync` o `GetPriceByNamesAsync`). Su lógica es más rica y no encaja con el contrato mínimo de `BaseBusiness`.

## Opciones para unificar (y por qué no se aplicaron automáticamente)

1. Adaptar las interfaces de data para soportar claves compuestas y que, además, implementen `IBaseData<T,D>` (por ejemplo añadir sobrecargas con `int id` o alguna abstracción). Esto es un cambio mayor: afecta Data, Business y tests.

2. Crear una clase base especializada para llaves compuestas (p.ej. `CompositeBaseBusiness`) que centralice logging y manejo de errores, dejando las firmas concretas en cada interfaz. Esto reduce duplicación sin tocar la capa Data. Esta es una solución intermedia y de bajo riesgo.

3. Mantener la separación actual: cada clase especial mantiene su propia implementación. Es el enfoque más conservador y el que actualmente está en uso.

## Recomendación

Si se busca reducir duplicación sin refactorizar toda la capa Data, la opción 2 es la más práctica: añadir una pequeña base con helpers (logging/WrapAsync) que las implementaciones con llaves compuestas puedan reutilizar.

Si se quiere un contrato unificado a largo plazo y normalizar convenciones, la opción 1 es la más limpia pero es más costosa y debe planificarse.

---
Archivo generado automáticamente para documentar la elección actual. Si quieres, puedo:  
- aplicar la opción 2 (añadir base auxiliar y usarla en las clases compuestas), o  
- preparar un plan de refactor (opción 1) con lista de archivos, tests a actualizar y riesgos.
