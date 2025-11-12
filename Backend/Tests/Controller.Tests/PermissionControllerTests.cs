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
    public class PermissionControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var mock = new Mock<IPermissionBusiness>();
            var list = new List<PermissionDto> { new PermissionDto { TypePermission = "T1" } };
            mock.Setup(m => m.GetAllAsync()).ReturnsAsync(list);

            var controller = new PermissionController(mock.Object);

            var result = await controller.GetAllAsync();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(list, ok.Value);
        }

        [Fact]
        public async Task GetById_WhenNotFound_ReturnsNotFound()
        {
            var mock = new Mock<IPermissionBusiness>();
            mock.Setup(m => m.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException("not"));
            var controller = new PermissionController(mock.Object);

            var result = await controller.GetByIdAsync(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var mock = new Mock<IPermissionBusiness>();
            var input = new PermissionDto { TypePermission = "T1" };
            var created = new PermissionDto { TypePermission = "T1" };
            mock.Setup(m => m.CreateAsync(input)).ReturnsAsync(created);

            var controller = new PermissionController(mock.Object);

            var result = await controller.CreateAsync(input);
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, obj.StatusCode);
            Assert.Equal(created, obj.Value);
        }
    }
}
