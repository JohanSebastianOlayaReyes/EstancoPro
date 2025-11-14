namespace Entity.Model
{
    /// <summary>
    /// Entidad Customer - Clientes del estanco
    /// Importante para facturación electrónica y programas de fidelización
    /// </summary>
    public class Customer : Base
    {
        public string DocumentType { get; set; }        // CC, NIT, CE, Pasaporte
        public string DocumentNumber { get; set; }      // Número de documento único
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal LoyaltyPoints { get; set; }      // Puntos de fidelización

        // Relaciones
        public ICollection<Sale> sales { get; set; }
    }
}
