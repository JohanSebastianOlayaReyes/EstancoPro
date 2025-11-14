namespace Entity.Model
{
    /// <summary>
    /// Entidad ProductAlert - Alertas automáticas de productos
    /// Stock bajo, productos por vencer, productos agotados, etc.
    /// </summary>
    public class ProductAlert : Base
    {
        public int ProductId { get; set; }
        public string AlertType { get; set; }           // LowStock, OutOfStock, Expiring, Expired, ReorderPoint
        public string Message { get; set; }             // Mensaje descriptivo de la alerta
        public string Severity { get; set; }            // Info, Warning, Critical
        public bool IsRead { get; set; }                // Si la alerta ha sido leída
        public DateTime GeneratedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public int? ReadByUserId { get; set; }          // Usuario que leyó la alerta

        // Relaciones
        public Product product { get; set; }
        public User readByUser { get; set; }
    }
}
