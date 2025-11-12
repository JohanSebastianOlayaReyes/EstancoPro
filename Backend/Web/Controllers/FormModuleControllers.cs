using Business.Interfaces;
using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    public class FormModuleController : BaseController<FormModule, FormModuleDto>
    {
        public FormModuleController(IFormModuleBusiness formModuleBusiness) : base(formModuleBusiness) { }
    }
}
