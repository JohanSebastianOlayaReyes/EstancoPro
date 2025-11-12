namespace Entity.Model
{
    public class Sale : Base
    {
        public DateTime SoldAt { get; set; }
        public string Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public int CashSessionId { get; set; }
        public CashSession cashSession { get; set; }
        public ICollection<SaleProductDetail> saleproductdetail { get; set; }
    }
}