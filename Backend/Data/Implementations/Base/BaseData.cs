namespace Data.Implementations.Base;

using AutoMapper;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación base mejorada con AutoMapper, DTOs y mejor manejo de errores
/// T = Entidad de base de datos
/// D = DTO (Data Transfer Object)
/// </summary>
public class BaseData<T, D> : ABaseData<T, D>
    where T : Base
    where D : BaseDto
{
    public BaseData(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    /// <summary>
    /// Obtiene todos los registros activos (no eliminados lógicamente)
    /// </summary>
    public override async Task<IEnumerable<D>> GetAllAsync(bool includeDeleted = false)
    {
        var query = _dbSet.AsQueryable();
        if (!includeDeleted)
        {
            query = query.Where(e => e.DeleteAt == null);
        }

        var entities = await query.AsNoTracking().ToListAsync();

        return _mapper.Map<IEnumerable<D>>(entities);
    }

    /// <summary>
    /// Obtiene una entidad por su ID
    /// </summary>
    public override async Task<D> GetByIdAsync(int id)
    {
        var entity = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.DeleteAt == null);

        if (entity == null)
        {
            throw new KeyNotFoundException($"No se encontró {typeof(T).Name} con Id {id}");
        }

        return _mapper.Map<D>(entity);
    }

    /// <summary>
    /// Crea una nueva entidad a partir de un DTO
    /// </summary>
    public override async Task<D> CreateAsync(D dto)
    {
        var entity = _mapper.Map<T>(dto);

        entity.CreateAt = DateTime.UtcNow;
        entity.UpdateAt = DateTime.UtcNow;
        entity.Active = true;

        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<D>(entity);
    }

    /// <summary>
    /// Actualiza completamente una entidad existente
    /// </summary>
    public override async Task UpdateAsync(int id, D dto)
    {
        var existingEntity = await _dbSet.FindAsync(id);
        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"No se encontró {typeof(T).Name} con Id {id}");
        }

        if (existingEntity.DeleteAt != null)
        {
            throw new InvalidOperationException($"No se puede actualizar {typeof(T).Name} con Id {id} porque está eliminado");
        }

        // Mapear los valores del DTO a la entidad existente
        _mapper.Map(dto, existingEntity);

        // Preservar fechas importantes
        existingEntity.UpdateAt = DateTime.UtcNow;
        existingEntity.Id = id; // Asegurar que el ID no cambie

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza parcialmente una entidad (solo propiedades no nulas del DTO)
    /// </summary>
    public override async Task PatchAsync(int id, D dto)
    {
        var existingEntity = await _dbSet.FindAsync(id);
        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"No se encontró {typeof(T).Name} con Id {id}");
        }

        if (existingEntity.DeleteAt != null)
        {
            throw new InvalidOperationException($"No se puede actualizar {typeof(T).Name} con Id {id} porque está eliminado");
        }

        // Solo actualizar propiedades que no sean nulas en el DTO
        var entry = _context.Entry(existingEntity);
        var dtoType = dto.GetType();

        foreach (var property in dtoType.GetProperties())
        {
            var value = property.GetValue(dto);
            if (value != null && property.Name != "Id" && property.Name != "CreateAt")
            {
                var entityProperty = entry.Property(property.Name);
                if (entityProperty != null)
                {
                    entityProperty.CurrentValue = value;
                }
            }
        }

        existingEntity.UpdateAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Eliminación lógica (soft delete)
    /// </summary>
    public override async Task DeleteLogicAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"No se encontró {typeof(T).Name} con Id {id}");
        }

        entity.DeleteAt = DateTime.UtcNow;
        entity.Active = false;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Eliminación permanente de la base de datos
    /// </summary>
    public override async Task DeletePermanentAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"No se encontró {typeof(T).Name} con Id {id}");
        }

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
