namespace Entity.Dto
{
    public class PurchaseDto : BaseDto
    {
        public DateTime? OrderedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public bool Status { get; set; }
        public decimal TotalCost { get; set; }
        public int SupplierId { get; set; }
    }
}
