using Business.Interfaces;
using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    public class RolFormPermissionController : BaseController<RolFormPermission, RolFormPermissionDto>
    {
        public RolFormPermissionController(IRolFormPermissionBusiness business) : base(business) { }
    }
}
