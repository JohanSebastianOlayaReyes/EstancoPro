using Entity.Model;
using System.Security.Claims;

namespace Utilities.Services
{
    /// <summary>
    /// Interfaz para el servicio de JWT
    /// </summary>
    public interface IJwtService
    {
        /// <summary>Genera un token JWT para un usuario</summary>
        string GenerateToken(int userId, string email, string roleName, out string jwtId);

        /// <summary>Genera un refresh token</summary>
        Task<RefreshToken> GenerateRefreshTokenAsync(int userId, string jwtId);

        /// <summary>Valida un token JWT</summary>
        ClaimsPrincipal ValidateToken(string token);

        /// <summary>Valida un refresh token</summary>
        Task<RefreshToken> ValidateRefreshTokenAsync(string refreshToken);

        /// <summary>Revoca un refresh token</summary>
        Task RevokeRefreshTokenAsync(string refreshToken);

        /// <summary>Revoca todos los refresh tokens de un usuario</summary>
        Task RevokeAllUserTokensAsync(int userId);

        /// <summary>Extrae el ID de usuario del token</summary>
        int GetUserIdFromToken(string token);

        /// <summary>Extrae el JTI del token</summary>
        string GetJtiFromToken(string token);
    }
}
