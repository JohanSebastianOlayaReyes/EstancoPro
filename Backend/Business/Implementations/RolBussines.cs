namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Business.Interfaces.Base;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

public class RolBusiness : BaseBusiness<Rol, RolDto>, IRolBusiness
{
    private readonly IRolData _rolData;

    public RolBusiness(IRolData rolData, ILogger<BaseBusiness<Rol, RolDto>> logger)
        : base(rolData, logger)
    {
        _rolData = rolData;
    }
}
