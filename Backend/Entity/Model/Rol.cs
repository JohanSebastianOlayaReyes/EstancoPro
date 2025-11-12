namespace Entity.Model
{
    public class Rol : Base
    {
        public string TypeRol { get; set; }
        public string Description { get; set; }
        public ICollection<UserRol> userrols { get; set; }
        public ICollection<RolFormPermission> rolFormPermissions { get; set; }
    }
}
