namespace Entity.Model
{
    public class RolFormPermission : Base
    {
        public int RolId { get; set; }
        public int FormId { get; set; }
        public int PermissionId { get; set; }
        public Rol rol { get; set; }
        public Form form { get; set; }
        public Permission permission { get; set; }
    }
}
