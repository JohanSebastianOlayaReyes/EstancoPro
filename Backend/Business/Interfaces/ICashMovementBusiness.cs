namespace Business.Interfaces;

using Entity.Dto;

/// <summary>
/// Interfaz de negocio para CashMovement - Solo CRUD básico
/// No hereda de IBaseBusiness porque tiene llave compuesta
/// </summary>
public interface ICashMovementBusiness
{
    Task<IEnumerable<CashMovementDto>> GetAllAsync();
    Task<CashMovementDto> GetByIdAsync(int cashSessionId, DateTime at);
    Task<CashMovementDto> CreateAsync(CashMovementDto dto);
    Task UpdateAsync(int cashSessionId, DateTime at, CashMovementDto dto);
    Task DeleteAsync(int cashSessionId, DateTime at);
}
