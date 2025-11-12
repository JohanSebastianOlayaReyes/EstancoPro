using Business.Interfaces;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para UnitMeasure - Solo CRUD básico
    /// </summary>
    public class UnitMeasureController : BaseController<UnitMeasure, UnitMeasureDto>
    {
        public UnitMeasureController(IUnitMeasureBusiness unitMeasureBusiness) : base(unitMeasureBusiness) { }
    }
}
