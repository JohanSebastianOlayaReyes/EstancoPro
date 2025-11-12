namespace Entity.Model
{
    public class User : Base
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
        public int PersonId { get; set; }
        public Rol rol { get; set; }
        public Person person { get; set; }
        public ICollection<UserRol> userrols { get; set; }
    }
}
