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
    public class SaleProductDetailControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var mock = new Mock<ISaleProductDetailBusiness>();
            mock.Setup(m => m.GetAllAsync()).ReturnsAsync(new List<SaleProductDetailDto> { new SaleProductDetailDto() });

            var sut = new SaleProductDetailController(mock.Object);

            var res = await sut.GetAll();

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var mock = new Mock<ISaleProductDetailBusiness>();
            mock.Setup(m => m.GetByIdAsync(1,1,1)).ThrowsAsync(new KeyNotFoundException());

            var sut = new SaleProductDetailController(mock.Object);

            var res = await sut.GetById(1,1,1);

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var mock = new Mock<ISaleProductDetailBusiness>();
            var dto = new SaleProductDetailDto { SaleId = 1, ProductId = 1, Quantity = 1, UnitPrice = 10 };
            mock.Setup(m => m.CreateAsync(dto)).ReturnsAsync(dto);

            var sut = new SaleProductDetailController(mock.Object);

            var res = await sut.Create(dto);

            Assert.IsType<ObjectResult>(res);
            var obj = res as ObjectResult;
            Assert.Equal(201, obj.StatusCode);
        }
    }
}
