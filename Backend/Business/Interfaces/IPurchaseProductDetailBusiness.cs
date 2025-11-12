namespace Business.Interfaces;

using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

/// <summary>
/// Interfaz de negocio para PurchaseProductDetail - Solo CRUD básico
/// </summary>
public interface IPurchaseProductDetailBusiness : IBaseBusiness<PurchaseProductDetail, PurchaseProductDetailDto>
{
}
