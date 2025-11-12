namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

public class PermissionBusiness : BaseBusiness<Permission, PermissionDto>, IPermissionBusiness
{
    private readonly IPermissionData _permissionData;

    public PermissionBusiness(IPermissionData permissionData, ILogger<BaseBusiness<Permission, PermissionDto>> logger)
        : base(permissionData, logger)
    {
        _permissionData = permissionData;
    }
}
