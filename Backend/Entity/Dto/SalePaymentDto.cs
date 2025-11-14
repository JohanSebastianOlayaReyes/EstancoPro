namespace Entity.Dto
{
    public class SalePaymentDto
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public DateTime PaidAt { get; set; }
        public bool Active { get; set; }

        // Datos relacionados para respuestas
        public string PaymentMethodName { get; set; }
    }
}
