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
    public class ModuleBusinessTests
    {
        [Fact]
        public async Task GetAll_DelegatesToData_ReturnsList()
        {
            var mockData = new Mock<IModuleData>();
            var expected = new List<ModuleDto> { new ModuleDto { Name = "M1" } };
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = new Mock<ILogger<BaseBusiness<Entity.Model.Module, ModuleDto>>>().Object;
            var sut = new ModuleBusiness(mockData.Object, logger as ILogger<BaseBusiness<Entity.Model.Module, ModuleDto>>);

            var result = await sut.GetAllAsync();

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task Create_DelegatesToData_ReturnsCreated()
        {
            var mockData = new Mock<IModuleData>();
            var input = new ModuleDto { Name = "M1" };
            var created = new ModuleDto { Name = "M1" };
            mockData.Setup(d => d.CreateAsync(input)).ReturnsAsync(created);

            var logger = new Mock<ILogger<BaseBusiness<Entity.Model.Module, ModuleDto>>>().Object;
            var sut = new ModuleBusiness(mockData.Object, logger as ILogger<BaseBusiness<Entity.Model.Module, ModuleDto>>);

            var result = await sut.CreateAsync(input);

            Assert.Equal(created, result);
        }
    }
}
