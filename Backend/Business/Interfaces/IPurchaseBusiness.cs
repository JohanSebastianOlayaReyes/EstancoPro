namespace Business.Interfaces;

using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

/// <summary>
/// Interfaz de negocio para Purchase con métodos específicos
/// </summary>
public interface IPurchaseBusiness : IBaseBusiness<Purchase, PurchaseDto>
{
    /// <summary>
    /// Recibe una compra: actualiza stock y opcionalmente registra pago en caja
    /// </summary>
    /// <param name="purchaseId">ID de la compra a recibir</param>
    /// <param name="payInCash">Si se paga en efectivo inmediatamente</param>
    /// <param name="cashSessionId">ID de la sesión de caja (requerido si payInCash = true)</param>
    Task ReceivePurchaseAsync(int purchaseId, bool payInCash = false, int? cashSessionId = null);

    /// <summary>
    /// Cancela una compra en estado Ordered
    /// </summary>
    /// <param name="purchaseId">ID de la compra a cancelar</param>
    /// <param name="reason">Razón de la cancelación</param>
    Task CancelPurchaseAsync(int purchaseId, string reason);

    /// <summary>
    /// Obtiene compras filtradas por nombre de proveedor
    /// </summary>
    /// <param name="supplierName">Nombre del proveedor</param>
    Task<IEnumerable<PurchaseDto>> GetBySupplierNameAsync(string supplierName);

    /// <summary>
    /// Obtiene compras en un rango de fechas
    /// </summary>
    Task<IEnumerable<PurchaseDto>> GetByDateRangeAsync(DateTime from, DateTime to);

    /// <summary>
    /// Obtiene compras por estado
    /// </summary>
    /// <param name="status">true = recibida, false = ordenada/cancelada</param>
    Task<IEnumerable<PurchaseDto>> GetByStatusAsync(bool status);
}
