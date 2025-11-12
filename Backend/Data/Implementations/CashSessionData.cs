using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class CashSessionData : BaseData<CashSession, CashSessionDto>, ICashSessionData
{
    public CashSessionData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }

    // métodos específicos
}
