using System.Threading.Tasks;
using Business.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Xunit;

namespace Controller.Tests
{
    public class SaleControllerTests
    {
        [Fact]
        public async Task FinalizeSale_OnSuccess_ReturnsOk()
        {
            var mockBusiness = new Mock<ISaleBusiness>();
            mockBusiness.Setup(b => b.FinalizeSaleAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            var controller = new SaleController(mockBusiness.Object);

            var result = await controller.FinalizeSale(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task FinalizeSale_WhenNotFound_ReturnsNotFound()
        {
            var mockBusiness = new Mock<ISaleBusiness>();
            mockBusiness.Setup(b => b.FinalizeSaleAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException("no"));

            var controller = new SaleController(mockBusiness.Object);

            var result = await controller.FinalizeSale(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
