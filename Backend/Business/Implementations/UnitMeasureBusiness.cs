namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para UnitMeasure - Solo CRUD básico
/// Validación: ConversionFactor debe ser > 0 (se valida en Data layer)
/// </summary>
public class UnitMeasureBusiness : BaseBusiness<UnitMeasure, UnitMeasureDto>, IUnitMeasureBusiness
{
    public UnitMeasureBusiness(
        IUnitMeasureData unitMeasureData,
        ILogger<BaseBusiness<UnitMeasure, UnitMeasureDto>> logger)
        : base(unitMeasureData, logger)
    {
    }
}
