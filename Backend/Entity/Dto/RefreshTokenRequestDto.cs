using System.ComponentModel.DataAnnotations;

namespace Entity.Dto
{
    /// <summary>
    /// DTO para solicitar un nuevo token usando refresh token
    /// </summary>
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "El token es requerido")]
        public string Token { get; set; }

        [Required(ErrorMessage = "El refresh token es requerido")]
        public string RefreshToken { get; set; }
    }
}
