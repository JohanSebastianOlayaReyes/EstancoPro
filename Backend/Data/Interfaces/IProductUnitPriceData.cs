namespace Data.Interfaces;

using Entity.Dto;
using Entity.Model;

public interface IProductUnitPriceData
{
    Task<ProductUnitPriceDto> GetByIdAsync(int productId, int unitMeasureId);
    Task<IEnumerable<ProductUnitPriceDto>> GetAllAsync();
    Task<ProductUnitPriceDto> CreateAsync(ProductUnitPriceDto dto);
    Task UpdateAsync(int productId, int unitMeasureId, ProductUnitPriceDto dto);
    Task DeleteAsync(int productId, int unitMeasureId);
}
