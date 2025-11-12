namespace Entity.Model
{
    public class PurchaseProductDetail : Base
    {
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int UnitMeasureId { get; set; }
        public Purchase purchase { get; set; }
        public Product product { get; set; }
        public UnitMeasure unitMeasure { get; set; }
    }
}