using Business.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para Purchase con flujo de entrada de inventario
    /// </summary>
    public class PurchaseController : BaseController<Purchase, PurchaseDto>
    {
        private readonly IPurchaseBusiness _purchaseBusiness;

        public PurchaseController(IPurchaseBusiness purchaseBusiness) : base(purchaseBusiness)
        {
            _purchaseBusiness = purchaseBusiness;
        }

        /// <summary>
        /// Recibe una compra: actualiza stock y opcionalmente registra pago en caja
        /// POST: api/Purchase/{id}/receive
        /// Body: { "payInCash": true/false, "cashSessionId": 1 }
        /// </summary>
        [HttpPost("{id:int}/receive")]
        public async Task<IActionResult> ReceivePurchase(int id, [FromBody] ReceivePurchaseRequest request)
        {
            try
            {
                await _purchaseBusiness.ReceivePurchaseAsync(id, request.PayInCash, request.CashSessionId);
                return Ok(new { message = "Compra recibida exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al recibir compra", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancela una compra en estado Ordered
        /// POST: api/Purchase/{id}/cancel
        /// Body: { "reason": "..." }
        /// </summary>
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> CancelPurchase(int id, [FromBody] CancelPurchaseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "Debe especificar la razón de la cancelación" });

            try
            {
                await _purchaseBusiness.CancelPurchaseAsync(id, request.Reason);
                return Ok(new { message = "Compra cancelada exitosamente" });
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
                return StatusCode(500, new { message = "Error al cancelar compra", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene compras por nombre de proveedor
        /// GET: api/Purchase/by-supplier/{supplierName}
        /// </summary>
        [HttpGet("by-supplier/{supplierName}")]
        public async Task<IActionResult> GetBySupplierName(string supplierName)
        {
            try
            {
                var result = await _purchaseBusiness.GetBySupplierNameAsync(supplierName);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener compras por proveedor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene compras en un rango de fechas
        /// GET: api/Purchase/by-date-range?from=2024-01-01&to=2024-12-31
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                var result = await _purchaseBusiness.GetByDateRangeAsync(from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener compras por fecha", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene compras por estado
        /// GET: api/Purchase/by-status?status=true
        /// </summary>
        [HttpGet("by-status")]
        public async Task<IActionResult> GetByStatus([FromQuery] bool status)
        {
            try
            {
                var result = await _purchaseBusiness.GetByStatusAsync(status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener compras por estado", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request para recibir compra
    /// </summary>
    public class ReceivePurchaseRequest
    {
        public bool PayInCash { get; set; }
        public int? CashSessionId { get; set; }
    }

    /// <summary>
    /// Request para cancelar compra
    /// </summary>
    public class CancelPurchaseRequest
    {
        public string Reason { get; set; }
    }
}
