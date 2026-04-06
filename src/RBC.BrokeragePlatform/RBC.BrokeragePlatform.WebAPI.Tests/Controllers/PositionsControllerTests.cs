using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RBC.BrokeragePlatform.Application.Features.GetAccountPositions;
using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.WebAPI.Controllers;

namespace RBC.BrokeragePlatform.WebAPI.Tests.Controllers
{
    public class PositionsControllerTests
    {
        [Fact]
        public async Task GetPositionsByAccountId_ReturnsOkWithPositions()
        {
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<PositionsController>>();
            var positions = new List<PositionDto> { new PositionDto{

               AccountId = 1,
               EquityId = 1,
               PositionId = 1,
               Symbol = "MA",
               Quantity = 1,
               AverageCostPerShare = 1m,
               CurrentPrice = 1m,
               CurrentValue = 1m
            } };
            mockMediator.Setup(m => m.Send(It.IsAny<GetAccountPositionsQuery>(), default)).ReturnsAsync(positions);
            var controller = new PositionsController(mockMediator.Object, mockLogger.Object);

            var result = await controller.GetPositionsByAccountId(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPositions = Assert.IsAssignableFrom<IEnumerable<PositionDto>>(okResult.Value);
            Assert.Single(returnedPositions);
        }

        [Fact]
        public async Task GetPositionsByAccountId_ReturnsInternalServerErrorOnException()
        {
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<PositionsController>>();
            mockMediator.Setup(m => m.Send(It.IsAny<GetAccountPositionsQuery>(), default)).ThrowsAsync(new Exception("fail"));
            var controller = new PositionsController(mockMediator.Object, mockLogger.Object);

            var result = await controller.GetPositionsByAccountId(1);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}
