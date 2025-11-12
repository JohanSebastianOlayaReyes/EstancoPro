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
    public class CashSessionControllerTests
    {
        [Fact]
        public async Task OpenSession_ReturnsCreated_OnSuccess()
        {
            var mock = new Mock<ICashSessionBusiness>();
            var created = new CashSessionDto { Id = 1, OpeningAmount = 100m };
            mock.Setup(m => m.OpenSessionAsync(100m)).ReturnsAsync(created);

            var controller = new CashSessionController(mock.Object);

            var result = await controller.OpenSession(new OpenSessionRequest { OpeningAmount = 100m });

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, obj.StatusCode);
            Assert.Equal(created, obj.Value);
        }

        [Fact]
        public async Task OpenSession_ReturnsBadRequest_WhenNegative()
        {
            var mock = new Mock<ICashSessionBusiness>();
            var controller = new CashSessionController(mock.Object);

            var result = await controller.OpenSession(new OpenSessionRequest { OpeningAmount = -5m });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task OpenSession_ReturnsBadRequest_OnInvalidOperation()
        {
            var mock = new Mock<ICashSessionBusiness>();
            mock.Setup(m => m.OpenSessionAsync(It.IsAny<decimal>())).ThrowsAsync(new InvalidOperationException("already open"));
            var controller = new CashSessionController(mock.Object);

            var result = await controller.OpenSession(new OpenSessionRequest { OpeningAmount = 10m });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CloseSession_ReturnsOk_OnSuccess()
        {
            var mock = new Mock<ICashSessionBusiness>();
            mock.Setup(m => m.CloseSessionAsync(1, 150m)).ReturnsAsync(50m);
            var controller = new CashSessionController(mock.Object);

            var result = await controller.CloseSession(1, new CloseSessionRequest { ClosingAmount = 150m });

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task CloseSession_ReturnsNotFound_WhenMissing()
        {
            var mock = new Mock<ICashSessionBusiness>();
            mock.Setup(m => m.CloseSessionAsync(It.IsAny<int>(), It.IsAny<decimal>())).ThrowsAsync(new KeyNotFoundException("not"));
            var controller = new CashSessionController(mock.Object);

            var result = await controller.CloseSession(5, new CloseSessionRequest { ClosingAmount = 10m });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CloseSession_ReturnsBadRequest_OnInvalidOperation()
        {
            var mock = new Mock<ICashSessionBusiness>();
            mock.Setup(m => m.CloseSessionAsync(It.IsAny<int>(), It.IsAny<decimal>())).ThrowsAsync(new InvalidOperationException("invalid"));
            var controller = new CashSessionController(mock.Object);

            var result = await controller.CloseSession(1, new CloseSessionRequest { ClosingAmount = 10m });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetOpenSession_ReturnsOk_WhenExists()
        {
            var mock = new Mock<ICashSessionBusiness>();
            var session = new CashSessionDto { Id = 2, OpeningAmount = 50m };
            mock.Setup(m => m.GetOpenSessionAsync()).ReturnsAsync(session);
            var controller = new CashSessionController(mock.Object);

            var result = await controller.GetOpenSession();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(session, ok.Value);
        }

        [Fact]
        public async Task GetOpenSession_ReturnsNotFound_WhenNone()
        {
            var mock = new Mock<ICashSessionBusiness>();
            mock.Setup(m => m.GetOpenSessionAsync()).ReturnsAsync((CashSessionDto)null);
            var controller = new CashSessionController(mock.Object);

            var result = await controller.GetOpenSession();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetByDateRange_ReturnsOk()
        {
            var mock = new Mock<ICashSessionBusiness>();
            var list = new List<CashSessionDto> { new CashSessionDto { Id = 1 } };
            var from = DateTime.UtcNow.AddDays(-1);
            var to = DateTime.UtcNow;
            mock.Setup(m => m.GetByDateRangeAsync(from, to)).ReturnsAsync(list);
            var controller = new CashSessionController(mock.Object);

            var result = await controller.GetByDateRange(from, to);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetSessionBalance_ReturnsOk_OnSuccess()
        {
            var mock = new Mock<ICashSessionBusiness>();
            var balance = new CashSessionBalanceDto
            {
                SessionId = 1,
                OpeningAmount = 100m,
                ExpectedAmount = 100m,
                ActualAmount = 110m,
                Difference = 10m,
                Movements = new System.Collections.Generic.List<CashMovementDto>()
            };
            mock.Setup(m => m.GetSessionBalanceAsync(1)).ReturnsAsync(balance);
            var controller = new CashSessionController(mock.Object);

            var result = await controller.GetSessionBalance(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(balance, ok.Value);
        }

        [Fact]
        public async Task GetSessionBalance_ReturnsNotFound_WhenMissing()
        {
            var mock = new Mock<ICashSessionBusiness>();
            mock.Setup(m => m.GetSessionBalanceAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException("not"));
            var controller = new CashSessionController(mock.Object);

            var result = await controller.GetSessionBalance(7);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
