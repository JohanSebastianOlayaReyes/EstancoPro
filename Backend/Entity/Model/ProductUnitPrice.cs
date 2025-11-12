namespace Entity.Model
{
    public class ProductUnitPrice
    {
        public int ProductId { get; set; }
        public int UnitMeasureId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal ConversionFactor { get; set; }
        public string? Barcode { get; set; }
        public Product product { get; set; }
        public  UnitMeasure  unitmeasure { get; set; }
    }
}