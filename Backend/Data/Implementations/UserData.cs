using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class UserData : BaseData<User, UserDto>, IUserData
{
    public UserData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }
}