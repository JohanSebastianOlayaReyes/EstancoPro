namespace Entity.Model
{
    public class Supplier : Base
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public ICollection<Purchase> purchases { get; set; }
    }
}