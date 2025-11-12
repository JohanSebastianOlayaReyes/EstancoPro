using Business.Interfaces;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para Category - Solo CRUD básico
    /// </summary>
    public class CategoryController : BaseController<Category, CategoryDto>
    {
        public CategoryController(ICategoryBusiness categoryBusiness) : base(categoryBusiness) { }
    }
}
