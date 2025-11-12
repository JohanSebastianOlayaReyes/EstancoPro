namespace Entity.Model
{
    public class Module : Base
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<FormModule> formModules { get; set; }
    }
}
