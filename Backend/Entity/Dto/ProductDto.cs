namespace Entity.Dto
{
    public class ProductDto : BaseDto
    {
        public string Name { get; set; }
        public decimal UnitCost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? TaxRate { get; set; }
        public int StockOnHand { get; set; }
        public int ReorderPoint { get; set; }
        public int CategoryId { get; set; }
        public int UnitMeasureId { get; set; }
    }
}
