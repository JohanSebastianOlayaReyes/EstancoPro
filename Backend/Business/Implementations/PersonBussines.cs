namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Business.Interfaces.Base;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

/// <summary>
/// Capa de negocio para la entidad Person.
/// Hereda del BaseBusiness y utiliza la lógica genérica sin agregar métodos específicos.
/// </summary>
public class PersonBusiness : BaseBusiness<Person, PersonDto>, IPersonBusiness
{
    private readonly IPersonData _personData;

    public PersonBusiness(IPersonData personData, ILogger<BaseBusiness<Person, PersonDto>> logger)
        : base(personData, logger)
    {
        _personData = personData;
    }
}
