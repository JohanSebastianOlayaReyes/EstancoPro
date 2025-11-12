namespace Entity.Dto
{
    public class PurchaseProductDetailDto : BaseDto
    {
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int UnitMeasureId { get; set; }
    }
}
