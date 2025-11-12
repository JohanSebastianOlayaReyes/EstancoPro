namespace Business.Implementations;

using Business.Interfaces;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para ProductUnitPrice (precios por presentación)
/// </summary>
public class ProductUnitPriceBusiness : IProductUnitPriceBusiness
{
    private readonly IProductUnitPriceData _productUnitPriceData;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductUnitPriceBusiness> _logger;

    public ProductUnitPriceBusiness(
        IProductUnitPriceData productUnitPriceData,
        ApplicationDbContext context,
        ILogger<ProductUnitPriceBusiness> logger)
    {
        _productUnitPriceData = productUnitPriceData;
        _context = context;
        _logger = logger;
    }

    #region CRUD Básico

    /// <summary>
    /// Obtiene todos los precios por presentación
    /// </summary>
    public async Task<IEnumerable<ProductUnitPriceDto>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los precios por presentación");
            return await _productUnitPriceData.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los precios por presentación");
            throw;
        }
    }

    /// <summary>
    /// Obtiene un precio específico por ProductId y UnitMeasureId
    /// </summary>
    public async Task<ProductUnitPriceDto> GetByIdAsync(int productId, int unitMeasureId)
    {
        try
        {
            _logger.LogInformation("Obteniendo precio: ProductId={ProductId}, UnitMeasureId={UnitMeasureId}",
                productId, unitMeasureId);
            return await _productUnitPriceData.GetByIdAsync(productId, unitMeasureId);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "No se encontró el precio especificado");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener precio por ID");
            throw;
        }
    }

    /// <summary>
    /// Crea un nuevo precio por presentación
    /// Regla: Validar que el producto y la unidad de medida existan
    /// </summary>
    public async Task<ProductUnitPriceDto> CreateAsync(ProductUnitPriceDto dto)
    {
        try
        {
            _logger.LogInformation("Creando precio por presentación: ProductId={ProductId}, UnitMeasureId={UnitMeasureId}",
                dto.ProductId, dto.UnitMeasureId);

            // Validar que el producto existe
            var productExists = await _context.products.AnyAsync(p => p.Id == dto.ProductId && p.DeleteAt == null);
            if (!productExists)
            {
                throw new KeyNotFoundException($"No se encontró el producto con Id {dto.ProductId}");
            }

            // Validar que la unidad de medida existe
            var unitMeasureExists = await _context.unitMeasures.AnyAsync(u => u.Id == dto.UnitMeasureId && u.DeleteAt == null);
            if (!unitMeasureExists)
            {
                throw new KeyNotFoundException($"No se encontró la unidad de medida con Id {dto.UnitMeasureId}");
            }

            // Validar que el precio es positivo
            if (dto.UnitPrice <= 0)
            {
                throw new ArgumentException("El precio debe ser mayor a cero");
            }

            return await _productUnitPriceData.CreateAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear precio por presentación");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un precio existente
    /// </summary>
    public async Task UpdateAsync(int productId, int unitMeasureId, ProductUnitPriceDto dto)
    {
        try
        {
            _logger.LogInformation("Actualizando precio: ProductId={ProductId}, UnitMeasureId={UnitMeasureId}",
                productId, unitMeasureId);

            // Validar que el precio es positivo
            if (dto.UnitPrice <= 0)
            {
                throw new ArgumentException("El precio debe ser mayor a cero");
            }

            await _productUnitPriceData.UpdateAsync(productId, unitMeasureId, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar precio por presentación");
            throw;
        }
    }

    /// <summary>
    /// Elimina un precio por presentación
    /// </summary>
    public async Task DeleteAsync(int productId, int unitMeasureId)
    {
        try
        {
            _logger.LogInformation("Eliminando precio: ProductId={ProductId}, UnitMeasureId={UnitMeasureId}",
                productId, unitMeasureId);
            await _productUnitPriceData.DeleteAsync(productId, unitMeasureId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar precio por presentación");
            throw;
        }
    }

    #endregion

    #region Métodos Específicos

    /// <summary>
    /// Obtiene todos los precios (presentaciones) de un producto por nombre
    /// Regla UX punto 7: Usuario selecciona producto y ve todas las presentaciones disponibles
    /// </summary>
    public async Task<IEnumerable<ProductUnitPriceDto>> GetByProductNameAsync(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacío");
        }

        try
        {
            _logger.LogInformation("Obteniendo precios por nombre de producto: {ProductName}", productName);

            // JOIN con Product para filtrar por nombre
            var prices = await _context.productUnitPrices
                .Include(pup => pup.product)
                .Include(pup => pup.unitmeasure)
                .Where(pup => pup.product.Name == productName && pup.product.DeleteAt == null)
                .Select(pup => new ProductUnitPriceDto
                {
                    ProductId = pup.ProductId,
                    UnitMeasureId = pup.UnitMeasureId,
                    UnitPrice = pup.UnitPrice,
                    UnitCost = pup.UnitCost,
                    Barcode = pup.Barcode
                })
                .ToListAsync();

            return prices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener precios por nombre de producto");
            throw;
        }
    }

    /// <summary>
    /// Obtiene el precio de una presentación específica usando nombres
    /// Regla UX punto 7: Precargar precio/costo al seleccionar producto + presentación en POS
    /// Regla punto 2.2: Si existe ProductUnitPrice, usar ese precio; si no, derivar del base
    /// </summary>
    public async Task<ProductUnitPriceDto?> GetPriceByNamesAsync(string productName, string unitMeasureName)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacío");
        }

        if (string.IsNullOrWhiteSpace(unitMeasureName))
        {
            throw new ArgumentException("El nombre de la unidad de medida no puede estar vacío");
        }

        try
        {
            _logger.LogInformation("Obteniendo precio por nombres: Producto={ProductName}, Presentación={UnitMeasureName}",
                productName, unitMeasureName);

            // JOIN con Product y UnitMeasure para buscar por nombres
            var price = await _context.productUnitPrices
                .Include(pup => pup.product)
                .Include(pup => pup.unitmeasure)
                .Where(pup => pup.product.Name == productName &&
                             pup.unitmeasure.Name == unitMeasureName &&
                             pup.product.DeleteAt == null)
                .Select(pup => new ProductUnitPriceDto
                {
                    ProductId = pup.ProductId,
                    UnitMeasureId = pup.UnitMeasureId,
                    UnitPrice = pup.UnitPrice,
                    UnitCost = pup.UnitCost,
                    Barcode = pup.Barcode
                })
                .FirstOrDefaultAsync();

            // Si no existe precio específico, calcular desde precio base
            // Regla punto 2.2: precio = UnitPriceBase × ConversionFactor
            if (price == null)
            {
                var product = await _context.products
                    .FirstOrDefaultAsync(p => p.Name == productName && p.DeleteAt == null);

                var unitMeasure = await _context.unitMeasures
                    .FirstOrDefaultAsync(u => u.Name == unitMeasureName && u.DeleteAt == null);

                if (product != null && unitMeasure != null)
                {
                    // Buscar el ProductUnitPrice para obtener el ConversionFactor
                    var productUnitPrice = await _context.productUnitPrices
                        .FirstOrDefaultAsync(pup => pup.ProductId == product.Id && pup.UnitMeasureId == unitMeasure.Id);

                    if (productUnitPrice != null)
                    {
                        _logger.LogInformation("Precio encontrado en ProductUnitPrice: {Price}", productUnitPrice.UnitPrice);

                        return new ProductUnitPriceDto
                        {
                            ProductId = product.Id,
                            UnitMeasureId = unitMeasure.Id,
                            UnitPrice = productUnitPrice.UnitPrice,
                            UnitCost = productUnitPrice.UnitCost,
                            ConversionFactor = productUnitPrice.ConversionFactor,
                            Barcode = productUnitPrice.Barcode
                        };
                    }

                    // Si no existe ProductUnitPrice, usar precio base (asumiendo ConversionFactor = 1)
                    _logger.LogInformation("Precio no encontrado, usando precio base del producto");

                    return new ProductUnitPriceDto
                    {
                        ProductId = product.Id,
                        UnitMeasureId = unitMeasure.Id,
                        UnitPrice = product.UnitPrice,
                        UnitCost = product.UnitCost,
                        ConversionFactor = 1,
                        Barcode = null
                    };
                }
            }

            return price;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener precio por nombres");
            throw;
        }
    }

    #endregion
}
