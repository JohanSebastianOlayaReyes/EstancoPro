namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para Purchase con flujo de entrada de inventario
/// </summary>
public class PurchaseBusiness : BaseBusiness<Purchase, PurchaseDto>, IPurchaseBusiness
{
    private readonly IPurchaseData _purchaseData;
    private readonly ApplicationDbContext _context;

    public PurchaseBusiness(
        IPurchaseData purchaseData,
        ApplicationDbContext context,
        ILogger<BaseBusiness<Purchase, PurchaseDto>> logger)
        : base(purchaseData, logger)
    {
        _purchaseData = purchaseData;
        _context = context;
    }

    /// <summary>
    /// Recibe una compra: actualiza stock y opcionalmente registra pago en caja
    /// Flujo crítico 3.1: Entrada de inventario con impacto en stock y caja
    /// Validaciones críticas punto 4: No recibir si Status != Ordered, validar líneas
    /// </summary>
    public async Task ReceivePurchaseAsync(int purchaseId, bool payInCash = false, int? cashSessionId = null)
    {
        // Usar transacción para garantizar atomicidad
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Obtener la compra con sus líneas
            var purchase = await _context.purchases
                .Include(p => p.purchaseproductdetail)
                    .ThenInclude(ppd => ppd.product)
                .Include(p => p.purchaseproductdetail)
                    .ThenInclude(ppd => ppd.unitMeasure)
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            if (purchase == null)
            {
                throw new KeyNotFoundException($"No se encontró la compra con Id {purchaseId}");
            }

            // 2. Validar estado (debe estar en Ordered)
            if (purchase.status)
            {
                throw new InvalidOperationException("La compra ya fue recibida previamente");
            }

            // 3. Validar que tenga líneas
            if (!purchase.purchaseproductdetail.Any())
            {
                throw new InvalidOperationException("La compra no tiene líneas de productos");
            }

            // 4. Si se paga en efectivo, validar que haya sesión de caja abierta
            if (payInCash)
            {
                if (!cashSessionId.HasValue)
                {
                    throw new ArgumentException("Se requiere ID de sesión de caja para pagos en efectivo");
                }

                var cashSession = await _context.cashSessions.FindAsync(cashSessionId.Value);
                if (cashSession == null)
                {
                    throw new KeyNotFoundException($"No se encontró la sesión de caja con Id {cashSessionId}");
                }

                if (cashSession.ClosedAt != null)
                {
                    throw new InvalidOperationException("No se puede registrar pago en una sesión de caja cerrada");
                }
            }

            // 5. Actualizar stock por cada línea
            // Regla punto 2.1: stock += qty × ConversionFactor (en unidad base)
            foreach (var detail in purchase.purchaseproductdetail)
            {
                // Obtener el ConversionFactor del ProductUnitPrice
                var productUnitPrice = await _context.productUnitPrices
                    .FirstOrDefaultAsync(pup => pup.ProductId == detail.ProductId && pup.UnitMeasureId == detail.UnitMeasureId);

                decimal conversionFactor = productUnitPrice?.ConversionFactor ?? 1; // Si no existe, asumir 1:1

                // Calcular cantidad en unidad base
                var quantityInBaseUnits = detail.Quantity * conversionFactor;

                // Actualizar stock del producto
                detail.product.StockOnHand += (int)quantityInBaseUnits;
                detail.product.UpdateAt = DateTime.UtcNow;

                // TODO: Opcionalmente recalcular costo base ponderado del producto
                // CostoPromedio = ((StockAnterior × CostoAnterior) + (CantidadNueva × CostoNuevo)) / StockNuevo
            }

            // 6. Marcar compra como recibida
            purchase.status = true;
            purchase.ReceivedAt = DateTime.UtcNow;
            purchase.UpdateAt = DateTime.UtcNow;

            // 7. Si se paga en efectivo, crear movimiento de caja (salida)
            if (payInCash && cashSessionId.HasValue)
            {
                var cashMovement = new CashMovement
                {
                    CashSessionId = cashSessionId.Value,
                    Type = "PurchasePayment",
                    Amount = purchase.TotalCost,
                    Reason = $"Pago compra #{purchase.Id}",
                    At = DateTime.UtcNow,
                    RelatedId = purchase.Id,
                    RelatedEntity = "Purchase"
                };

                await _context.cashMovements.AddAsync(cashMovement);
            }

            // 8. Guardar todos los cambios
            await _context.SaveChangesAsync();

            // 9. Confirmar transacción
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            // Revertir transacción en caso de error
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Cancela una compra en estado Ordered
    /// Validación crítica punto 4: No cancelar si ya fue recibida
    /// </summary>
    public async Task CancelPurchaseAsync(int purchaseId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Debe especificar la razón de la cancelación");
        }

        var purchase = await _context.purchases.FindAsync(purchaseId);

        if (purchase == null)
        {
            throw new KeyNotFoundException($"No se encontró la compra con Id {purchaseId}");
        }

        // Validar que esté en estado Ordered (status = false)
        if (purchase.status)
        {
            throw new InvalidOperationException("No se puede cancelar una compra que ya fue recibida");
        }

        // Marcar como cancelada (usando DeleteAt como flag de cancelación)
        purchase.DeleteAt = DateTime.UtcNow;
        purchase.Active = false;
        purchase.UpdateAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // TODO: Registrar razón de cancelación en tabla de auditoría
    }

    /// <summary>
    /// Obtiene compras filtradas por nombre de proveedor
    /// Frontend trabaja con nombres (regla UX)
    /// </summary>
    public async Task<IEnumerable<PurchaseDto>> GetBySupplierNameAsync(string supplierName)
    {
        if (string.IsNullOrWhiteSpace(supplierName))
        {
            throw new ArgumentException("El nombre del proveedor no puede estar vacío");
        }

        var purchases = await _context.purchases
            .Include(p => p.supplier)
            .Where(p => p.supplier.Name == supplierName && p.DeleteAt == null)
            .Select(p => new PurchaseDto
            {
                Id = p.Id,
                OrderedAt = p.OrderedAt,
                ReceivedAt = p.ReceivedAt,
                Status = p.status,
                TotalCost = p.TotalCost,
                SupplierId = p.SupplierId,
                Active = p.Active,
                CreateAt = p.CreateAt,
                UpdateAt = p.UpdateAt,
                DeleteAt = p.DeleteAt
            })
            .ToListAsync();

        return purchases;
    }

    /// <summary>
    /// Obtiene compras en un rango de fechas
    /// Punto 6: Reporte de compras por periodo
    /// </summary>
    public async Task<IEnumerable<PurchaseDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var purchases = await _context.purchases
            .Where(p => p.OrderedAt >= from && p.OrderedAt <= to && p.DeleteAt == null)
            .Select(p => new PurchaseDto
            {
                Id = p.Id,
                OrderedAt = p.OrderedAt,
                ReceivedAt = p.ReceivedAt,
                Status = p.status,
                TotalCost = p.TotalCost,
                SupplierId = p.SupplierId,
                Active = p.Active,
                CreateAt = p.CreateAt,
                UpdateAt = p.UpdateAt,
                DeleteAt = p.DeleteAt
            })
            .OrderByDescending(p => p.OrderedAt)
            .ToListAsync();

        return purchases;
    }

    /// <summary>
    /// Obtiene compras por estado
    /// true = recibidas, false = ordenadas/pendientes
    /// </summary>
    public async Task<IEnumerable<PurchaseDto>> GetByStatusAsync(bool status)
    {
        var purchases = await _context.purchases
            .Where(p => p.status == status && p.DeleteAt == null)
            .Select(p => new PurchaseDto
            {
                Id = p.Id,
                OrderedAt = p.OrderedAt,
                ReceivedAt = p.ReceivedAt,
                Status = p.status,
                TotalCost = p.TotalCost,
                SupplierId = p.SupplierId,
                Active = p.Active,
                CreateAt = p.CreateAt,
                UpdateAt = p.UpdateAt,
                DeleteAt = p.DeleteAt
            })
            .OrderByDescending(p => p.OrderedAt)
            .ToListAsync();

        return purchases;
    }
}
