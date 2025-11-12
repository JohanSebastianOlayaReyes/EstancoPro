namespace Entity.Dto
{
    public class CashMovementDto
    {
        public int CashSessionId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
        public DateTime At { get; set; }
        public int? RelatedId { get; set; }
        public string? RelatedEntity { get; set; }
    }
}
