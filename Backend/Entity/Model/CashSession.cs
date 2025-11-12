namespace Entity.Model
{
    public class CashSession : Base
    {
        public DateTime OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal OpeningAmount { get; set; }
        public decimal ClosingAmount { get; set; }
        public ICollection<CashMovement> cashmovement { get; set; }
        public ICollection<Sale> sele { get; set; }
    }
}