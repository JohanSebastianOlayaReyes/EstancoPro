using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Business.Interfaces;
using Entity.Model;
using Xunit;

namespace Controller.Tests
{
    public class RolFormPermissionControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var mock = new Mock<IRolFormPermissionBusiness>();
            mock.Setup(m => m.GetAllAsync()).ReturnsAsync(new List<RolFormPermissionDto> { new RolFormPermissionDto() });

            var sut = new RolFormPermissionController(mock.Object);

            var res = await sut.GetAllAsync();

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<IRolFormPermissionBusiness>();
            mock.Setup(m => m.GetByIdAsync(999)).ThrowsAsync(new KeyNotFoundException());

            var sut = new RolFormPermissionController(mock.Object);

            var res = await sut.GetByIdAsync(999);

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var mock = new Mock<IRolFormPermissionBusiness>();
            var dto = new RolFormPermissionDto { RolId = 1, FormId = 1, PermissionId = 1 };
            mock.Setup(m => m.CreateAsync(dto)).ReturnsAsync(dto);

            var sut = new RolFormPermissionController(mock.Object);

            var res = await sut.CreateAsync(dto);

            Assert.IsType<ObjectResult>(res);
            var obj = res as ObjectResult;
            Assert.Equal(201, obj.StatusCode);
        }
    }
}
