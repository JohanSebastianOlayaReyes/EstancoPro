namespace Entity.Dto
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string InvoiceNumber { get; set; }
        public int? CashSessionId { get; set; }
        public int? SupplierId { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaidAt { get; set; }
        public bool Active { get; set; }

        // Datos relacionados para respuestas
        public string SupplierName { get; set; }
    }
}
