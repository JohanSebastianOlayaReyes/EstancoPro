namespace Business.Interfaces;

using Entity.Dto;

/// <summary>
/// Interfaz de negocio para SaleProductDetail - Solo CRUD básico
/// No hereda de IBaseBusiness porque tiene llave compuesta
/// </summary>
public interface ISaleProductDetailBusiness
{
    Task<IEnumerable<SaleProductDetailDto>> GetAllAsync();
    Task<SaleProductDetailDto> GetByIdAsync(int saleId, int productId, int unitMeasureId);
    Task<SaleProductDetailDto> CreateAsync(SaleProductDetailDto dto);
    Task UpdateAsync(int saleId, int productId, int unitMeasureId, SaleProductDetailDto dto);
    Task DeleteAsync(int saleId, int productId, int unitMeasureId);
}
