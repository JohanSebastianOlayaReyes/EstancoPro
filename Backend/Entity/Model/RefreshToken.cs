namespace Entity.Model
{
    /// <summary>
    /// Entidad para almacenar Refresh Tokens
    /// </summary>
    public class RefreshToken : Base
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }

        // Navegación
        public User User { get; set; }
    }
}
