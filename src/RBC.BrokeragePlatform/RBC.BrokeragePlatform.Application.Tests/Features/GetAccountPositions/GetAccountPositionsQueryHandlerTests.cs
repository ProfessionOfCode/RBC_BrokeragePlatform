using Microsoft.Extensions.Logging;
using Moq;
using RBC.BrokeragePlatform.Application.Features.GetAccountPositions;
using RBC.BrokeragePlatform.Application.Interfaces.Mapping;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Domain.Entities;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.Application.Tests.Features.GetAccountPositions
{
    public class GetAccountPositionsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsMappedPositions_WhenRepositoryReturnsPositions()
        {
            var mockPositionRepo = new Mock<IPositionRepository>();
            var mockEquityRepo = new Mock<IEquityRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<GetAccountPositionsQueryHandler>>();
            var positions = new List<Position> { new() };
            var equities = new List<Equity> { new() };
            var dtos = new List<PositionDto> { new PositionDto{
                
               AccountId = 1,
               EquityId = 1, 
               PositionId = 1, 
               Symbol = "MA", 
               Quantity = 1, 
               AverageCostPerShare = 1m, 
               CurrentPrice = 1m, 
               CurrentValue = 1m 
            } };
            mockPositionRepo.Setup(r => r.GetPositionsByAccountIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);
            mockEquityRepo.Setup(r => r.GetEquitiesByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(equities);
            mockMapper.Setup(m => m.MapCollection<Position, PositionDto>(positions)).Returns(dtos);
            var handler = new GetAccountPositionsQueryHandler(mockPositionRepo.Object, mockEquityRepo.Object, mockMapper.Object, mockLogger.Object);
            var result = await handler.Handle(new GetAccountPositionsQuery(1), CancellationToken.None);
            Assert.Single(result);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenRepositoryThrows()
        {
            var mockPositionRepo = new Mock<IPositionRepository>();
            var mockEquityRepo = new Mock<IEquityRepository>();
            var mockMapper = new Mock<IMapper>();
            var equities = new List<Equity> { new() };
            var mockLogger = new Mock<ILogger<GetAccountPositionsQueryHandler>>();
            mockPositionRepo.Setup(r => r.GetPositionsByAccountIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception());
            mockEquityRepo.Setup(r => r.GetEquitiesByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(equities);
            var handler = new GetAccountPositionsQueryHandler(mockPositionRepo.Object, mockEquityRepo.Object, mockMapper.Object, mockLogger.Object);
            var result = await handler.Handle(new GetAccountPositionsQuery(1), CancellationToken.None);
            Assert.Empty(result);
        }
    }
}
