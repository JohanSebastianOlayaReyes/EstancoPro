using Business.Interfaces;
using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    public class UserController : BaseController<User, UserDto>
    {
        public UserController(IUserBusiness userBusiness) : base(userBusiness) { }
    }
}
