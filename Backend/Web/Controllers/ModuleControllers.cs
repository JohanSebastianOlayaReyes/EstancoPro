using Business.Interfaces;
using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    public class ModuleController : BaseController<Module, ModuleDto>
    {
        public ModuleController(IModuleBusiness moduleBusiness) : base(moduleBusiness) { }
    }
}
