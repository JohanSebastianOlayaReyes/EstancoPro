namespace Entity.Model
{
    public class Person : Base
    {
        public string FullName { get; set; }
        public int PhoneNumber { get; set; }
        public int NumberIdentification { get; set; }
        public User users { get; set; }
    }
}
