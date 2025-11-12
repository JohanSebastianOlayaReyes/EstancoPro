namespace Business.Interfaces;

using Business.Interfaces.Base;
using Entity.Dto;
using Entity.Model;

/// <summary>
/// Interfaz de negocio para Category - Solo CRUD básico
/// </summary>
public interface ICategoryBusiness : IBaseBusiness<Category, CategoryDto>
{
}
