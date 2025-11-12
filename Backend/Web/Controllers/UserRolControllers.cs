using Business.Interfaces;
using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para la entidad UserRol.
    /// Hereda de BaseController y usa la lógica genérica sin métodos personalizados.
    /// </summary>
    public class UserRolController : BaseController<UserRol, UserRolDto>
    {
        public UserRolController(IUserRolBusiness userRolBusiness) : base(userRolBusiness){}
    }
}
