namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para la entidad FormModule.
/// Hereda la funcionalidad base sin métodos personalizados.
/// </summary>
public class FormModuleBusiness : BaseBusiness<FormModule, FormModuleDto>, IFormModuleBusiness
{
    private readonly IFormModuleData _formModuleData;

    public FormModuleBusiness(IFormModuleData formModuleData, ILogger<BaseBusiness<FormModule, FormModuleDto>> logger)
        : base(formModuleData, logger)
    {
        _formModuleData = formModuleData;
    }
}
