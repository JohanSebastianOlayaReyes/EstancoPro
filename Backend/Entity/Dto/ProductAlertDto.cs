namespace Entity.Dto
{
    public class ProductAlertDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public bool IsRead { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public int? ReadByUserId { get; set; }
        public bool Active { get; set; }

        // Datos relacionados para respuestas
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderPoint { get; set; }
    }
}
