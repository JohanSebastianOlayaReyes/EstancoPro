namespace Entity.Dto
{
    public class PaymentMethodDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool RequiresReference { get; set; }
        public bool IsActive { get; set; }
        public bool Active { get; set; }
    }
}
