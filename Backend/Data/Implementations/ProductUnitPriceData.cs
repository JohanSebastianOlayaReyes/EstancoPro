using AutoMapper;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementations;

public class ProductUnitPriceData : IProductUnitPriceData
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly DbSet<ProductUnitPrice> _dbSet;

    public ProductUnitPriceData(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _dbSet = _context.Set<ProductUnitPrice>();
    }

    public async Task<ProductUnitPriceDto> GetByIdAsync(int productId, int unitMeasureId)
    {
        var entity = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ProductId == productId && e.UnitMeasureId == unitMeasureId);

        if (entity == null)
        {
            throw new KeyNotFoundException($"No se encontró ProductUnitPrice con ProductId {productId} y UnitMeasureId {unitMeasureId}");
        }

        return _mapper.Map<ProductUnitPriceDto>(entity);
    }

    public async Task<IEnumerable<ProductUnitPriceDto>> GetAllAsync()
    {
        var entities = await _dbSet
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductUnitPriceDto>>(entities);
    }

    public async Task<ProductUnitPriceDto> CreateAsync(ProductUnitPriceDto dto)
    {
        var entity = _mapper.Map<ProductUnitPrice>(dto);

        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductUnitPriceDto>(entity);
    }

    public async Task UpdateAsync(int productId, int unitMeasureId, ProductUnitPriceDto dto)
    {
        var existingEntity = await _dbSet
            .FirstOrDefaultAsync(e => e.ProductId == productId && e.UnitMeasureId == unitMeasureId);

        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"No se encontró ProductUnitPrice con ProductId {productId} y UnitMeasureId {unitMeasureId}");
        }

        _mapper.Map(dto, existingEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int productId, int unitMeasureId)
    {
        var entity = await _dbSet
            .FirstOrDefaultAsync(e => e.ProductId == productId && e.UnitMeasureId == unitMeasureId);

        if (entity == null)
        {
            throw new KeyNotFoundException($"No se encontró ProductUnitPrice con ProductId {productId} y UnitMeasureId {unitMeasureId}");
        }

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
