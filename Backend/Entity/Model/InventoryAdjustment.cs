namespace Entity.Model
{
    /// <summary>
    /// Entidad InventoryAdjustment - Ajustes de inventario
    /// Para manejar mermas, pérdidas, robos, vencimientos, conteos físicos, etc.
    /// </summary>
    public class InventoryAdjustment : Base
    {
        public int ProductId { get; set; }
        public int AdjustedQuantity { get; set; }       // Cantidad ajustada (positiva o negativa)
        public int PreviousStock { get; set; }          // Stock antes del ajuste
        public int NewStock { get; set; }               // Stock después del ajuste
        public string AdjustmentType { get; set; }      // Increase, Decrease
        public string Reason { get; set; }              // Merma, Robo, Conteo, Vencimiento, Donación, etc.
        public string Notes { get; set; }               // Notas adicionales
        public DateTime AdjustedAt { get; set; }
        public int UserId { get; set; }                 // Quién hizo el ajuste (auditoría)

        // Relaciones
        public Product product { get; set; }
        public User user { get; set; }
    }
}
