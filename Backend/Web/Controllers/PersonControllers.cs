using Business.Interfaces;
using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    public class PersonController : BaseController<Person, PersonDto>
    {
        public PersonController(IPersonBusiness personBusiness) : base(personBusiness) { }
    }
}
