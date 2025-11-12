namespace Business.Implementations;

using Business.Implementations.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

public class FormBusiness : BaseBusiness<Form, FormDto>, IFormBusiness
{
    private readonly IFormData _formData;

    public FormBusiness(IFormData formData, ILogger<BaseBusiness<Form, FormDto>> logger)
        : base(formData, logger)
    {
        _formData = formData;
    }
}
