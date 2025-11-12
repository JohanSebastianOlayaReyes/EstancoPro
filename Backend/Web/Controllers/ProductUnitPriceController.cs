using Business.Interfaces;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para ProductUnitPrice (precios por presentación)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductUnitPriceController : ControllerBase
    {
        private readonly IProductUnitPriceBusiness _productUnitPriceBusiness;

        public ProductUnitPriceController(IProductUnitPriceBusiness productUnitPriceBusiness)
        {
            _productUnitPriceBusiness = productUnitPriceBusiness;
        }

        /// <summary>
        /// Obtiene todos los precios por presentación
        /// GET: api/ProductUnitPrice
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _productUnitPriceBusiness.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener precios", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un precio específico por IDs compuestos
        /// GET: api/ProductUnitPrice/{productId}/{unitMeasureId}
        /// </summary>
        [HttpGet("{productId:int}/{unitMeasureId:int}")]
        public async Task<IActionResult> GetById(int productId, int unitMeasureId)
        {
            try
            {
                var result = await _productUnitPriceBusiness.GetByIdAsync(productId, unitMeasureId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener precio", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo precio por presentación
        /// POST: api/ProductUnitPrice
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductUnitPriceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _productUnitPriceBusiness.CreateAsync(dto);
                return StatusCode(201, result);
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
                return StatusCode(500, new { message = "Error al crear precio", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un precio existente
        /// PUT: api/ProductUnitPrice/{productId}/{unitMeasureId}
        /// </summary>
        [HttpPut("{productId:int}/{unitMeasureId:int}")]
        public async Task<IActionResult> Update(int productId, int unitMeasureId, [FromBody] ProductUnitPriceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _productUnitPriceBusiness.UpdateAsync(productId, unitMeasureId, dto);
                return NoContent();
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
                return StatusCode(500, new { message = "Error al actualizar precio", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un precio por presentación
        /// DELETE: api/ProductUnitPrice/{productId}/{unitMeasureId}
        /// </summary>
        [HttpDelete("{productId:int}/{unitMeasureId:int}")]
        public async Task<IActionResult> Delete(int productId, int unitMeasureId)
        {
            try
            {
                await _productUnitPriceBusiness.DeleteAsync(productId, unitMeasureId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar precio", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los precios (presentaciones) de un producto por nombre
        /// GET: api/ProductUnitPrice/by-product/{productName}
        /// </summary>
        [HttpGet("by-product/{productName}")]
        public async Task<IActionResult> GetByProductName(string productName)
        {
            try
            {
                var result = await _productUnitPriceBusiness.GetByProductNameAsync(productName);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener precios por producto", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene precio específico por nombres (producto + presentación)
        /// GET: api/ProductUnitPrice/by-names?productName=...&unitMeasureName=...
        /// </summary>
        [HttpGet("by-names")]
        public async Task<IActionResult> GetPriceByNames([FromQuery] string productName, [FromQuery] string unitMeasureName)
        {
            try
            {
                var result = await _productUnitPriceBusiness.GetPriceByNamesAsync(productName, unitMeasureName);

                if (result == null)
                    return NotFound(new { message = "No se encontró precio para la combinación especificada" });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener precio por nombres", error = ex.Message });
            }
        }
    }
}
