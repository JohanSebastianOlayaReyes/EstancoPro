using Business.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para CashSession con flujo de apertura/cierre de caja
    /// </summary>
    public class CashSessionController : BaseController<CashSession, CashSessionDto>
    {
        private readonly ICashSessionBusiness _cashSessionBusiness;

        public CashSessionController(ICashSessionBusiness cashSessionBusiness) : base(cashSessionBusiness)
        {
            _cashSessionBusiness = cashSessionBusiness;
        }

        /// <summary>
        /// Abre una nueva sesión de caja (valida que no haya otra abierta)
        /// POST: api/CashSession/open
        /// Body: { "openingAmount": 100000 }
        /// </summary>
        [HttpPost("open")]
        public async Task<IActionResult> OpenSession([FromBody] OpenSessionRequest request)
        {
            if (request.OpeningAmount < 0)
                return BadRequest(new { message = "El monto de apertura no puede ser negativo" });

            try
            {
                var result = await _cashSessionBusiness.OpenSessionAsync(request.OpeningAmount);
                return StatusCode(201, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al abrir sesión de caja", error = ex.Message });
            }
        }

        /// <summary>
        /// Cierra una sesión de caja y calcula la diferencia
        /// POST: api/CashSession/{id}/close
        /// Body: { "closingAmount": 150000 }
        /// </summary>
        [HttpPost("{id:int}/close")]
        public async Task<IActionResult> CloseSession(int id, [FromBody] CloseSessionRequest request)
        {
            try
            {
                var difference = await _cashSessionBusiness.CloseSessionAsync(id, request.ClosingAmount);

                return Ok(new
                {
                    message = "Sesión cerrada exitosamente",
                    difference = difference,
                    status = difference >= 0 ? "Sobrante" : "Faltante"
                });
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
                return StatusCode(500, new { message = "Error al cerrar sesión de caja", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la sesión de caja actualmente abierta
        /// GET: api/CashSession/open
        /// </summary>
        [HttpGet("open")]
        public async Task<IActionResult> GetOpenSession()
        {
            try
            {
                var result = await _cashSessionBusiness.GetOpenSessionAsync();

                if (result == null)
                    return NotFound(new { message = "No hay sesión de caja abierta" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener sesión abierta", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene sesiones de caja en un rango de fechas
        /// GET: api/CashSession/by-date-range?from=2024-01-01&to=2024-12-31
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                var result = await _cashSessionBusiness.GetByDateRangeAsync(from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener sesiones por fecha", error = ex.Message });
            }
        }

        /// <summary>
        /// Calcula el balance de una sesión (esperado, real, diferencia)
        /// GET: api/CashSession/{id}/balance
        /// </summary>
        [HttpGet("{id:int}/balance")]
        public async Task<IActionResult> GetSessionBalance(int id)
        {
            try
            {
                var result = await _cashSessionBusiness.GetSessionBalanceAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener balance de sesión", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request para abrir sesión
    /// </summary>
    public class OpenSessionRequest
    {
        public decimal OpeningAmount { get; set; }
    }

    /// <summary>
    /// Request para cerrar sesión
    /// </summary>
    public class CloseSessionRequest
    {
        public decimal ClosingAmount { get; set; }
    }
}
