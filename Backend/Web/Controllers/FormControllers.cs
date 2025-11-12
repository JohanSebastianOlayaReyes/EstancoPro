using Business.Interfaces;
using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    public class FormController : BaseController<Form, FormDto>
    {
        public FormController(IFormBusiness formBusiness) : base(formBusiness) { }
    }
}
