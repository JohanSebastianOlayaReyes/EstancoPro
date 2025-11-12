using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Implementations;
using Data.Interfaces;
using Entity.Dto;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Business.Tests
{
    public class SaleBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var expected = new List<SaleDto> { new SaleDto() };
            var mockData = new Mock<ISaleData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<BaseBusiness<Sale, SaleDto>>>();
            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Entity.Context.ApplicationDbContext>()
                .UseInMemoryDatabase(nameof(GetAllAsync_ReturnsList))
                .Options;
            using var context = new Entity.Context.ApplicationDbContext(options);
            var sut = new SaleBusiness(mockData.Object, context, logger);

            var actual = await sut.GetAllAsync();

            Assert.Same(expected, actual);
        }
    }
}
