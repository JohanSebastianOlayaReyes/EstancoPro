namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Business.Interfaces.Base;
using Data.Interfaces;
using Entity.Model;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para la entidad User.
/// Hereda de BaseBusiness y puede extender la lógica con reglas específicas.
/// </summary>
public class UserBusiness : BaseBusiness<User, UserDto>, IUserBusiness
{
    private readonly IUserData _userData; // Interfaz específica de datos

    public UserBusiness(IUserData userData, ILogger<BaseBusiness<User, UserDto>> logger)
        : base(userData, logger)
    {
        _userData = userData;
    }
}
