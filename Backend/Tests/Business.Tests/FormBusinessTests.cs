using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Business.Implementations;
using Data.Interfaces;
using Entity.Dto;

namespace Business.Tests
{
    public class FormBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_DelegatesToData()
        {
            var mockData = new Mock<IFormData>();
            var expected = new List<FormDto> { new FormDto { Name = "f1" } };
            mockData.Setup(m => m.GetAllAsync()).ReturnsAsync(expected);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.Form, FormDto>>>();
            var sut = new FormBusiness(mockData.Object, mockLogger.Object);

            var res = await sut.GetAllAsync();

            Assert.Same(expected, res);
            mockData.Verify(m => m.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_CallsDataAndReturnsCreated()
        {
            var mockData = new Mock<IFormData>();
            var input = new FormDto { Name = "New" };
            var created = new FormDto { Id = 11, Name = "New" };
            mockData.Setup(m => m.CreateAsync(input)).ReturnsAsync(created);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.Form, FormDto>>>();
            var sut = new FormBusiness(mockData.Object, mockLogger.Object);

            var res = await sut.CreateAsync(input);

            Assert.Equal(created.Id, res.Id);
            mockData.Verify(m => m.CreateAsync(input), Times.Once);
        }
    }
}
