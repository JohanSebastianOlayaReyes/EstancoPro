using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Implementations;
using Data.Interfaces;
using Entity.Model;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Business.Tests
{
    public class RolFormPermissionBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var expected = new List<RolFormPermissionDto> { new RolFormPermissionDto() };
            var mockData = new Mock<IRolFormPermissionData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<BaseBusiness<RolFormPermission, RolFormPermissionDto>>>();
            var sut = new RolFormPermissionBusiness(mockData.Object, logger);

            var actual = await sut.GetAllAsync();

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Create_DelegatesToData()
        {
            var mock = new Mock<IRolFormPermissionData>();
            var dto = new RolFormPermissionDto { RolId = 1, FormId = 1, PermissionId = 1 };
            mock.Setup(m => m.CreateAsync(dto)).ReturnsAsync(dto);

            var loggerMock = new Mock<ILogger<BaseBusiness<RolFormPermission, RolFormPermissionDto>>>();
            var sut = new RolFormPermissionBusiness(mock.Object, loggerMock.Object);

            var res = await sut.CreateAsync(dto);

            mock.Verify(m => m.CreateAsync(dto), Times.Once);
        }
    }
}
