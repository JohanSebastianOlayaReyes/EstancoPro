using System;
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
    public class PurchaseControllerTests
    {
        [Fact]
        public async Task ReceivePurchase_Succeeds_ReturnsOk()
        {
            var mockBusiness = new Mock<IPurchaseBusiness>();
            mockBusiness.Setup(b => b.ReceivePurchaseAsync(1, true, 1)).Returns(Task.CompletedTask);

            var controller = new PurchaseController(mockBusiness.Object);

            var req = new ReceivePurchaseRequest { PayInCash = true, CashSessionId = 1 };
            var result = await controller.ReceivePurchase(1, req);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Compra recibida", ok.Value.ToString());
        }

        [Fact]
        public async Task ReceivePurchase_KeyNotFound_ReturnsNotFound()
        {
            var mockBusiness = new Mock<IPurchaseBusiness>();
            mockBusiness.Setup(b => b.ReceivePurchaseAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int?>()))
                .ThrowsAsync(new KeyNotFoundException("no"));

            var controller = new PurchaseController(mockBusiness.Object);

            var req = new ReceivePurchaseRequest { PayInCash = false };
            var result = await controller.ReceivePurchase(99, req);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ReceivePurchase_InvalidOperation_ReturnsBadRequest()
        {
            var mockBusiness = new Mock<IPurchaseBusiness>();
            mockBusiness.Setup(b => b.ReceivePurchaseAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int?>()))
                .ThrowsAsync(new InvalidOperationException("bad"));

            var controller = new PurchaseController(mockBusiness.Object);

            var req = new ReceivePurchaseRequest { PayInCash = false };
            var result = await controller.ReceivePurchase(2, req);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CancelPurchase_EmptyReason_ReturnsBadRequest()
        {
            var mockBusiness = new Mock<IPurchaseBusiness>();
            var controller = new PurchaseController(mockBusiness.Object);

            var req = new CancelPurchaseRequest { Reason = "" };
            var result = await controller.CancelPurchase(1, req);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetBySupplierName_ReturnsOk()
        {
            var mockBusiness = new Mock<IPurchaseBusiness>();
            var expected = new List<PurchaseDto> { new PurchaseDto { Id = 1 } };
            mockBusiness.Setup(b => b.GetBySupplierNameAsync(It.IsAny<string>())).ReturnsAsync(expected);

            var controller = new PurchaseController(mockBusiness.Object);

            var result = await controller.GetBySupplierName("supplier");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);
        }
    }
}
