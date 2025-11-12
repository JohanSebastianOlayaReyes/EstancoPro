using Business.Interfaces;
using Entity.Dto;
using Entity.Model;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controlador para PurchaseProductDetail - Solo CRUD básico
    /// Se gestiona en contexto de Purchase
    /// </summary>
    public class PurchaseProductDetailController : BaseController<PurchaseProductDetail, PurchaseProductDetailDto>
    {
        public PurchaseProductDetailController(IPurchaseProductDetailBusiness purchaseProductDetailBusiness)
            : base(purchaseProductDetailBusiness) { }
    }
}
