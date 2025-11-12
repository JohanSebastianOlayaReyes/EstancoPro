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
    public class ProductUnitPriceBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arrange
            var expected = new List<ProductUnitPriceDto>
            {
                new ProductUnitPriceDto { }
            };

            var mockData = new Mock<IProductUnitPriceData>();
            mockData.Setup(d => d.GetAllAsync()).ReturnsAsync(expected);

            var mockContext = new Mock<Entity.Context.ApplicationDbContext>(new object[] { new Microsoft.EntityFrameworkCore.DbContextOptions<Entity.Context.ApplicationDbContext>() });
            var logger = Mock.Of<ILogger<ProductUnitPriceBusiness>>();

            var sut = new ProductUnitPriceBusiness(mockData.Object, mockContext.Object, logger);

            // Act
            var actual = await sut.GetAllAsync();

            // Assert
            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDataThrows_Propagates()
        {
            // Arrange
            var mockData = new Mock<IProductUnitPriceData>();
            var ex = new InvalidOperationException("fail");
            mockData.Setup(d => d.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(ex);

            var mockContext = new Mock<Entity.Context.ApplicationDbContext>(new object[] { new Microsoft.EntityFrameworkCore.DbContextOptions<Entity.Context.ApplicationDbContext>() });
            var logger = Mock.Of<ILogger<ProductUnitPriceBusiness>>();
            var sut = new ProductUnitPriceBusiness(mockData.Object, mockContext.Object, logger);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetByIdAsync(1,2));
        }

        [Fact]
        public async Task UpdateAsync_WithNonPositivePrice_ThrowsArgumentException()
        {
            // Arrange
            var dto = new ProductUnitPriceDto { UnitPrice = 0 };
            var mockData = new Mock<IProductUnitPriceData>();
            var mockContext = new Mock<Entity.Context.ApplicationDbContext>(new object[] { new Microsoft.EntityFrameworkCore.DbContextOptions<Entity.Context.ApplicationDbContext>() });
            var logger = Mock.Of<ILogger<ProductUnitPriceBusiness>>();
            var sut = new ProductUnitPriceBusiness(mockData.Object, mockContext.Object, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => sut.UpdateAsync(1,1,dto));
        }

        [Fact]
        public async Task DeleteAsync_CallsData()
        {
            // Arrange
            var mockData = new Mock<IProductUnitPriceData>();
            mockData.Setup(d => d.DeleteAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var mockContext = new Mock<Entity.Context.ApplicationDbContext>(new object[] { new Microsoft.EntityFrameworkCore.DbContextOptions<Entity.Context.ApplicationDbContext>() });
            var logger = Mock.Of<ILogger<ProductUnitPriceBusiness>>();
            var sut = new ProductUnitPriceBusiness(mockData.Object, mockContext.Object, logger);

            // Act
            await sut.DeleteAsync(1,2);

            // Assert
            mockData.Verify(d => d.DeleteAsync(1,2), Times.Once);
        }
    }
}
