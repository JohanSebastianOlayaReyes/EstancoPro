namespace Entity.Model
{
    public class FormModule : Base
    {
        public  int FormId { get; set; }
        public  int ModuleId { get; set; }
        public Form form { get; set; }
        public Module module { get; set; }
    }
}
