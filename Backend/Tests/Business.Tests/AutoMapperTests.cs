using AutoMapper;
using Entity.Model;
using Entity.Dto;
using Utilities.Mapper;
using Xunit;

namespace Business.Tests
{
    public class AutoMapperTests
    {
        private readonly IMapper _mapper;

        public AutoMapperTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Should_Map_Supplier_To_SupplierDto()
        {
            // Arrange
            var supplier = new Supplier
            {
                Name = "Test Supplier",
                Phone = "123456789"
            };

            // Act
            var supplierDto = _mapper.Map<SupplierDto>(supplier);

            // Assert
            Assert.NotNull(supplierDto);
            Assert.Equal(supplier.Name, supplierDto.Name);
            Assert.Equal(supplier.Phone, supplierDto.Phone);
        }

        [Fact]
        public void Should_Map_SupplierDto_To_Supplier()
        {
            // Arrange
            var supplierDto = new SupplierDto
            {
                Name = "Test Supplier",
                Phone = "123456789"
            };

            // Act
            var supplier = _mapper.Map<Supplier>(supplierDto);

            // Assert
            Assert.NotNull(supplier);
            Assert.Equal(supplierDto.Name, supplier.Name);
            Assert.Equal(supplierDto.Phone, supplier.Phone);
        }
    }
}