namespace Entity.Dto
{
    public class ProductUnitPriceDto
    {
        public int ProductId { get; set; }
        public int UnitMeasureId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal ConversionFactor { get; set; }
        public string? Barcode { get; set; }
    }
}
