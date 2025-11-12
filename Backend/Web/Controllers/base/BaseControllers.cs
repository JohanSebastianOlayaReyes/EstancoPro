using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador base genérico con soporte para DTOs
    /// T = Entidad de base de datos
    /// D = DTO (Data Transfer Object)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T, D> : ControllerBase
        where T : Base
        where D : BaseDto
    {
        protected readonly IBaseBusiness<T, D> _business;

        protected BaseController(IBaseBusiness<T, D> business)
        {
            _business = business;
        }

        /// <summary>Obtiene todos los registros activos</summary>
        [HttpGet]
        public virtual async Task<IActionResult> GetAllAsync([FromQuery] bool includeDeleted = false)
        {
            try
            {
                var result = await _business.GetAllAsync(includeDeleted);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros", error = ex.Message });
            }
        }

        /// <summary>Obtiene un registro por su ID</summary>
        [HttpGet("{id:int}")]
        public virtual async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var result = await _business.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el registro", error = ex.Message });
            }
        }

        /// <summary>Crea un nuevo registro</summary>
        [HttpPost]
        public virtual async Task<IActionResult> CreateAsync([FromBody] D dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _business.CreateAsync(dto);
                return StatusCode(201, result); // Created
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el registro", error = ex.Message });
            }
        }

        /// <summary>Actualiza completamente un registro existente</summary>
        [HttpPut("{id:int}")]
        public virtual async Task<IActionResult> UpdateAsync(int id, [FromBody] D dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _business.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el registro", error = ex.Message });
            }
        }

        /// <summary>Actualiza parcialmente un registro existente</summary>
        [HttpPatch("{id:int}")]
        public virtual async Task<IActionResult> PatchAsync(int id, [FromBody] D dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _business.PatchAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el registro", error = ex.Message });
            }
        }

        /// <summary>Elimina lógicamente un registro (soft delete)</summary>
        [HttpDelete("logic/{id:int}")]
        public virtual async Task<IActionResult> DeleteLogicAsync(int id)
        {
            try
            {
                await _business.DeleteLogicAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el registro", error = ex.Message });
            }
        }

        /// <summary>Elimina permanentemente un registro de la base de datos</summary>
        [HttpDelete("permanent/{id:int}")]
        public virtual async Task<IActionResult> DeletePermanentAsync(int id)
        {
            try
            {
                await _business.DeletePermanentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el registro", error = ex.Message });
            }
        }
    }
}
