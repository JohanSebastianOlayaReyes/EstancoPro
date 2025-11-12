using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Business.Implementations;
using Data.Interfaces;
using Entity.Dto;

namespace Business.Tests
{
    public class PurchaseProductDetailBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsListFromData()
        {
            var mockData = new Mock<IPurchaseProductDetailData>();
            var items = new List<PurchaseProductDetailDto>
            {
                new PurchaseProductDetailDto { Id = 1, Quantity = 1 },
                new PurchaseProductDetailDto { Id = 2, Quantity = 2 }
            };
            mockData.Setup(m => m.GetAllAsync()).ReturnsAsync(items);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.PurchaseProductDetail, PurchaseProductDetailDto>>>();
            var sut = new PurchaseProductDetailBusiness(mockData.Object, mockLogger.Object);

            var res = await sut.GetAllAsync();

            Assert.NotNull(res);
            Assert.Equal(2, ((ICollection<PurchaseProductDetailDto>)res).Count);
            mockData.Verify(m => m.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_PropagatesKeyNotFoundException()
        {
            var mockData = new Mock<IPurchaseProductDetailData>();
            mockData.Setup(m => m.GetByIdAsync(99)).ThrowsAsync(new KeyNotFoundException("no"));

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.PurchaseProductDetail, PurchaseProductDetailDto>>>();
            var sut = new PurchaseProductDetailBusiness(mockData.Object, mockLogger.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetByIdAsync(99));
        }

        [Fact]
        public async Task CreateAsync_CallsDataAndReturnsCreated()
        {
            var mockData = new Mock<IPurchaseProductDetailData>();
            var input = new PurchaseProductDetailDto { Quantity = 5 };
            var created = new PurchaseProductDetailDto { Id = 10, Quantity = 5 };
            mockData.Setup(m => m.CreateAsync(input)).ReturnsAsync(created);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.PurchaseProductDetail, PurchaseProductDetailDto>>>();
            var sut = new PurchaseProductDetailBusiness(mockData.Object, mockLogger.Object);

            var res = await sut.CreateAsync(input);

            Assert.Equal(created.Id, res.Id);
            mockData.Verify(m => m.CreateAsync(input), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsData()
        {
            var mockData = new Mock<IPurchaseProductDetailData>();
            mockData.Setup(m => m.UpdateAsync(3, It.IsAny<PurchaseProductDetailDto>())).Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.PurchaseProductDetail, PurchaseProductDetailDto>>>();
            var sut = new PurchaseProductDetailBusiness(mockData.Object, mockLogger.Object);

            await sut.UpdateAsync(3, new PurchaseProductDetailDto { Quantity = 7 });

            mockData.Verify(m => m.UpdateAsync(3, It.IsAny<PurchaseProductDetailDto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteLogicAsync_CallsData()
        {
            var mockData = new Mock<IPurchaseProductDetailData>();
            mockData.Setup(m => m.DeleteLogicAsync(7)).Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.PurchaseProductDetail, PurchaseProductDetailDto>>>();
            var sut = new PurchaseProductDetailBusiness(mockData.Object, mockLogger.Object);

            await sut.DeleteLogicAsync(7);

            mockData.Verify(m => m.DeleteLogicAsync(7), Times.Once);
        }
    }
}
