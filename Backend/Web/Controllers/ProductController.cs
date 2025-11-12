using Business.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para Product con métodos específicos de inventario
    /// </summary>
    public class ProductController : BaseController<Product, ProductDto>
    {
        private readonly IProductBusiness _productBusiness;

        public ProductController(IProductBusiness productBusiness) : base(productBusiness)
        {
            _productBusiness = productBusiness;
        }

        /// <summary>
        /// Obtiene productos con stock bajo o igual al punto de reorden
        /// GET: api/Product/low-stock
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProducts()
        {
            try
            {
                var result = await _productBusiness.GetLowStockProductsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener productos con stock bajo", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene productos por nombre de categoría
        /// GET: api/Product/by-category/{categoryName}
        /// </summary>
        [HttpGet("by-category/{categoryName}")]
        public async Task<IActionResult> GetByCategoryName(string categoryName)
        {
            try
            {
                var result = await _productBusiness.GetByCategoryNameAsync(categoryName);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener productos por categoría", error = ex.Message });
            }
        }

        /// <summary>
        /// Ajusta el stock de un producto manualmente
        /// POST: api/Product/adjust-stock
        /// Body: { "productName": "...", "quantityChange": 10, "reason": "..." }
        /// </summary>
        [HttpPost("adjust-stock")]
        public async Task<IActionResult> AdjustStock([FromBody] AdjustStockRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _productBusiness.AdjustStockAsync(request.ProductName, request.QuantityChange, request.Reason);
                return Ok(new { message = "Stock ajustado exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al ajustar stock", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el stock disponible en todas las presentaciones de un producto
        /// GET: api/Product/stock-by-presentations/{productName}
        /// </summary>
        [HttpGet("stock-by-presentations/{productName}")]
        public async Task<IActionResult> GetStockByPresentations(string productName)
        {
            try
            {
                var result = await _productBusiness.GetStockByPresentationsAsync(productName);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener stock por presentaciones", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request para ajustar stock
    /// </summary>
    public class AdjustStockRequest
    {
        public string ProductName { get; set; }
        public int QuantityChange { get; set; }
        public string Reason { get; set; }
    }
}
