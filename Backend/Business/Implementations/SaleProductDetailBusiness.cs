namespace Business.Implementations;

using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para SaleProductDetail - Solo CRUD básico
/// Se gestiona en contexto de Sale (carrito)
/// La validación de stock y totales se hace en SaleBusiness.FinalizeSaleAsync
/// </summary>
public class SaleProductDetailBusiness : ISaleProductDetailBusiness
{
    private readonly ISaleProductDetailData _saleProductDetailData;
    private readonly ILogger<SaleProductDetailBusiness> _logger;

    public SaleProductDetailBusiness(
        ISaleProductDetailData saleProductDetailData,
        ILogger<SaleProductDetailBusiness> logger)
    {
        _saleProductDetailData = saleProductDetailData;
        _logger = logger;
    }

    public async Task<IEnumerable<SaleProductDetailDto>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los detalles de venta");
            return await _saleProductDetailData.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalles de venta");
            throw;
        }
    }

    public async Task<SaleProductDetailDto> GetByIdAsync(int saleId, int productId, int unitMeasureId)
    {
        try
        {
            _logger.LogInformation("Obteniendo detalle de venta: SaleId={SaleId}, ProductId={ProductId}, UnitMeasureId={UnitMeasureId}",
                saleId, productId, unitMeasureId);
            return await _saleProductDetailData.GetByIdAsync(saleId, productId, unitMeasureId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle de venta");
            throw;
        }
    }

    public async Task<SaleProductDetailDto> CreateAsync(SaleProductDetailDto dto)
    {
        try
        {
            _logger.LogInformation("Creando detalle de venta");
            return await _saleProductDetailData.CreateAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear detalle de venta");
            throw;
        }
    }

    public async Task UpdateAsync(int saleId, int productId, int unitMeasureId, SaleProductDetailDto dto)
    {
        try
        {
            _logger.LogInformation("Actualizando detalle de venta");
            await _saleProductDetailData.UpdateAsync(saleId, productId, unitMeasureId, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar detalle de venta");
            throw;
        }
    }

    public async Task DeleteAsync(int saleId, int productId, int unitMeasureId)
    {
        try
        {
            _logger.LogInformation("Eliminando detalle de venta");
            await _saleProductDetailData.DeleteAsync(saleId, productId, unitMeasureId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar detalle de venta");
            throw;
        }
    }
}
