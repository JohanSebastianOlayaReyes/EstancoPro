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
    public class PermissionBusinessTests
    {
        [Fact]
        public async Task GetAll_DelegatesToData_ReturnsList()
        {
            var mockData = new Mock<IPermissionData>();
            var expected = new List<PermissionDto> { new PermissionDto { TypePermission = "T1" } };
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = new Mock<ILogger<BaseBusiness<Entity.Model.Permission, PermissionDto>>>().Object;
            var sut = new PermissionBusiness(mockData.Object, logger as ILogger<BaseBusiness<Entity.Model.Permission, PermissionDto>>);

            var result = await sut.GetAllAsync();

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task Create_DelegatesToData_ReturnsCreated()
        {
            var mockData = new Mock<IPermissionData>();
            var input = new PermissionDto { TypePermission = "T1" };
            var created = new PermissionDto { TypePermission = "T1" };
            mockData.Setup(d => d.CreateAsync(input)).ReturnsAsync(created);

            var logger = new Mock<ILogger<BaseBusiness<Entity.Model.Permission, PermissionDto>>>().Object;
            var sut = new PermissionBusiness(mockData.Object, logger as ILogger<BaseBusiness<Entity.Model.Permission, PermissionDto>>);

            var result = await sut.CreateAsync(input);

            Assert.Equal(created, result);
        }
    }
}
