using System;
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
    public class CashMovementBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var expected = new List<CashMovementDto> { new CashMovementDto { Amount = 10m, CashSessionId = 1, At = DateTime.UtcNow } };
            var mockData = new Mock<ICashMovementData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<CashMovementBusiness>>();
            var sut = new CashMovementBusiness(mockData.Object, logger);

            var actual = await sut.GetAllAsync();

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task GetByIdAsync_DelegatesToData()
        {
            var at = DateTime.UtcNow;
            var expected = new CashMovementDto { Amount = 10m, CashSessionId = 1, At = at };
            var mockData = new Mock<ICashMovementData>();
            mockData.Setup(d => d.GetByIdAsync(1, at)).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<CashMovementBusiness>>();
            var sut = new CashMovementBusiness(mockData.Object, logger);

            var actual = await sut.GetByIdAsync(1, at);

            Assert.Equal(expected.Amount, actual.Amount);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDataThrows_PropagatesException()
        {
            var mockData = new Mock<ICashMovementData>();
            var ex = new InvalidOperationException("fail");
            mockData.Setup(d => d.GetByIdAsync(It.IsAny<int>(), It.IsAny<DateTime>())).ThrowsAsync(ex);

            var logger = Mock.Of<ILogger<CashMovementBusiness>>();
            var sut = new CashMovementBusiness(mockData.Object, logger);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetByIdAsync(1, DateTime.UtcNow));
        }

        [Fact]
        public async Task CreateAsync_CallsDataAndReturnsCreated()
        {
            var input = new CashMovementDto { Amount = 5m, CashSessionId = 2, At = DateTime.UtcNow };
            var expected = new CashMovementDto { Amount = 5m, CashSessionId = 2, At = input.At };

            var mockData = new Mock<ICashMovementData>();
            mockData.Setup(d => d.CreateAsync(input)).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<CashMovementBusiness>>();
            var sut = new CashMovementBusiness(mockData.Object, logger);

            var actual = await sut.CreateAsync(input);

            Assert.Same(expected, actual);
            mockData.Verify(d => d.CreateAsync(input), Times.Once);
        }
    }
}
