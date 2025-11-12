namespace Business.Interfaces;

using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

/// <summary>
/// Interfaz de negocio para Sale con métodos específicos
/// </summary>
public interface ISaleBusiness : IBaseBusiness<Sale, SaleDto>
{
    /// <summary>
    /// Finaliza una venta: valida stock, recalcula totales, descuenta inventario y registra en caja
    /// </summary>
    /// <param name="saleId">ID de la venta en estado Draft</param>
    Task FinalizeSaleAsync(int saleId);

    /// <summary>
    /// Cancela una venta en borrador (elimina líneas y venta)
    /// </summary>
    /// <param name="saleId">ID de la venta a cancelar</param>
    Task CancelSaleAsync(int saleId);

    /// <summary>
    /// Recalcula los totales de una venta (subtotal, impuestos, total)
    /// </summary>
    /// <param name="saleId">ID de la venta</param>
    Task RecalculateTotalsAsync(int saleId);

    /// <summary>
    /// Obtiene ventas de una sesión de caja específica
    /// </summary>
    /// <param name="cashSessionId">ID de la sesión de caja</param>
    Task<IEnumerable<SaleDto>> GetByCashSessionAsync(int cashSessionId);

    /// <summary>
    /// Obtiene ventas en un rango de fechas
    /// </summary>
    Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime from, DateTime to);

    /// <summary>
    /// Obtiene ventas por estado
    /// </summary>
    /// <param name="status">Estado de la venta (Draft, Completed, Cancelled)</param>
    Task<IEnumerable<SaleDto>> GetByStatusAsync(string status);

    /// <summary>
    /// Genera reporte de ventas con totales agregados por periodo
    /// </summary>
    Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to);
}
