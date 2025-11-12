using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class FormModuleData : BaseData<FormModule, FormModuleDto>, IFormModuleData
{
    public FormModuleData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }
}