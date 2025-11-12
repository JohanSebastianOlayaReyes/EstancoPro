using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class RolData : BaseData<Rol, RolDto>, IRolData
{
    public RolData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }
}