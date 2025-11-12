using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class PurchaseData : BaseData<Purchase, PurchaseDto>, IPurchaseData
{
    public PurchaseData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }

    // métodos específicos
}
