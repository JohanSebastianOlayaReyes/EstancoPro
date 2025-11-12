using AutoMapper;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementations;

public class CashMovementData : ICashMovementData
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly DbSet<CashMovement> _dbSet;

    public CashMovementData(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _dbSet = _context.Set<CashMovement>();
    }

    public async Task<CashMovementDto> GetByIdAsync(int cashSessionId, DateTime at)
    {
        var entity = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.CashSessionId == cashSessionId && e.At == at);

        if (entity == null)
        {
            throw new KeyNotFoundException($"No se encontró CashMovement con CashSessionId {cashSessionId} y At {at}");
        }

        return _mapper.Map<CashMovementDto>(entity);
    }

    public async Task<IEnumerable<CashMovementDto>> GetAllAsync()
    {
        var entities = await _dbSet
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<CashMovementDto>>(entities);
    }

    public async Task<CashMovementDto> CreateAsync(CashMovementDto dto)
    {
        var entity = _mapper.Map<CashMovement>(dto);

        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<CashMovementDto>(entity);
    }

    public async Task UpdateAsync(int cashSessionId, DateTime at, CashMovementDto dto)
    {
        var existingEntity = await _dbSet
            .FirstOrDefaultAsync(e => e.CashSessionId == cashSessionId && e.At == at);

        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"No se encontró CashMovement con CashSessionId {cashSessionId} y At {at}");
        }

        _mapper.Map(dto, existingEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int cashSessionId, DateTime at)
    {
        var entity = await _dbSet
            .FirstOrDefaultAsync(e => e.CashSessionId == cashSessionId && e.At == at);

        if (entity == null)
        {
            throw new KeyNotFoundException($"No se encontró CashMovement con CashSessionId {cashSessionId} y At {at}");
        }

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
