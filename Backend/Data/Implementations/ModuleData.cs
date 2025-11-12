using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class ModuleData : BaseData<Module, ModuleDto>, IModuleData
{
    public ModuleData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }
}