namespace Business.Interfaces;

using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

/// <summary>
/// Interfaz de negocio para Supplier - Solo CRUD básico
/// </summary>
public interface ISupplierBusiness : IBaseBusiness<Supplier, SupplierDto>
{
}
