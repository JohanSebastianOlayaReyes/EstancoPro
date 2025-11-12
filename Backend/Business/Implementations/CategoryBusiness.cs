namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para Category - Solo CRUD básico
/// Las consultas de "productos por categoría" se hacen desde ProductBusiness
/// </summary>
public class CategoryBusiness : BaseBusiness<Category, CategoryDto>, ICategoryBusiness
{
    public CategoryBusiness(
        ICategoryData categoryData,
        ILogger<BaseBusiness<Category, CategoryDto>> logger)
        : base(categoryData, logger)
    {
    }
}
