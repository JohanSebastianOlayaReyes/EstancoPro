namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Business.Interfaces.Base;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

public class UserRolBusiness : BaseBusiness<UserRol, UserRolDto>, IUserRolBusiness
{
    private readonly IRolUserData _userRolData;

    public UserRolBusiness(IRolUserData userRolData, ILogger<BaseBusiness<UserRol, UserRolDto>> logger)
        : base(userRolData, logger)
    {
        _userRolData = userRolData;
    }
}
