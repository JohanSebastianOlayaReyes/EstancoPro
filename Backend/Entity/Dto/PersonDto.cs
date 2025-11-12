namespace Entity.Dto
{
    public class PersonDto : BaseDto
    {
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string? SecondName { get; set; }
        public string FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        public int PhoneNumber { get; set; }
        public int NumberIdentification { get; set; }
    }
}
