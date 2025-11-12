namespace Entity.Model
{
    public class Purchase : Base
    {
        public DateTime? OrderedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public bool status { get; set; }
        public decimal TotalCost { get; set; }
        public int SupplierId { get; set; }
        public Supplier supplier { get; set; }
        public ICollection<PurchaseProductDetail> purchaseproductdetail { get; set; }
    }
}