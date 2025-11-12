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
    public class RolBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var expected = new List<RolDto> { new RolDto() };
            var mockData = new Mock<IRolData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<BaseBusiness<Rol, RolDto>>>();
            var sut = new RolBusiness(mockData.Object, logger);

            var actual = await sut.GetAllAsync();

            Assert.Same(expected, actual);
        }
    }
}
