using AutoMapper;
using Data.Implementations.Base;
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;

namespace Data.Implementations;

public class FormData : BaseData<Form, FormDto>, IFormData
{
    public FormData(ApplicationDbContext context, IMapper mapper) : base(context, mapper) { }
}