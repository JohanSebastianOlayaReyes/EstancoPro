namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Business.Interfaces.Base;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

public class ModuleBusiness : BaseBusiness<Module, ModuleDto>, IModuleBusiness
{
    private readonly IModuleData _moduleData;

    public ModuleBusiness(IModuleData moduleData, ILogger<BaseBusiness<Module, ModuleDto>> logger)
        : base(moduleData, logger)
    {
        _moduleData = moduleData;
    }
}
