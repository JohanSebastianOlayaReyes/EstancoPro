using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class SupplierData : BaseData<Supplier, SupplierDto>, ISupplierData
{
    public SupplierData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }

    // métodos específicos
}
