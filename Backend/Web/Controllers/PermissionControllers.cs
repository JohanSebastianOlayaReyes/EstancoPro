using Business.Interfaces;
using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    public class PermissionController : BaseController<Permission, PermissionDto>
    {
        public PermissionController(IPermissionBusiness permissionBusiness) : base(permissionBusiness) { }
    }
}
