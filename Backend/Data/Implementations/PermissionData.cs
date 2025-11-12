using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class PemissionData : BaseData<Permission, PermissionDto>, IPermissionData
{
    public PemissionData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }
}