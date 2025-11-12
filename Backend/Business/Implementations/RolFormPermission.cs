namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Business.Interfaces.Base;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para la entidad RoleFormPermission.
/// Gestiona la relación entre roles, formularios y permisos.
/// </summary>
public class RolFormPermissionBusiness : BaseBusiness<RolFormPermission, RolFormPermissionDto>, IRolFormPermissionBusiness
{
    private readonly IRolFormPermissionData _rolFormPermissionData;

    public RolFormPermissionBusiness(IRolFormPermissionData rolFormPermissionData, ILogger<BaseBusiness<RolFormPermission, RolFormPermissionDto>> logger)
        : base(rolFormPermissionData, logger)
    {
        _rolFormPermissionData = rolFormPermissionData;
    }
}
