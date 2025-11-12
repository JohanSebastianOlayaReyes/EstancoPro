using Business.Interfaces;
using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    public class RolController : BaseController<Rol, RolDto>
    {
        public RolController(IRolBusiness rolBusiness) : base(rolBusiness) { }
    }
}
