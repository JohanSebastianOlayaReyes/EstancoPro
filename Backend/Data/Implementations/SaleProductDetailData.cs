using AutoMapper;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementations;

public class SaleProductDetailData : ISaleProductDetailData
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly DbSet<SaleProductDetail> _dbSet;

    public SaleProductDetailData(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _dbSet = _context.Set<SaleProductDetail>();
    }

    public async Task<SaleProductDetailDto> GetByIdAsync(int saleId, int productId, int unitMeasureId)
    {
        var entity = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.SaleId == saleId && e.ProductId == productId && e.UnitMeasureId == unitMeasureId);

        if (entity == null)
        {
            throw new KeyNotFoundException($"No se encontró SaleProductDetail con SaleId {saleId}, ProductId {productId}, UnitMeasureId {unitMeasureId}");
        }

        return _mapper.Map<SaleProductDetailDto>(entity);
    }

    public async Task<IEnumerable<SaleProductDetailDto>> GetAllAsync()
    {
        var entities = await _dbSet
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<SaleProductDetailDto>>(entities);
    }

    public async Task<SaleProductDetailDto> CreateAsync(SaleProductDetailDto dto)
    {
        var entity = _mapper.Map<SaleProductDetail>(dto);

        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<SaleProductDetailDto>(entity);
    }

    public async Task UpdateAsync(int saleId, int productId, int unitMeasureId, SaleProductDetailDto dto)
    {
        var existingEntity = await _dbSet
            .FirstOrDefaultAsync(e => e.SaleId == saleId && e.ProductId == productId && e.UnitMeasureId == unitMeasureId);

        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"No se encontró SaleProductDetail con SaleId {saleId}, ProductId {productId}, UnitMeasureId {unitMeasureId}");
        }

        _mapper.Map(dto, existingEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int saleId, int productId, int unitMeasureId)
    {
        var entity = await _dbSet
            .FirstOrDefaultAsync(e => e.SaleId == saleId && e.ProductId == productId && e.UnitMeasureId == unitMeasureId);

        if (entity == null)
        {
            throw new KeyNotFoundException($"No se encontró SaleProductDetail con SaleId {saleId}, ProductId {productId}, UnitMeasureId {unitMeasureId}");
        }

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
