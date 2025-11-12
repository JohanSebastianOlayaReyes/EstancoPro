namespace Entity.Dto
{
    public class CashSessionDto : BaseDto
    {
        public DateTime OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal OpeningAmount { get; set; }
        public decimal ClosingAmount { get; set; }
    }
}
