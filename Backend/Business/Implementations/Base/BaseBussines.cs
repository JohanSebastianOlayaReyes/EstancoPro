namespace Business.Implementations.Base;

using Data.Interfaces.Base;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación base de la capa de negocio con manejo de errores, logging y validación.
/// T = Entidad de base de datos
/// D = DTO (Data Transfer Object)
/// </summary>
public class BaseBusiness<T, D> : ABaseBusiness<T, D>
    where T : Base
    where D : BaseDto
{
    private readonly ILogger<BaseBusiness<T, D>> _logger;

    public BaseBusiness(IBaseData<T, D> data, ILogger<BaseBusiness<T, D>> logger)
        : base(data)
    {
        _logger = logger;
    }

    public override async Task<IEnumerable<D>> GetAllAsync(bool includeDeleted = false)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los registros de {Entity}", typeof(T).Name);
            return await _data.GetAllAsync(includeDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los registros de {Entity}", typeof(T).Name);
            throw;
        }
    }

    public override async Task<D> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo {Entity} con Id {Id}", typeof(T).Name, id);
            return await _data.GetByIdAsync(id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "No se encontró {Entity} con Id {Id}", typeof(T).Name, id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener {Entity} con Id {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public override async Task<D> CreateAsync(D dto)
    {
        try
        {
            _logger.LogInformation("Creando nuevo {Entity}", typeof(T).Name);
            return await _data.CreateAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear {Entity}", typeof(T).Name);
            throw;
        }
    }

    public override async Task UpdateAsync(int id, D dto)
    {
        try
        {
            _logger.LogInformation("Actualizando {Entity} con Id {Id}", typeof(T).Name, id);
            await _data.UpdateAsync(id, dto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "No se encontró {Entity} con Id {Id} para actualizar", typeof(T).Name, id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar {Entity} con Id {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public override async Task PatchAsync(int id, D dto)
    {
        try
        {
            _logger.LogInformation("Actualizando parcialmente {Entity} con Id {Id}", typeof(T).Name, id);
            await _data.PatchAsync(id, dto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "No se encontró {Entity} con Id {Id} para actualizar", typeof(T).Name, id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar parcialmente {Entity} con Id {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public override async Task DeleteLogicAsync(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando lógicamente {Entity} con Id {Id}", typeof(T).Name, id);
            await _data.DeleteLogicAsync(id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "No se encontró {Entity} con Id {Id} para eliminar", typeof(T).Name, id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar lógicamente {Entity} con Id {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public override async Task DeletePermanentAsync(int id)
    {
        try
        {
            _logger.LogWarning("Eliminando permanentemente {Entity} con Id {Id}", typeof(T).Name, id);
            await _data.DeletePermanentAsync(id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "No se encontró {Entity} con Id {Id} para eliminar", typeof(T).Name, id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permanentemente {Entity} con Id {Id}", typeof(T).Name, id);
            throw;
        }
    }
}
