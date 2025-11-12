using Utilities.Services;
using Entity.Dto;
using Entity.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entity.Context;
using BCrypt.Net;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para autenticación y autorización con soporte completo de JWT
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IJwtService jwtService,
            ApplicationDbContext context,
            ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 1. LOGIN - Inicia sesión y devuelve un token JWT con refresh token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Buscar usuario por email
                var user = await _context.users
                    .Include(u => u.rol)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Active && u.DeleteAt == null);

                if (user == null)
                {
                    _logger.LogWarning("Intento de login fallido para email: {Email}", loginDto.Email);
                    return Unauthorized(new { message = "Email o contraseña incorrectos" });
                }

                // Verificar contraseña hasheada con BCrypt
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    _logger.LogWarning("Contraseña incorrecta para email: {Email}", loginDto.Email);
                    return Unauthorized(new { message = "Email o contraseña incorrectos" });
                }

                // Generar JWT token
                var token = _jwtService.GenerateToken(user.Id, user.Email, user.rol?.TypeRol ?? "User", out string jwtId);

                // Generar refresh token
                var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, jwtId);

                var response = new LoginResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken.Token,
                    Email = user.Email,
                    RoleName = user.rol?.TypeRol ?? "User",
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    RefreshTokenExpiresAt = refreshToken.ExpiresAt
                };

                _logger.LogInformation("Login exitoso para usuario: {Email}", loginDto.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login para email: {Email}", loginDto.Email);
                return StatusCode(500, new { message = "Error al procesar la solicitud", error = ex.Message });
            }
        }

        /// <summary>
        /// 2. REFRESH TOKEN - Renueva el token JWT usando un refresh token válido
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Validar el refresh token
                var storedRefreshToken = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken);
                if (storedRefreshToken == null)
                {
                    _logger.LogWarning("Intento de refresh con token inválido o expirado");
                    return Unauthorized(new { message = "Refresh token inválido o expirado" });
                }

                // Validar que el JTI del token coincida con el del refresh token
                var jtiFromToken = _jwtService.GetJtiFromToken(request.Token);
                if (jtiFromToken != storedRefreshToken.JwtId)
                {
                    _logger.LogWarning("JTI no coincide con el refresh token");
                    return Unauthorized(new { message = "Token inválido" });
                }

                // Marcar el refresh token como usado
                storedRefreshToken.IsUsed = true;
                storedRefreshToken.UpdateAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Obtener usuario
                var user = await _context.users
                    .Include(u => u.rol)
                    .FirstOrDefaultAsync(u => u.Id == storedRefreshToken.UserId && u.Active);

                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado para refresh token");
                    return Unauthorized(new { message = "Usuario no encontrado" });
                }

                // Generar nuevo JWT token y refresh token
                var newToken = _jwtService.GenerateToken(user.Id, user.Email, user.rol?.TypeRol ?? "User", out string newJwtId);
                var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, newJwtId);

                var response = new LoginResponseDto
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken.Token,
                    Email = user.Email,
                    RoleName = user.rol?.TypeRol ?? "User",
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
                };

                _logger.LogInformation("Token renovado exitosamente para usuario: {UserId}", user.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el refresh token");
                return StatusCode(500, new { message = "Error al renovar el token", error = ex.Message });
            }
        }

        /// <summary>
        /// 3. LOGOUT - Cierra sesión revocando el refresh token
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                // Intentar obtener el userId de diferentes formas (más robusto)
                var userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("No se pudo extraer userId del token. Claims disponibles: {Claims}",
                        string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
                    return Unauthorized(new { message = "Token inválido" });
                }

                // Revocar el refresh token específico
                await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken);

                _logger.LogInformation("Logout exitoso para usuario: {UserId}", userId);
                return Ok(new { message = "Sesión cerrada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el logout");
                return StatusCode(500, new { message = "Error al cerrar sesión", error = ex.Message });
            }
        }

        /// <summary>
        /// 3b. LOGOUT ALL - Cierra todas las sesiones del usuario (logout global)
        /// </summary>
        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogoutAll()
        {
            try
            {
                // Intentar obtener el userId de diferentes formas (más robusto)
                var userIdStr = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    _logger.LogWarning("No se pudo extraer userId del token para logout-all. Claims disponibles: {Claims}",
                        string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
                    return Unauthorized(new { message = "Token inválido" });
                }

                // Revocar todos los refresh tokens del usuario
                await _jwtService.RevokeAllUserTokensAsync(userId);

                _logger.LogInformation("Logout global exitoso para usuario: {UserId}", userId);
                return Ok(new { message = "Todas las sesiones han sido cerradas exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el logout global");
                return StatusCode(500, new { message = "Error al cerrar sesiones", error = ex.Message });
            }
        }

        /// <summary>
        /// 4. VALIDATE - Valida el token actual y devuelve información del usuario
        /// </summary>
        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            // Intentar obtener claims de diferentes formas (más robusto)
            var userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            var email = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value;

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                ?? User.FindFirst("role")?.Value;

            return Ok(new
            {
                valid = true,
                userId,
                email,
                role
            });
        }
    }
}
