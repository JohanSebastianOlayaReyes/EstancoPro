namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para Product con métodos específicos de inventario
/// </summary>
public class ProductBusiness : BaseBusiness<Product, ProductDto>, IProductBusiness
{
    private readonly IProductData _productData;
    private readonly ApplicationDbContext _context;

    public ProductBusiness(
        IProductData productData,
        ApplicationDbContext context,
        ILogger<BaseBusiness<Product, ProductDto>> logger)
        : base(productData, logger)
    {
        _productData = productData;
        _context = context;
    }

    /// <summary>
    /// Obtiene productos con stock bajo o igual al punto de reorden
    /// Regla de negocio: StockOnHand <= ReorderPoint (punto 6 KPIs)
    /// </summary>
    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
    {
        // Consulta productos donde el stock actual es menor o igual al punto de reorden
        // Excluye productos eliminados lógicamente
        var products = await _context.products
            .Where(p => p.StockOnHand <= p.ReorderPoint && p.DeleteAt == null)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                UnitCost = p.UnitCost,
                UnitPrice = p.UnitPrice,
                TaxRate = p.TaxRate,
                StockOnHand = p.StockOnHand,
                ReorderPoint = p.ReorderPoint,
                CategoryId = p.CategoryId,
                UnitMeasureId = p.UnitMeasureId,
                Active = p.Active,
                CreateAt = p.CreateAt,
                UpdateAt = p.UpdateAt,
                DeleteAt = p.DeleteAt
            })
            .ToListAsync();

        return products;
    }

    /// <summary>
    /// Obtiene productos filtrados por nombre de categoría
    /// Frontend trabaja con nombres, no IDs (regla UX punto 7)
    /// </summary>
    public async Task<IEnumerable<ProductDto>> GetByCategoryNameAsync(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("El nombre de la categoría no puede estar vacío");
        }

        // JOIN con Category para filtrar por nombre de categoría
        // Excluye productos eliminados
        var products = await _context.products
            .Include(p => p.category)
            .Where(p => p.category.Name == categoryName && p.DeleteAt == null)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                UnitCost = p.UnitCost,
                UnitPrice = p.UnitPrice,
                TaxRate = p.TaxRate,
                StockOnHand = p.StockOnHand,
                ReorderPoint = p.ReorderPoint,
                CategoryId = p.CategoryId,
                UnitMeasureId = p.UnitMeasureId,
                Active = p.Active,
                CreateAt = p.CreateAt,
                UpdateAt = p.UpdateAt,
                DeleteAt = p.DeleteAt
            })
            .ToListAsync();

        return products;
    }

    /// <summary>
    /// Ajusta el stock manualmente (inventario físico, mermas, correcciones)
    /// Regla de negocio: No permitir stock negativo
    /// </summary>
    public async Task AdjustStockAsync(string productName, int quantityChange, string reason)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacío");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Debe especificar la razón del ajuste");
        }

        // Buscar producto por nombre
        var product = await _context.products
            .FirstOrDefaultAsync(p => p.Name == productName && p.DeleteAt == null);

        if (product == null)
        {
            throw new KeyNotFoundException($"No se encontró el producto '{productName}'");
        }

        // Calcular nuevo stock
        var newStock = product.StockOnHand + quantityChange;

        // Validar que el stock no quede negativo
        if (newStock < 0)
        {
            throw new InvalidOperationException(
                $"El ajuste resultaría en stock negativo. Stock actual: {product.StockOnHand}, " +
                $"Ajuste: {quantityChange}, Resultado: {newStock}");
        }

        // Aplicar ajuste
        product.StockOnHand = newStock;
        product.UpdateAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // TODO: Registrar el ajuste en una tabla de auditoría de inventario
        // con: ProductId, QuantityChange, Reason, DateTime, User
    }

    /// <summary>
    /// Obtiene el stock disponible convertido a todas las presentaciones del producto
    /// Regla de negocio: Conversión usando ConversionFactor (punto 2.1)
    /// Ejemplo: Stock=200 unidades → Unidad:200, Paquete:33.33, Caja:8.33
    /// </summary>
    public async Task<Dictionary<string, decimal>> GetStockByPresentationsAsync(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacío");
        }

        // Buscar producto con su unidad base
        var product = await _context.products
            .Include(p => p.unitmeasure)
            .FirstOrDefaultAsync(p => p.Name == productName && p.DeleteAt == null);

        if (product == null)
        {
            throw new KeyNotFoundException($"No se encontró el producto '{productName}'");
        }

        var result = new Dictionary<string, decimal>();

        // Obtener todas las presentaciones (ProductUnitPrice) del producto
        var presentations = await _context.productUnitPrices
            .Include(pup => pup.unitmeasure)
            .Where(pup => pup.ProductId == product.Id)
            .ToListAsync();

        // Si no hay presentaciones definidas, solo mostrar en unidad base
        if (!presentations.Any())
        {
            result[product.unitmeasure.Name] = product.StockOnHand;
            return result;
        }

        // Convertir stock a cada presentación
        // Regla: stock en presentación = StockOnHand / ConversionFactor
        foreach (var presentation in presentations)
        {
            var stockInPresentation = presentation.ConversionFactor > 0
                ? (decimal)product.StockOnHand / presentation.ConversionFactor
                : 0;

            result[presentation.unitmeasure.Name] = Math.Round(stockInPresentation, 2);
        }

        return result;
    }
}
