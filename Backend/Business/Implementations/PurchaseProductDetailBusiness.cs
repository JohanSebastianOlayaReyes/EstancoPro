namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para PurchaseProductDetail - Solo CRUD básico
/// Se gestiona en contexto de Purchase (ya tiene purchaseId)
/// El cálculo de stock se hace en PurchaseBusiness.ReceivePurchaseAsync
/// </summary>
public class PurchaseProductDetailBusiness : BaseBusiness<PurchaseProductDetail, PurchaseProductDetailDto>, IPurchaseProductDetailBusiness
{
    public PurchaseProductDetailBusiness(
        IPurchaseProductDetailData purchaseProductDetailData,
        ILogger<BaseBusiness<PurchaseProductDetail, PurchaseProductDetailDto>> logger)
        : base(purchaseProductDetailData, logger)
    {
    }
}
