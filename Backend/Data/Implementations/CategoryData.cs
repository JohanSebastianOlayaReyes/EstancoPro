using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class CategoryData : BaseData<Category, CategoryDto>, ICategoryData
{
    public CategoryData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }

    // métodos específicos
}
