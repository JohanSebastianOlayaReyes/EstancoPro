namespace Entity.Model
{
    public class Permission : Base
    {
        public string TypePermission { get; set; }
        public string Description { get; set; }
        public ICollection<RolFormPermission> rolFormPermissions { get; set; }
    }
}
