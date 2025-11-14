namespace Entity.Model
{
    /// <summary>
    /// Entidad Expense - Gastos operativos del negocio
    /// Servicios públicos, arriendo, nómina, transporte, etc.
    /// </summary>
    public class Expense : Base
    {
        public DateTime ExpenseDate { get; set; }
        public string Category { get; set; }            // Servicios, Arriendo, Nómina, Transporte, etc.
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string InvoiceNumber { get; set; }       // Número de factura del gasto
        public int? CashSessionId { get; set; }         // Si se pagó de caja (nullable)
        public int? SupplierId { get; set; }            // Proveedor asociado (nullable)
        public string PaymentStatus { get; set; }       // Paid, Pending, Cancelled
        public DateTime? PaidAt { get; set; }

        // Relaciones
        public CashSession cashSession { get; set; }
        public Supplier supplier { get; set; }
    }
}
