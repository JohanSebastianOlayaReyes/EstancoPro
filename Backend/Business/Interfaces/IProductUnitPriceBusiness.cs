namespace Business.Interfaces;

using Entity.Dto;

/// <summary>
/// Interfaz de negocio para ProductUnitPrice con métodos específicos
/// </summary>
public interface IProductUnitPriceBusiness
{
    /// <summary>
    /// Obtiene todos los registros (CRUD básico)
    /// </summary>
    Task<IEnumerable<ProductUnitPriceDto>> GetAllAsync();

    /// <summary>
    /// Obtiene un precio específico por IDs compuestos (CRUD básico)
    /// </summary>
    Task<ProductUnitPriceDto> GetByIdAsync(int productId, int unitMeasureId);

    /// <summary>
    /// Crea un nuevo precio por presentación (CRUD básico)
    /// </summary>
    Task<ProductUnitPriceDto> CreateAsync(ProductUnitPriceDto dto);

    /// <summary>
    /// Actualiza un precio existente (CRUD básico)
    /// </summary>
    Task UpdateAsync(int productId, int unitMeasureId, ProductUnitPriceDto dto);

    /// <summary>
    /// Elimina un precio por presentación (CRUD básico)
    /// </summary>
    Task DeleteAsync(int productId, int unitMeasureId);

    /// <summary>
    /// Obtiene todos los precios (presentaciones) de un producto por nombre
    /// </summary>
    /// <param name="productName">Nombre del producto</param>
    Task<IEnumerable<ProductUnitPriceDto>> GetByProductNameAsync(string productName);

    /// <summary>
    /// Obtiene el precio de una presentación específica usando nombres (para POS)
    /// </summary>
    /// <param name="productName">Nombre del producto</param>
    /// <param name="unitMeasureName">Nombre de la presentación</param>
    /// <returns>Precio de la presentación o null si no existe</returns>
    Task<ProductUnitPriceDto?> GetPriceByNamesAsync(string productName, string unitMeasureName);
}
