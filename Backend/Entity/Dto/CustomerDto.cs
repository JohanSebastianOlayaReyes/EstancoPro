namespace Entity.Dto
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal LoyaltyPoints { get; set; }
        public bool Active { get; set; }
    }
}
