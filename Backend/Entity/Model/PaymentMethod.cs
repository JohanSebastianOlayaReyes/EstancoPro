namespace Entity.Model
{
    /// <summary>
    /// Entidad PaymentMethod - Métodos de pago disponibles
    /// Efectivo, Tarjeta, Nequi, Daviplata, Transferencia, etc.
    /// </summary>
    public class PaymentMethod : Base
    {
        public string Name { get; set; }                // Efectivo, Tarjeta Débito, Nequi, etc.
        public string Type { get; set; }                // Cash, Card, DigitalWallet, Transfer
        public bool RequiresReference { get; set; }     // True para transferencias/pagos digitales
        public bool IsActive { get; set; }              // Método activo o no

        // Relaciones
        public ICollection<SalePayment> salePayments { get; set; }
    }
}
