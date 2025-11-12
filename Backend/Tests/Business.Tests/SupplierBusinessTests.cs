using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Implementations;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Business.Tests
{
    public class SupplierBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_DelegatesToData()
        {
            var expected = new List<SupplierDto> { new SupplierDto { Name = "S1", Phone = "p" } };
            var mockData = new Mock<ISupplierData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<BaseBusiness<Supplier, SupplierDto>>>();
            var sut = new SupplierBusiness(mockData.Object, logger);

            var actual = await sut.GetAllAsync();

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var expected = new List<SupplierDto> { new SupplierDto() };
            var mockData = new Mock<ISupplierData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<BaseBusiness<Supplier, SupplierDto>>>();
            var sut = new SupplierBusiness(mockData.Object, logger);

            var actual = await sut.GetAllAsync();

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Create_DelegatesToData()
        {
            var mock = new Mock<ISupplierData>();
            var dto = new SupplierDto { Name = "S", Phone = "1" };
            mock.Setup(m => m.CreateAsync(dto)).ReturnsAsync(dto);

            var loggerMock = new Mock<ILogger<BaseBusiness<Supplier, SupplierDto>>>();
            var sut = new SupplierBusiness(mock.Object, loggerMock.Object);

            var res = await sut.CreateAsync(dto);

            mock.Verify(m => m.CreateAsync(dto), Times.Once);
        }
    }
}
