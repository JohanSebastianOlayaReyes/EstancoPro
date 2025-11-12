namespace Business.Implementations.Base;

using Business.Interfaces.Base;
using Data.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

/// <summary>
/// Clase abstracta que define la estructura básica de la capa de negocio con soporte para DTOs.
/// Permite extender comportamiento genérico sin modificar las implementaciones concretas.
/// T = Entidad de base de datos
/// D = DTO (Data Transfer Object)
/// </summary>
public abstract class ABaseBusiness<T, D> : IBaseBusiness<T, D>
    where T : Base
    where D : BaseDto
{
    protected readonly IBaseData<T, D> _data;

    protected ABaseBusiness(IBaseData<T, D> data)
    {
        _data = data;
    }

    /// <summary>Obtiene todos los registros de la entidad.</summary>
    public abstract Task<IEnumerable<D>> GetAllAsync(bool includeDeleted = false);

    /// <summary>Obtiene una entidad por su Id.</summary>
    public abstract Task<D> GetByIdAsync(int id);

    /// <summary>Crea una nueva entidad.</summary>
    public abstract Task<D> CreateAsync(D dto);

    /// <summary>Actualiza una entidad existente.</summary>
    public abstract Task UpdateAsync(int id, D dto);

    /// <summary>Actualiza parcialmente una entidad existente.</summary>
    public abstract Task PatchAsync(int id, D dto);

    /// <summary>Elimina lógicamente una entidad (soft delete).</summary>
    public abstract Task DeleteLogicAsync(int id);

    /// <summary>Elimina permanentemente una entidad.</summary>
    public abstract Task DeletePermanentAsync(int id);
}
