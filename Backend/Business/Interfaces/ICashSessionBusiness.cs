namespace Business.Interfaces;

using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

/// <summary>
/// Interfaz de negocio para CashSession con métodos específicos
/// </summary>
public interface ICashSessionBusiness : IBaseBusiness<CashSession, CashSessionDto>
{
    /// <summary>
    /// Abre una nueva sesión de caja (valida que no haya otra abierta)
    /// </summary>
    /// <param name="openingAmount">Monto inicial de apertura</param>
    Task<CashSessionDto> OpenSessionAsync(decimal openingAmount);

    /// <summary>
    /// Cierra una sesión de caja y calcula la diferencia entre esperado y real
    /// </summary>
    /// <param name="sessionId">ID de la sesión a cerrar</param>
    /// <param name="closingAmount">Monto contado físicamente</param>
    /// <returns>Diferencia (closingAmount - expected)</returns>
    Task<decimal> CloseSessionAsync(int sessionId, decimal closingAmount);

    /// <summary>
    /// Obtiene la sesión de caja actualmente abierta (si existe)
    /// </summary>
    /// <returns>Sesión abierta o null si no hay ninguna</returns>
    Task<CashSessionDto?> GetOpenSessionAsync();

    /// <summary>
    /// Obtiene sesiones de caja en un rango de fechas
    /// </summary>
    Task<IEnumerable<CashSessionDto>> GetByDateRangeAsync(DateTime from, DateTime to);

    /// <summary>
    /// Calcula el balance de una sesión (esperado, real, diferencia)
    /// </summary>
    /// <param name="sessionId">ID de la sesión</param>
    Task<CashSessionBalanceDto> GetSessionBalanceAsync(int sessionId);
}
