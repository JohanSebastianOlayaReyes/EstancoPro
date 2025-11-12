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
    public class RolControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var mockBusiness = new Mock<IRolBusiness>();
            var expected = new List<RolDto> { new RolDto { TypeRol = "Admin" } };
            mockBusiness.Setup(b => b.GetAllAsync()).ReturnsAsync(expected);

            var controller = new RolController(mockBusiness.Object);

            var result = await controller.GetAllAsync();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);
        }

        [Fact]
        public async Task GetById_WhenNotFound_ReturnsNotFound()
        {
            var mockBusiness = new Mock<IRolBusiness>();
            mockBusiness.Setup(b => b.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException("not"));

            var controller = new RolController(mockBusiness.Object);

            var result = await controller.GetByIdAsync(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var mockBusiness = new Mock<IRolBusiness>();
            var input = new RolDto { TypeRol = "new" };
            var created = new RolDto { Id = 9, TypeRol = "new" };
            mockBusiness.Setup(b => b.CreateAsync(input)).ReturnsAsync(created);

            var controller = new RolController(mockBusiness.Object);

            var result = await controller.CreateAsync(input);
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, obj.StatusCode);
            Assert.Equal(created, obj.Value);
        }
    }
}
