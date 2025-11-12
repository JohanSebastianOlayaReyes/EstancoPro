using Business.Interfaces;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para SaleProductDetail (líneas de venta)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SaleProductDetailController : ControllerBase
    {
        private readonly ISaleProductDetailBusiness _saleProductDetailBusiness;

        public SaleProductDetailController(ISaleProductDetailBusiness saleProductDetailBusiness)
        {
            _saleProductDetailBusiness = saleProductDetailBusiness;
        }

        /// <summary>
        /// Obtiene todos los detalles de venta
        /// GET: api/SaleProductDetail
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _saleProductDetailBusiness.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener detalles de venta", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un detalle de venta por IDs compuestos
        /// GET: api/SaleProductDetail/{saleId}/{productId}/{unitMeasureId}
        /// </summary>
        [HttpGet("{saleId:int}/{productId:int}/{unitMeasureId:int}")]
        public async Task<IActionResult> GetById(int saleId, int productId, int unitMeasureId)
        {
            try
            {
                var result = await _saleProductDetailBusiness.GetByIdAsync(saleId, productId, unitMeasureId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener detalle de venta", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo detalle de venta
        /// POST: api/SaleProductDetail
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaleProductDetailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _saleProductDetailBusiness.CreateAsync(dto);
                return StatusCode(201, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear detalle de venta", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un detalle de venta existente
        /// PUT: api/SaleProductDetail/{saleId}/{productId}/{unitMeasureId}
        /// </summary>
        [HttpPut("{saleId:int}/{productId:int}/{unitMeasureId:int}")]
        public async Task<IActionResult> Update(int saleId, int productId, int unitMeasureId, [FromBody] SaleProductDetailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _saleProductDetailBusiness.UpdateAsync(saleId, productId, unitMeasureId, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar detalle de venta", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un detalle de venta
        /// DELETE: api/SaleProductDetail/{saleId}/{productId}/{unitMeasureId}
        /// </summary>
        [HttpDelete("{saleId:int}/{productId:int}/{unitMeasureId:int}")]
        public async Task<IActionResult> Delete(int saleId, int productId, int unitMeasureId)
        {
            try
            {
                await _saleProductDetailBusiness.DeleteAsync(saleId, productId, unitMeasureId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar detalle de venta", error = ex.Message });
            }
        }
    }
}
