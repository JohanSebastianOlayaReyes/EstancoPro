namespace Business.Implementations;

using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para CashMovement - Solo CRUD básico
/// Se gestiona en contexto de CashSession
/// Los cálculos de balance se hacen en CashSessionBusiness
/// </summary>
public class CashMovementBusiness : ICashMovementBusiness
{
    private readonly ICashMovementData _cashMovementData;
    private readonly ILogger<CashMovementBusiness> _logger;

    public CashMovementBusiness(
        ICashMovementData cashMovementData,
        ILogger<CashMovementBusiness> logger)
    {
        _cashMovementData = cashMovementData;
        _logger = logger;
    }

    public async Task<IEnumerable<CashMovementDto>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los movimientos de caja");
            return await _cashMovementData.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos de caja");
            throw;
        }
    }

    public async Task<CashMovementDto> GetByIdAsync(int cashSessionId, DateTime at)
    {
        try
        {
            _logger.LogInformation("Obteniendo movimiento de caja: SessionId={SessionId}, At={At}",
                cashSessionId, at);
            return await _cashMovementData.GetByIdAsync(cashSessionId, at);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimiento de caja");
            throw;
        }
    }

    public async Task<CashMovementDto> CreateAsync(CashMovementDto dto)
    {
        try
        {
            _logger.LogInformation("Creando movimiento de caja");
            return await _cashMovementData.CreateAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear movimiento de caja");
            throw;
        }
    }

    public async Task UpdateAsync(int cashSessionId, DateTime at, CashMovementDto dto)
    {
        try
        {
            _logger.LogInformation("Actualizando movimiento de caja");
            await _cashMovementData.UpdateAsync(cashSessionId, at, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar movimiento de caja");
            throw;
        }
    }

    public async Task DeleteAsync(int cashSessionId, DateTime at)
    {
        try
        {
            _logger.LogInformation("Eliminando movimiento de caja");
            await _cashMovementData.DeleteAsync(cashSessionId, at);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar movimiento de caja");
            throw;
        }
    }
}
