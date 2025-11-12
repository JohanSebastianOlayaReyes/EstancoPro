using Business.Interfaces;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para Supplier - Solo CRUD básico
    /// </summary>
    public class SupplierController : BaseController<Supplier, SupplierDto>
    {
        public SupplierController(ISupplierBusiness supplierBusiness) : base(supplierBusiness) { }
    }
}
