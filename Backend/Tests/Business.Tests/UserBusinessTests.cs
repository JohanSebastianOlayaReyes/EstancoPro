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
    public class UserBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var expected = new List<UserDto> { new UserDto() };
            var mockData = new Mock<IUserData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<BaseBusiness<User, UserDto>>>();
            var sut = new UserBusiness(mockData.Object, logger);

            var actual = await sut.GetAllAsync();

            Assert.Same(expected, actual);
        }
    }
}
