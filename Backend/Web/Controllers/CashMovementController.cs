using Business.Interfaces;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para CashMovement (movimientos de caja)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CashMovementController : ControllerBase
    {
        private readonly ICashMovementBusiness _cashMovementBusiness;

        public CashMovementController(ICashMovementBusiness cashMovementBusiness)
        {
            _cashMovementBusiness = cashMovementBusiness;
        }

        /// <summary>
        /// Obtiene todos los movimientos de caja
        /// GET: api/CashMovement
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _cashMovementBusiness.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener movimientos de caja", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un movimiento de caja por IDs compuestos
        /// GET: api/CashMovement/{cashSessionId}/{at}
        /// </summary>
        [HttpGet("{cashSessionId:int}/{at:datetime}")]
        public async Task<IActionResult> GetById(int cashSessionId, DateTime at)
        {
            try
            {
                var result = await _cashMovementBusiness.GetByIdAsync(cashSessionId, at);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener movimiento de caja", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo movimiento de caja
        /// POST: api/CashMovement
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CashMovementDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _cashMovementBusiness.CreateAsync(dto);
                return StatusCode(201, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear movimiento de caja", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un movimiento de caja existente
        /// PUT: api/CashMovement/{cashSessionId}/{at}
        /// </summary>
        [HttpPut("{cashSessionId:int}/{at:datetime}")]
        public async Task<IActionResult> Update(int cashSessionId, DateTime at, [FromBody] CashMovementDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _cashMovementBusiness.UpdateAsync(cashSessionId, at, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar movimiento de caja", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un movimiento de caja
        /// DELETE: api/CashMovement/{cashSessionId}/{at}
        /// </summary>
        [HttpDelete("{cashSessionId:int}/{at:datetime}")]
        public async Task<IActionResult> Delete(int cashSessionId, DateTime at)
        {
            try
            {
                await _cashMovementBusiness.DeleteAsync(cashSessionId, at);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar movimiento de caja", error = ex.Message });
            }
        }
    }
}
