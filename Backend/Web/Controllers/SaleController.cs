using Business.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para Sale con flujo completo de ventas POS
    /// </summary>
    public class SaleController : BaseController<Sale, SaleDto>
    {
        private readonly ISaleBusiness _saleBusiness;

        public SaleController(ISaleBusiness saleBusiness) : base(saleBusiness)
        {
            _saleBusiness = saleBusiness;
        }

        /// <summary>
        /// Finaliza una venta: valida stock, recalcula totales, descuenta inventario y registra en caja
        /// POST: api/Sale/{id}/finalize
        /// FLUJO MÁS CRÍTICO DEL SISTEMA
        /// </summary>
        [HttpPost("{id:int}/finalize")]
        public async Task<IActionResult> FinalizeSale(int id)
        {
            try
            {
                await _saleBusiness.FinalizeSaleAsync(id);
                return Ok(new { message = "Venta finalizada exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al finalizar venta", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancela una venta en borrador
        /// POST: api/Sale/{id}/cancel
        /// </summary>
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> CancelSale(int id)
        {
            try
            {
                await _saleBusiness.CancelSaleAsync(id);
                return Ok(new { message = "Venta cancelada exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cancelar venta", error = ex.Message });
            }
        }

        /// <summary>
        /// Recalcula los totales de una venta (al agregar/editar líneas)
        /// POST: api/Sale/{id}/recalculate-totals
        /// </summary>
        [HttpPost("{id:int}/recalculate-totals")]
        public async Task<IActionResult> RecalculateTotals(int id)
        {
            try
            {
                await _saleBusiness.RecalculateTotalsAsync(id);
                return Ok(new { message = "Totales recalculados exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al recalcular totales", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene ventas de una sesión de caja específica
        /// GET: api/Sale/by-cash-session/{cashSessionId}
        /// </summary>
        [HttpGet("by-cash-session/{cashSessionId:int}")]
        public async Task<IActionResult> GetByCashSession(int cashSessionId)
        {
            try
            {
                var result = await _saleBusiness.GetByCashSessionAsync(cashSessionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener ventas por sesión", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene ventas en un rango de fechas
        /// GET: api/Sale/by-date-range?from=2024-01-01&to=2024-12-31
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                var result = await _saleBusiness.GetByDateRangeAsync(from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener ventas por fecha", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene ventas por estado
        /// GET: api/Sale/by-status?status=Completed
        /// </summary>
        [HttpGet("by-status")]
        public async Task<IActionResult> GetByStatus([FromQuery] string status)
        {
            try
            {
                var result = await _saleBusiness.GetByStatusAsync(status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener ventas por estado", error = ex.Message });
            }
        }

        /// <summary>
        /// Genera reporte de ventas con totales agregados
        /// GET: api/Sale/report?from=2024-01-01&to=2024-12-31
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                var result = await _saleBusiness.GetSalesReportAsync(from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al generar reporte de ventas", error = ex.Message });
            }
        }
    }
}
