using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Xunit;

namespace Controller.Tests
{
    public class FormControllerTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsOkWithList()
        {
            var mockBusiness = new Mock<IFormBusiness>();
            var expected = new List<FormDto> { new FormDto { Name = "f1" } };
            mockBusiness.Setup(b => b.GetAllAsync()).ReturnsAsync(expected);

            var controller = new FormController(mockBusiness.Object);

            var result = await controller.GetAllAsync();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedObject()
        {
            var mockBusiness = new Mock<IFormBusiness>();
            var input = new FormDto { Name = "new" };
            var created = new FormDto { Id = 3, Name = "new" };
            mockBusiness.Setup(b => b.CreateAsync(input)).ReturnsAsync(created);

            var controller = new FormController(mockBusiness.Object);

            var result = await controller.CreateAsync(input);
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, obj.StatusCode);
            Assert.Equal(created, obj.Value);
        }
    }
}
