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
    public class ProductUnitPriceControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var mock = new Mock<IProductUnitPriceBusiness>();
            mock.Setup(m => m.GetAllAsync()).ReturnsAsync(new List<ProductUnitPriceDto> { new ProductUnitPriceDto { UnitPrice = 1m } });

            var sut = new ProductUnitPriceController(mock.Object);

            var res = await sut.GetAll();

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<IProductUnitPriceBusiness>();
            mock.Setup(m => m.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());

            var sut = new ProductUnitPriceController(mock.Object);

            var res = await sut.GetById(999, 999);

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var mock = new Mock<IProductUnitPriceBusiness>();
            var dto = new ProductUnitPriceDto { ProductId = 1, UnitMeasureId = 1, UnitPrice = 9.99m };
            mock.Setup(m => m.CreateAsync(dto)).ReturnsAsync(dto);

            var sut = new ProductUnitPriceController(mock.Object);

            var res = await sut.Create(dto);

            Assert.IsType<ObjectResult>(res);
            var obj = res as ObjectResult;
            Assert.Equal(201, obj.StatusCode);
        }

        [Fact]
        public async Task GetPriceByNames_WhenNull_ReturnsNotFound()
        {
            var mock = new Mock<IProductUnitPriceBusiness>();
            mock.Setup(m => m.GetPriceByNamesAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((ProductUnitPriceDto?)null);

            var sut = new ProductUnitPriceController(mock.Object);

            var res = await sut.GetPriceByNames("x", "y");

            Assert.IsType<NotFoundObjectResult>(res);
        }
    }
}
