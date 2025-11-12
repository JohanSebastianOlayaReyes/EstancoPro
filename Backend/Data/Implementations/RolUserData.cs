using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class RolUserData : BaseData<UserRol, UserRolDto>, IRolUserData
{
    public RolUserData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }
}