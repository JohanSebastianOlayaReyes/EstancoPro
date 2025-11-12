namespace Data.Interfaces;

using Entity.Dto;
using Entity.Model;

public interface ISaleProductDetailData
{
    Task<SaleProductDetailDto> GetByIdAsync(int saleId, int productId, int unitMeasureId);
    Task<IEnumerable<SaleProductDetailDto>> GetAllAsync();
    Task<SaleProductDetailDto> CreateAsync(SaleProductDetailDto dto);
    Task UpdateAsync(int saleId, int productId, int unitMeasureId, SaleProductDetailDto dto);
    Task DeleteAsync(int saleId, int productId, int unitMeasureId);
}
