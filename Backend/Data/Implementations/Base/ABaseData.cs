namespace Data.Implementations.Base;

using AutoMapper;
using Data.Interfaces.Base;
using Entity.Dto;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Clase abstracta para operaciones CRUD con soporte para DTOs y AutoMapper
/// T = Entidad de base de datos
/// D = DTO (Data Transfer Object)
/// </summary>
public abstract class ABaseData<T, D> : IBaseData<T, D>
    where T : Base
    where D : BaseDto
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly IMapper _mapper;

    protected ABaseData(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _mapper = mapper;
    }

    // Métodos abstractos que deben ser implementados por las clases derivadas
    public abstract Task<D> GetByIdAsync(int id);
    public abstract Task<IEnumerable<D>> GetAllAsync(bool includeDeleted = false);
    public abstract Task<D> CreateAsync(D dto);
    public abstract Task UpdateAsync(int id, D dto);
    public abstract Task PatchAsync(int id, D dto);
    public abstract Task DeleteLogicAsync(int id);
    public abstract Task DeletePermanentAsync(int id);
}
