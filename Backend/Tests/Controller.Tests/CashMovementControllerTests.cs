using System;
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
    public class CashMovementControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var mock = new Mock<ICashMovementBusiness>();
            var expected = new List<CashMovementDto> { new CashMovementDto { Amount = 100m } };
            mock.Setup(m => m.GetAllAsync()).ReturnsAsync(expected);

            var controller = new CashMovementController(mock.Object);

            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);
        }

        [Fact]
        public async Task GetById_WhenNotFound_ReturnsNotFound()
        {
            var mock = new Mock<ICashMovementBusiness>();
            mock.Setup(m => m.GetByIdAsync(It.IsAny<int>(), It.IsAny<DateTime>())).ThrowsAsync(new KeyNotFoundException("not"));

            var controller = new CashMovementController(mock.Object);

            var result = await controller.GetById(1, DateTime.UtcNow);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsOkWithObject()
        {
            var mock = new Mock<ICashMovementBusiness>();
            var at = DateTime.UtcNow;
            var expected = new CashMovementDto { CashSessionId = 1, At = at, Amount = 50m };
            mock.Setup(m => m.GetByIdAsync(1, at)).ReturnsAsync(expected);

            var controller = new CashMovementController(mock.Object);

            var result = await controller.GetById(1, at);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var mock = new Mock<ICashMovementBusiness>();
            var input = new CashMovementDto { CashSessionId = 1, At = DateTime.UtcNow, Amount = 75m };
            var created = new CashMovementDto { CashSessionId = 1, At = input.At, Amount = 75m };
            mock.Setup(m => m.CreateAsync(input)).ReturnsAsync(created);

            var controller = new CashMovementController(mock.Object);

            var result = await controller.Create(input);
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, obj.StatusCode);
            Assert.Equal(created, obj.Value);
        }

        [Fact]
        public async Task Update_WhenNotFound_ReturnsNotFound()
        {
            var mock = new Mock<ICashMovementBusiness>();
            mock.Setup(m => m.UpdateAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CashMovementDto>())).ThrowsAsync(new KeyNotFoundException("not"));

            var controller = new CashMovementController(mock.Object);

            var result = await controller.Update(1, DateTime.UtcNow, new CashMovementDto());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_WhenNotFound_ReturnsNotFound()
        {
            var mock = new Mock<ICashMovementBusiness>();
            mock.Setup(m => m.DeleteAsync(It.IsAny<int>(), It.IsAny<DateTime>())).ThrowsAsync(new KeyNotFoundException("not"));

            var controller = new CashMovementController(mock.Object);

            var result = await controller.Delete(1, DateTime.UtcNow);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
