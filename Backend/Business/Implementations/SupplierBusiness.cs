namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para Supplier - Solo CRUD básico
/// Las consultas de "compras por proveedor" se hacen desde PurchaseBusiness
/// </summary>
public class SupplierBusiness : BaseBusiness<Supplier, SupplierDto>, ISupplierBusiness
{
    public SupplierBusiness(
        ISupplierData supplierData,
        ILogger<BaseBusiness<Supplier, SupplierDto>> logger)
        : base(supplierData, logger)
    {
    }
}
