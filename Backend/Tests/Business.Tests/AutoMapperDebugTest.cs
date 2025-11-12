using AutoMapper;
using Entity.Model;
using Entity.Dto;
using Utilities.Mapper;
using System;
using Xunit;

namespace Business.Tests
{
    public class AutoMapperDebugTest
    {
        [Fact]
        public void TestSupplierMapping()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            var mapper = config.CreateMapper();

            // Test mapping from Supplier to SupplierDto
            var supplier = new Supplier
            {
                Name = "Test Supplier",
                Phone = "123456789"
            };

            var supplierDto = mapper.Map<SupplierDto>(supplier);
            Assert.NotNull(supplierDto);
            Assert.Equal(supplier.Name, supplierDto.Name);
            Assert.Equal(supplier.Phone, supplierDto.Phone);

            // Test mapping from SupplierDto to Supplier
            var dto = new SupplierDto
            {
                Name = "Test Supplier",
                Phone = "123456789"
            };

            var entity = mapper.Map<Supplier>(dto);
            Assert.NotNull(entity);
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Phone, entity.Phone);
        }
    }
}