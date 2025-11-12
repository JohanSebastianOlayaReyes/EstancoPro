using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Utilities.Services
{
    /// <summary>
    /// Servicio para generar y validar tokens JWT con soporte de Refresh Token
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public JwtService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Genera un token JWT para un usuario
        /// </summary>
        public string GenerateToken(int userId, string email, string roleName, out string jwtId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            jwtId = Guid.NewGuid().ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Genera un refresh token único y lo almacena en la base de datos
        /// </summary>
        public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId, string jwtId)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = GenerateSecureRandomToken(),
                JwtId = jwtId,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 días de validez
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                Active = true,
                IsRevoked = false,
                IsUsed = false
            };

            await _context.Set<RefreshToken>().AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        /// <summary>
        /// Valida un token JWT
        /// </summary>
        public ClaimsPrincipal ValidateToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Valida un refresh token y verifica que no haya sido revocado o usado
        /// </summary>
        public async Task<RefreshToken> ValidateRefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.Active && rt.DeleteAt == null);

            if (storedToken == null)
                return null;

            // Verificar si el token ha expirado
            if (storedToken.ExpiresAt < DateTime.UtcNow)
                return null;

            // Verificar si el token ha sido revocado
            if (storedToken.IsRevoked)
                return null;

            // Verificar si el token ya fue usado
            if (storedToken.IsUsed)
                return null;

            return storedToken;
        }

        /// <summary>
        /// Revoca un refresh token específico
        /// </summary>
        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.Active);

            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                storedToken.UpdateAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Revoca todos los refresh tokens de un usuario (útil para logout global)
        /// </summary>
        public async Task RevokeAllUserTokensAsync(int userId)
        {
            var userTokens = await _context.Set<RefreshToken>()
                .Where(rt => rt.UserId == userId && rt.Active && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
                token.UpdateAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Extrae el ID de usuario del token
        /// </summary>
        public int GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null)
                return 0;

            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return 0;
        }

        /// <summary>
        /// Extrae el JTI (JWT ID) del token
        /// </summary>
        public string GetJtiFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null)
                return null;

            return principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        }

        /// <summary>
        /// Genera un token aleatorio seguro para refresh token
        /// </summary>
        private string GenerateSecureRandomToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }
}
