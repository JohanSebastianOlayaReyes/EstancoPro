namespace Entity.Model
{
    /// <summary>
    /// Entidad SalePayment - Detalle de pagos de una venta
    /// Permite ventas con múltiples métodos de pago
    /// Ejemplo: Cliente paga $10,000 en efectivo + $5,000 con Nequi
    /// </summary>
    public class SalePayment : Base
    {
        public int SaleId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Amount { get; set; }             // Monto pagado con este método
        public string Reference { get; set; }           // Número de transacción, aprobación, etc.
        public DateTime PaidAt { get; set; }            // Fecha/hora del pago

        // Relaciones
        public Sale sale { get; set; }
        public PaymentMethod paymentMethod { get; set; }
    }
}
