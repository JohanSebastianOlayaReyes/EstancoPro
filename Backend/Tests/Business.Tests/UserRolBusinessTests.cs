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
    public class UserRolBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var expected = new List<UserRolDto> { new UserRolDto() };
            var mockData = new Mock<IRolUserData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<BaseBusiness<UserRol, UserRolDto>>>();
            var sut = new UserRolBusiness(mockData.Object, logger);

            var actual = await sut.GetAllAsync();

            Assert.Same(expected, actual);
        }
    }
}
