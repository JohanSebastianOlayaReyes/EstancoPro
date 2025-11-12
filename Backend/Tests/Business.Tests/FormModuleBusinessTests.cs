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
    public class FormModuleBusinessTests
    {
        [Fact]
        public async Task GetAll_DelegatesToData_ReturnsList()
        {
            var mockData = new Mock<IFormModuleData>();
            var expected = new List<FormModuleDto> { new FormModuleDto { FormId = 1, ModuleId = 2 } };
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = new Mock<ILogger<BaseBusiness<Entity.Model.FormModule, FormModuleDto>>>().Object;
            var sut = new FormModuleBusiness(mockData.Object, logger as ILogger<BaseBusiness<Entity.Model.FormModule, FormModuleDto>>);

            var result = await sut.GetAllAsync();

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task Create_DelegatesToData_ReturnsCreated()
        {
            var mockData = new Mock<IFormModuleData>();
            var input = new FormModuleDto { FormId = 1, ModuleId = 2 };
            var created = new FormModuleDto { FormId = 1, ModuleId = 2 };
            mockData.Setup(d => d.CreateAsync(input)).ReturnsAsync(created);

            var logger = new Mock<ILogger<BaseBusiness<Entity.Model.FormModule, FormModuleDto>>>().Object;
            var sut = new FormModuleBusiness(mockData.Object, logger as ILogger<BaseBusiness<Entity.Model.FormModule, FormModuleDto>>);

            var result = await sut.CreateAsync(input);

            Assert.Equal(created, result);
        }
    }
}
