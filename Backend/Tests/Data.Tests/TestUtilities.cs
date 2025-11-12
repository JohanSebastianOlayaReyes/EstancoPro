using AutoMapper;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Tests
{
    public static class TestUtilities
    {
        public static ApplicationDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Map basic entities used in tests
                cfg.CreateMap<Entity.Model.Product, Entity.Dto.ProductDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.ProductUnitPrice, Entity.Dto.ProductUnitPriceDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.CashMovement, Entity.Dto.CashMovementDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.User, Entity.Model.UserDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.PurchaseProductDetail, Entity.Dto.PurchaseProductDetailDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Form, Entity.Dto.FormDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Person, Entity.Dto.PersonDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Module, Entity.Dto.ModuleDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Permission, Entity.Dto.PermissionDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.FormModule, Entity.Dto.FormModuleDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Rol, Entity.Dto.RolDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.RolFormPermission, Entity.Model.RolFormPermissionDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Category, Entity.Dto.CategoryDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.UnitMeasure, Entity.Dto.UnitMeasureDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Supplier, Entity.Dto.SupplierDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Sale, Entity.Dto.SaleDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.SaleProductDetail, Entity.Dto.SaleProductDetailDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.CashSession, Entity.Dto.CashSessionDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Purchase, Entity.Dto.PurchaseDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Supplier, Entity.Dto.SupplierDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.UserRol, Entity.Model.UserRolDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.User, Entity.Model.UserDto>().ReverseMap();
                cfg.CreateMap<Entity.Model.Rol, Entity.Dto.RolDto>().ReverseMap();
            });

            return config.CreateMapper();
        }
    }
}
