using System.Net.Http.Headers;

namespace Entity.Model
{
    public class Category : Base
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Product> products{ get; set; }
    }
}