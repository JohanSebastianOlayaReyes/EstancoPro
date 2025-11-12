namespace Entity.Model
{
    public class Form : Base
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public ICollection<FormModule> formModules { get; set; }
        public ICollection<RolFormPermission> rolFormPermissions { get; set; }
    }
}
