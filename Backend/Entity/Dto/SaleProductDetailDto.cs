namespace Entity.Dto
{
    public class SaleProductDetailDto
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal LineSubtotal { get; set; }
        public decimal LineTax { get; set; }
        public decimal LineTotal { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int UnitMeasureId { get; set; }
    }
}
