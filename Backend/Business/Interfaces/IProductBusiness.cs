namespace Business.Interfaces;

using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

/// <summary>
/// Interfaz de negocio para Product con métodos específicos
/// </summary>
public interface IProductBusiness : IBaseBusiness<Product, ProductDto>
{
    /// <summary>
    /// Obtiene productos con stock bajo o igual al punto de reorden (para alertas de reposición)
    /// </summary>
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();

    /// <summary>
    /// Obtiene productos filtrados por nombre de categoría
    /// </summary>
    /// <param name="categoryName">Nombre de la categoría a filtrar</param>
    Task<IEnumerable<ProductDto>> GetByCategoryNameAsync(string categoryName);

    /// <summary>
    /// Ajusta el stock manualmente (inventario físico, mermas, correcciones)
    /// </summary>
    /// <param name="productName">Nombre del producto a ajustar</param>
    /// <param name="quantityChange">Cantidad a ajustar (positivo = entrada, negativo = salida)</param>
    /// <param name="reason">Razón del ajuste</param>
    Task AdjustStockAsync(string productName, int quantityChange, string reason);

    /// <summary>
    /// Obtiene el stock disponible convertido a todas las presentaciones del producto
    /// </summary>
    /// <param name="productName">Nombre del producto</param>
    /// <returns>Diccionario con nombre de presentación y cantidad disponible</returns>
    Task<Dictionary<string, decimal>> GetStockByPresentationsAsync(string productName);
}
