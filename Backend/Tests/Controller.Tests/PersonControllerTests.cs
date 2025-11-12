using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Business.Interfaces;
using Entity.Dto;
using Xunit;

namespace Controller.Tests
{
    public class PersonControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var mock = new Mock<IPersonBusiness>();
            mock.Setup(m => m.GetAllAsync()).ReturnsAsync(new List<PersonDto> { new PersonDto { FullName = "X X", FirstName = "X" } });

            var sut = new PersonController(mock.Object);

            var res = await sut.GetAllAsync();

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<IPersonBusiness>();
            mock.Setup(m => m.GetByIdAsync(999)).ThrowsAsync(new KeyNotFoundException());

            var sut = new PersonController(mock.Object);

            var res = await sut.GetByIdAsync(999);

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var mock = new Mock<IPersonBusiness>();
            var dto = new PersonDto { FullName = "New One", FirstName = "New", FirstLastName = "One", PhoneNumber = 1, NumberIdentification = 2 };
            mock.Setup(m => m.CreateAsync(dto)).ReturnsAsync(dto);

            var sut = new PersonController(mock.Object);

            var res = await sut.CreateAsync(dto);

            Assert.IsType<ObjectResult>(res);
            var obj = res as ObjectResult;
            Assert.Equal(201, obj.StatusCode);
        }
    }
}
