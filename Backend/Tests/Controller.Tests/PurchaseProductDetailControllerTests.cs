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
    public class PurchaseProductDetailControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var mockBusiness = new Mock<IPurchaseProductDetailBusiness>();
            var expected = new List<PurchaseProductDetailDto> { new PurchaseProductDetailDto { Quantity = 3 } };
            mockBusiness.Setup(b => b.GetAllAsync()).ReturnsAsync(expected);

            var controller = new PurchaseProductDetailController(mockBusiness.Object);

            var result = await controller.GetAllAsync();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);
        }

        [Fact]
        public async Task GetById_WhenNotFound_ReturnsNotFound()
        {
            var mockBusiness = new Mock<IPurchaseProductDetailBusiness>();
            mockBusiness.Setup(b => b.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException("not"));

            var controller = new PurchaseProductDetailController(mockBusiness.Object);

            var result = await controller.GetByIdAsync(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var mockBusiness = new Mock<IPurchaseProductDetailBusiness>();
            var input = new PurchaseProductDetailDto { Quantity = 2 };
            var created = new PurchaseProductDetailDto { Id = 5, Quantity = 2 };
            mockBusiness.Setup(b => b.CreateAsync(input)).ReturnsAsync(created);

            var controller = new PurchaseProductDetailController(mockBusiness.Object);

            var result = await controller.CreateAsync(input);
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, obj.StatusCode);
            Assert.Equal(created, obj.Value);
        }
    }
}
