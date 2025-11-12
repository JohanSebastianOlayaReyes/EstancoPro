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
/// Capa de negocio para Sale con flujo completo de ventas POS
/// </summary>
public class SaleBusiness : BaseBusiness<Sale, SaleDto>, ISaleBusiness
{
    private readonly ISaleData _saleData;
    private readonly ApplicationDbContext _context;

    public SaleBusiness(
        ISaleData saleData,
        ApplicationDbContext context,
        ILogger<BaseBusiness<Sale, SaleDto>> logger)
        : base(saleData, logger)
    {
        _saleData = saleData;
        _context = context;
    }

    /// <summary>
    /// Finaliza una venta: valida stock, recalcula totales, descuenta inventario y registra en caja
    /// FLUJO MÁS CRÍTICO DEL SISTEMA - Punto 3.2
    /// Validaciones críticas punto 4:
    /// - Status debe ser Draft
    /// - Debe tener sesión de caja abierta
    /// - Stock suficiente con conversiones
    /// </summary>
    public async Task FinalizeSaleAsync(int saleId)
    {
        // Usar transacción para garantizar atomicidad (regla punto 4)
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Obtener la venta con sus líneas
            var sale = await _context.sales
                .Include(s => s.saleproductdetail)
                    .ThenInclude(spd => spd.product)
                .Include(s => s.saleproductdetail)
                    .ThenInclude(spd => spd.unitMeasure)
                .Include(s => s.cashSession)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null)
            {
                throw new KeyNotFoundException($"No se encontró la venta con Id {saleId}");
            }

            // 2. VALIDACIÓN: Status debe ser Draft
            if (sale.Status != "Draft")
            {
                throw new InvalidOperationException($"No se puede finalizar una venta con estado {sale.Status}");
            }

            // 3. VALIDACIÓN: Debe tener líneas
            if (!sale.saleproductdetail.Any())
            {
                throw new InvalidOperationException("La venta no tiene líneas de productos");
            }

            // 4. VALIDACIÓN: Debe tener sesión de caja asignada y abierta
            if (sale.CashSessionId == 0 || sale.cashSession == null)
            {
                throw new InvalidOperationException("La venta no tiene una sesión de caja asignada");
            }

            if (sale.cashSession.ClosedAt != null)
            {
                throw new InvalidOperationException("No se puede finalizar venta en una sesión de caja cerrada");
            }

            // 5. RECALCULAR TOTALES por línea y encabezado (Regla punto 2.3)
            decimal subtotal = 0;
            decimal taxTotal = 0;

            foreach (var detail in sale.saleproductdetail)
            {
                // Cálculos por línea
                detail.LineSubtotal = detail.Quantity * detail.UnitPrice;
                detail.LineTax = detail.LineSubtotal * detail.TaxRate;
                detail.LineTotal = detail.LineSubtotal + detail.LineTax;

                // Acumular para encabezado
                subtotal += detail.LineSubtotal;
                taxTotal += detail.LineTax;
            }

            // Totales del encabezado
            sale.Subtotal = subtotal;
            sale.TaxTotal = taxTotal;
            sale.GrandTotal = subtotal + taxTotal;

            // 6. VALIDAR STOCK SUFICIENTE con conversiones (Regla punto 2.1)
            foreach (var detail in sale.saleproductdetail)
            {
                // Obtener el ConversionFactor del ProductUnitPrice
                var productUnitPrice = await _context.productUnitPrices
                    .FirstOrDefaultAsync(pup => pup.ProductId == detail.ProductId && pup.UnitMeasureId == detail.UnitMeasureId);

                decimal conversionFactor = productUnitPrice?.ConversionFactor ?? 1; // Si no existe, asumir 1:1

                // Calcular cantidad requerida en unidad base
                var requiredBaseUnits = detail.Quantity * conversionFactor;

                // Validar stock disponible
                if (detail.product.StockOnHand < requiredBaseUnits)
                {
                    throw new InvalidOperationException(
                        $"Stock insuficiente para {detail.product.Name}. " +
                        $"Disponible: {detail.product.StockOnHand} unidades, " +
                        $"Requerido: {requiredBaseUnits} unidades " +
                        $"({detail.Quantity} × {detail.unitMeasure.Name})");
                }
            }

            // 7. DESCONTAR STOCK por cada línea
            foreach (var detail in sale.saleproductdetail)
            {
                // Obtener el ConversionFactor del ProductUnitPrice
                var productUnitPrice = await _context.productUnitPrices
                    .FirstOrDefaultAsync(pup => pup.ProductId == detail.ProductId && pup.UnitMeasureId == detail.UnitMeasureId);

                decimal conversionFactor = productUnitPrice?.ConversionFactor ?? 1; // Si no existe, asumir 1:1

                var quantityInBaseUnits = detail.Quantity * conversionFactor;
                detail.product.StockOnHand -= (int)quantityInBaseUnits;
                detail.product.UpdateAt = DateTime.UtcNow;
            }

            // 8. CREAR MOVIMIENTO DE CAJA (entrada por venta)
            var cashMovement = new CashMovement
            {
                CashSessionId = sale.CashSessionId,
                Type = "Sale",
                Amount = sale.GrandTotal,
                Reason = $"Venta #{sale.Id}",
                At = DateTime.UtcNow,
                RelatedId = sale.Id,
                RelatedEntity = "Sale"
            };

            await _context.cashMovements.AddAsync(cashMovement);

            // 9. MARCAR VENTA COMO COMPLETADA
            sale.Status = "Completed";
            sale.SoldAt = DateTime.UtcNow;
            sale.UpdateAt = DateTime.UtcNow;

            // 10. Guardar todos los cambios
            await _context.SaveChangesAsync();

            // 11. Confirmar transacción
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
    /// Cancela una venta en borrador (elimina líneas y venta)
    /// Validación punto 4: Solo cancelar si Status = Draft
    /// </summary>
    public async Task CancelSaleAsync(int saleId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var sale = await _context.sales
                .Include(s => s.saleproductdetail)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null)
            {
                throw new KeyNotFoundException($"No se encontró la venta con Id {saleId}");
            }

            // Validar que esté en Draft
            if (sale.Status != "Draft")
            {
                throw new InvalidOperationException($"No se puede cancelar una venta con estado {sale.Status}");
            }

            // Eliminar líneas de venta
            _context.saleProductDetails.RemoveRange(sale.saleproductdetail);

            // Marcar venta como cancelada
            sale.Status = "Cancelled";
            sale.DeleteAt = DateTime.UtcNow;
            sale.Active = false;
            sale.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Recalcula los totales de una venta (al agregar/editar líneas en carrito)
    /// Regla punto 2.3: Cálculo de totales por línea y encabezado
    /// Regla UX punto 7: Redondear por línea antes de sumar al encabezado
    /// </summary>
    public async Task RecalculateTotalsAsync(int saleId)
    {
        var sale = await _context.sales
            .Include(s => s.saleproductdetail)
            .FirstOrDefaultAsync(s => s.Id == saleId);

        if (sale == null)
        {
            throw new KeyNotFoundException($"No se encontró la venta con Id {saleId}");
        }

        decimal subtotal = 0;
        decimal taxTotal = 0;

        // Calcular totales por línea
        foreach (var detail in sale.saleproductdetail)
        {
            // Regla punto 2.3
            detail.LineSubtotal = Math.Round(detail.Quantity * detail.UnitPrice, 2);
            detail.LineTax = Math.Round(detail.LineSubtotal * detail.TaxRate, 2);
            detail.LineTotal = detail.LineSubtotal + detail.LineTax;

            subtotal += detail.LineSubtotal;
            taxTotal += detail.LineTax;
        }

        // Totales del encabezado
        sale.Subtotal = Math.Round(subtotal, 2);
        sale.TaxTotal = Math.Round(taxTotal, 2);
        sale.GrandTotal = sale.Subtotal + sale.TaxTotal;
        sale.UpdateAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene ventas de una sesión de caja específica
    /// Punto 6: Reporte de ventas por sesión
    /// </summary>
    public async Task<IEnumerable<SaleDto>> GetByCashSessionAsync(int cashSessionId)
    {
        var sales = await _context.sales
            .Where(s => s.CashSessionId == cashSessionId)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                SoldAt = s.SoldAt,
                Status = s.Status,
                Subtotal = s.Subtotal,
                TaxTotal = s.TaxTotal,
                GrandTotal = s.GrandTotal,
                CashSessionId = s.CashSessionId,
                Active = s.Active,
                CreateAt = s.CreateAt,
                UpdateAt = s.UpdateAt,
                DeleteAt = s.DeleteAt
            })
            .OrderByDescending(s => s.SoldAt)
            .ToListAsync();

        return sales;
    }

    /// <summary>
    /// Obtiene ventas en un rango de fechas
    /// Punto 6: Reporte de ventas por periodo
    /// </summary>
    public async Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var sales = await _context.sales
            .Where(s => s.SoldAt >= from && s.SoldAt <= to && s.Status == "Completed")
            .Select(s => new SaleDto
            {
                Id = s.Id,
                SoldAt = s.SoldAt,
                Status = s.Status,
                Subtotal = s.Subtotal,
                TaxTotal = s.TaxTotal,
                GrandTotal = s.GrandTotal,
                CashSessionId = s.CashSessionId,
                Active = s.Active,
                CreateAt = s.CreateAt,
                UpdateAt = s.UpdateAt,
                DeleteAt = s.DeleteAt
            })
            .OrderByDescending(s => s.SoldAt)
            .ToListAsync();

        return sales;
    }

    /// <summary>
    /// Obtiene ventas por estado
    /// Estados: Draft, Completed, Cancelled
    /// </summary>
    public async Task<IEnumerable<SaleDto>> GetByStatusAsync(string status)
    {
        var sales = await _context.sales
            .Where(s => s.Status == status)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                SoldAt = s.SoldAt,
                Status = s.Status,
                Subtotal = s.Subtotal,
                TaxTotal = s.TaxTotal,
                GrandTotal = s.GrandTotal,
                CashSessionId = s.CashSessionId,
                Active = s.Active,
                CreateAt = s.CreateAt,
                UpdateAt = s.UpdateAt,
                DeleteAt = s.DeleteAt
            })
            .OrderByDescending(s => s.CreateAt)
            .ToListAsync();

        return sales;
    }

    /// <summary>
    /// Genera reporte de ventas con totales agregados por periodo
    /// Punto 6: KPI de ventas por periodo/categoría
    /// </summary>
    public async Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to)
    {
        // Obtener ventas completadas en el rango
        var sales = await _context.sales
            .Include(s => s.saleproductdetail)
                .ThenInclude(spd => spd.product)
                    .ThenInclude(p => p.category)
            .Where(s => s.SoldAt >= from && s.SoldAt <= to && s.Status == "Completed")
            .ToListAsync();

        // Calcular totales generales
        var report = new SalesReportDto
        {
            FromDate = from,
            ToDate = to,
            TotalSales = sales.Count,
            TotalRevenue = sales.Sum(s => s.Subtotal),
            TotalTax = sales.Sum(s => s.TaxTotal),
            GrandTotal = sales.Sum(s => s.GrandTotal)
        };

        // Agrupar por categoría
        var salesByCategory = sales
            .SelectMany(s => s.saleproductdetail)
            .GroupBy(spd => spd.product.category.Name)
            .Select(g => new SalesByCategoryDto
            {
                CategoryName = g.Key,
                Count = g.Count(),
                Total = g.Sum(spd => spd.LineTotal)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        report.SalesByCategory = salesByCategory;

        return report;
    }
}
