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
    public class CategoryBusinessTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsListFromData()
        {
            var mockData = new Mock<ICategoryData>();
            var items = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "C1", Description = "d1" },
                new CategoryDto { Id = 2, Name = "C2", Description = "d2" }
            };
            mockData.Setup(m => m.GetAllAsync()).ReturnsAsync(items);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.Category, CategoryDto>>>();
            var sut = new CategoryBusiness(mockData.Object, mockLogger.Object);

            var res = await sut.GetAllAsync();

            Assert.NotNull(res);
            Assert.Equal(2, ((ICollection<CategoryDto>)res).Count);
            mockData.Verify(m => m.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenFound()
        {
            var mockData = new Mock<ICategoryData>();
            var dto = new CategoryDto { Id = 5, Name = "X", Description = "Y" };
            mockData.Setup(m => m.GetByIdAsync(5)).ReturnsAsync(dto);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.Category, CategoryDto>>>();
            var sut = new CategoryBusiness(mockData.Object, mockLogger.Object);

            var res = await sut.GetByIdAsync(5);

            Assert.Equal(dto.Id, res.Id);
            Assert.Equal(dto.Name, res.Name);
            mockData.Verify(m => m.GetByIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_PropagatesKeyNotFoundException()
        {
            var mockData = new Mock<ICategoryData>();
            mockData.Setup(m => m.GetByIdAsync(99)).ThrowsAsync(new KeyNotFoundException("no"));

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.Category, CategoryDto>>>();
            var sut = new CategoryBusiness(mockData.Object, mockLogger.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetByIdAsync(99));
        }

        [Fact]
        public async Task CreateAsync_CallsDataAndReturnsCreated()
        {
            var mockData = new Mock<ICategoryData>();
            var input = new CategoryDto { Name = "New" };
            var created = new CategoryDto { Id = 10, Name = "New" };
            mockData.Setup(m => m.CreateAsync(input)).ReturnsAsync(created);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.Category, CategoryDto>>>();
            var sut = new CategoryBusiness(mockData.Object, mockLogger.Object);

            var res = await sut.CreateAsync(input);

            Assert.Equal(created.Id, res.Id);
            mockData.Verify(m => m.CreateAsync(input), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsData()
        {
            var mockData = new Mock<ICategoryData>();
            mockData.Setup(m => m.UpdateAsync(3, It.IsAny<CategoryDto>())).Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.Category, CategoryDto>>>();
            var sut = new CategoryBusiness(mockData.Object, mockLogger.Object);

            await sut.UpdateAsync(3, new CategoryDto { Name = "U" });

            mockData.Verify(m => m.UpdateAsync(3, It.IsAny<CategoryDto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteLogicAsync_CallsData()
        {
            var mockData = new Mock<ICategoryData>();
            mockData.Setup(m => m.DeleteLogicAsync(7)).Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<BaseBusiness<Entity.Model.Category, CategoryDto>>>();
            var sut = new CategoryBusiness(mockData.Object, mockLogger.Object);

            await sut.DeleteLogicAsync(7);

            mockData.Verify(m => m.DeleteLogicAsync(7), Times.Once);
        }
    }
}
