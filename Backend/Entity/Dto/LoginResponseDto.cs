namespace Entity.Dto
{
    /// <summary>
    /// DTO para la respuesta del inicio de sesión
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
