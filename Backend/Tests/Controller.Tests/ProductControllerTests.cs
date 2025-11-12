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
    public class ProductControllerTests
    {
        [Fact]
        public async Task GetLowStockProducts_ReturnsOk()
        {
            var mockBusiness = new Mock<IProductBusiness>();
            var expected = new List<ProductDto> { new ProductDto { Name = "p1" } };
            mockBusiness.Setup(b => b.GetLowStockProductsAsync()).ReturnsAsync(expected);

            var controller = new ProductController(mockBusiness.Object);

            var result = await controller.GetLowStockProducts();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);
        }

        [Fact]
        public async Task GetByCategoryName_WhenBusinessThrowsArgumentException_ReturnsBadRequest()
        {
            var mockBusiness = new Mock<IProductBusiness>();
            mockBusiness.Setup(b => b.GetByCategoryNameAsync(It.IsAny<string>())).ThrowsAsync(new System.ArgumentException("bad"));

            var controller = new ProductController(mockBusiness.Object);

            var result = await controller.GetByCategoryName("x");

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
