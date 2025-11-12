using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class RolFormPermissionData : BaseData<RolFormPermission, RolFormPermissionDto>, IRolFormPermissionData
{
    public RolFormPermissionData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }
}