namespace Entity.Model
{
    public class UserRol : Base
    {
        public int UserId { set; get; }
        public int RolId { set; get; }
        public Rol rol { set; get; }
        public User user{ set; get; }
    }
}