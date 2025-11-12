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
    public class SaleProductDetailBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arrange
            var expected = new List<SaleProductDetailDto>
            {
                new SaleProductDetailDto { }
            };

            var mockData = new Mock<ISaleProductDetailData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<SaleProductDetailBusiness>>();
            var sut = new SaleProductDetailBusiness(mockData.Object, logger);

            // Act
            var actual = await sut.GetAllAsync();

            // Assert
            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDataThrows_Propagates()
        {
            // Arrange
            var mockData = new Mock<ISaleProductDetailData>();
            var ex = new InvalidOperationException("fail");
            mockData.Setup(d => d.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(ex);

            var logger = Mock.Of<ILogger<SaleProductDetailBusiness>>();
            var sut = new SaleProductDetailBusiness(mockData.Object, logger);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetByIdAsync(1,2,3));
        }

        [Fact]
        public async Task CreateAsync_CallsDataAndReturnsCreated()
        {
            // Arrange
            var dto = new SaleProductDetailDto { };
            var expected = new SaleProductDetailDto { };

            var mockData = new Mock<ISaleProductDetailData>();
            mockData.Setup(d => d.CreateAsync(dto)).ReturnsAsync(expected);

            var logger = Mock.Of<ILogger<SaleProductDetailBusiness>>();
            var sut = new SaleProductDetailBusiness(mockData.Object, logger);

            // Act
            var actual = await sut.CreateAsync(dto);

            // Assert
            Assert.Same(expected, actual);
            mockData.Verify(d => d.CreateAsync(dto), Times.Once);
        }
    }
}
