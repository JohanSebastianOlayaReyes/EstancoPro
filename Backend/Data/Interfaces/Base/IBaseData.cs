namespace Data.Interfaces.Base;

using Entity.Dto;
using Entity.Model;

/// <summary>
/// Interfaz base para operaciones CRUD genéricas con soporte para DTOs
/// T = Entidad de base de datos
/// D = DTO (Data Transfer Object)
/// </summary>
public interface IBaseData<T, D>
    where T : Base
    where D : BaseDto
{
    Task<D> GetByIdAsync(int id);
    Task<IEnumerable<D>> GetAllAsync(bool includeDeleted = false);
    Task<D> CreateAsync(D dto);
    Task UpdateAsync(int id, D dto);
    Task PatchAsync(int id, D dto);
    Task DeleteLogicAsync(int id);
    Task DeletePermanentAsync(int id);
}