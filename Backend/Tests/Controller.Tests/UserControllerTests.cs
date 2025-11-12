using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using System.Collections.Generic;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Xunit;

namespace Controller.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var mockBusiness = new Mock<IUserBusiness>();
            var expected = new List<UserDto> { new UserDto { Email = "a@a" } };
            mockBusiness.Setup(b => b.GetAllAsync()).ReturnsAsync((IEnumerable<UserDto>)expected);

            var controller = new UserController(mockBusiness.Object);

            var result = await controller.GetAllAsync();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);
        }
    }
}
