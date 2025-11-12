using System.ComponentModel.DataAnnotations;

namespace Entity.Dto
{
    /// <summary>
    /// DTO para el inicio de sesión
    /// </summary>
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
