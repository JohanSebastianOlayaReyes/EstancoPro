namespace Business.Interfaces.Base;

using Entity.Dto;
using Entity.Model;

/// <summary>
/// Interfaz base para la capa de negocio con soporte para DTOs
/// T = Entidad de base de datos
/// D = DTO (Data Transfer Object)
/// </summary>
public interface IBaseBusiness<T, D>
    where T : Base
    where D : BaseDto
{
    Task<IEnumerable<D>> GetAllAsync(bool includeDeleted = false);
    Task<D> GetByIdAsync(int id);
    Task<D> CreateAsync(D dto);
    Task UpdateAsync(int id, D dto);
    Task PatchAsync(int id, D dto);
    Task DeleteLogicAsync(int id);
    Task DeletePermanentAsync(int id);
}
