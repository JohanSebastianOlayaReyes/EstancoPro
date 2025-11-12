namespace Entity.Model
{
    public class UnitMeasure : Base
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Product> products { get; set; }
        public ICollection<ProductUnitPrice> productunitprices { get; set; }
        public ICollection<PurchaseProductDetail> purchaseproductdetails { get; set; }
        public ICollection<SaleProductDetail> saleproductdetail { get; set; }
    }
}