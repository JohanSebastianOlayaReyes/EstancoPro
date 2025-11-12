using AutoMapper;
using Entity.Dto;
using Entity.Model;

namespace Utilities.Mapper
{
    /// <summary>
    /// Configuración centralizada de AutoMapper para mapear entre entidades y DTOs
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapeos bidireccionales entre entidades y DTOs
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Person, PersonDto>().ReverseMap();
            CreateMap<Rol, RolDto>().ReverseMap();
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<Form, FormDto>().ReverseMap();
            CreateMap<Module, ModuleDto>().ReverseMap();
            CreateMap<FormModule, FormModuleDto>().ReverseMap();
            CreateMap<UserRol, UserRolDto>().ReverseMap();
            CreateMap<RolFormPermission, RolFormPermissionDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();

            // Mapeos adicionales si es necesario
            // CreateMap<Source, Destination>()
            //     .ForMember(dest => dest.Property, opt => opt.MapFrom(src => src.Property));
        }
    }
}
