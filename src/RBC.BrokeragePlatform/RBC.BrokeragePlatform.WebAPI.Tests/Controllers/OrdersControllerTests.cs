using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RBC.BrokeragePlatform.Application.Features.PlaceOrder;
using RBC.BrokeragePlatform.SharedCore.Enums;
using RBC.BrokeragePlatform.WebAPI.Controllers;

namespace RBC.BrokeragePlatform.WebAPI.Tests.Controllers
{
    public class OrdersControllerTests
    {
        [Fact]
        public async Task PlaceOrder_ReturnsOkOnSuccess()
        {
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<OrdersController>>();
            var command = new PlaceOrderCommand { AccountId = 1, Symbol = "AAPL", OrderType = (int)OrderTypeEnum.BUY, Quantity = 10, LimitPrice = 100 };
            mockMediator.Setup(m => m.Send(command, CancellationToken.None));
            var controller = new OrdersController(mockMediator.Object, mockLogger.Object);

            var result = await controller.PlaceOrder(command);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Order placed successfully", okResult.Value);
        }

        [Fact]
        public async Task PlaceOrder_ReturnsInternalServerErrorOnException()
        {
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<OrdersController>>();
            var command = new PlaceOrderCommand { AccountId = 1, Symbol = "AAPL", OrderType = (int)OrderTypeEnum.BUY, Quantity = 10, LimitPrice = 100 };
            mockMediator.Setup(m => m.Send(command, default)).ThrowsAsync(new Exception("fail"));
            var controller = new OrdersController(mockMediator.Object, mockLogger.Object);

            var result = await controller.PlaceOrder(command);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}
