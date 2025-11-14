namespace Entity.Dto
{
    public class InventoryAdjustmentDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int AdjustedQuantity { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public string AdjustmentType { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        public DateTime AdjustedAt { get; set; }
        public int UserId { get; set; }
        public bool Active { get; set; }

        // Datos relacionados para respuestas
        public string ProductName { get; set; }
        public string UserName { get; set; }
    }
}
