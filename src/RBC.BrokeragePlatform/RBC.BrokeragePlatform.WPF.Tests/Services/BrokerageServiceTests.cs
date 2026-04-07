using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.WPF.Interfaces.Services;
using RBC.BrokeragePlatform.WPF.Services;

namespace RBC.BrokeragePlatform.WPF.Tests.Services;

public class BrokerageServiceTests
{
    [Fact]
    public async Task StartPushNotificationsAsync_StartsSignalR_WhenDisconnected()
    {
        // Arrange
        var apiServiceMock = new Mock<IApiService>();
        var signalRServiceMock = new Mock<ISignalRService>();
        signalRServiceMock.SetupGet(service => service.IsConnected).Returns(false);

        var loggerMock = new Mock<ILogger<BrokerageService>>();
        var service = new BrokerageService(apiServiceMock.Object, signalRServiceMock.Object, loggerMock.Object);

        // Act
        await service.StartPushNotificationsAsync();

        // Assert
        signalRServiceMock.Verify(signalR => signalR.StartAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPositionsAsync_StoresPositions_ForDistinctSymbolLookup()
    {
        // Arrange
        var positions = new List<PositionDto>
        {
            new() { PositionId = 1, AccountId = 7, EquityId = 10, Symbol = "AAPL", Quantity = 5, AverageCostPerShare = 100m, CurrentPrice = 110m, CurrentValue = 550m },
            new() { PositionId = 2, AccountId = 7, EquityId = 11, Symbol = "MSFT", Quantity = 2, AverageCostPerShare = 200m, CurrentPrice = 210m, CurrentValue = 420m },
            new() { PositionId = 3, AccountId = 7, EquityId = 12, Symbol = "AAPL", Quantity = 1, AverageCostPerShare = 99m, CurrentPrice = 110m, CurrentValue = 110m }
        };

        var apiServiceMock = new Mock<IApiService>();
        apiServiceMock
            .Setup(service => service.GetAsync("api/positions/7"))
            .ReturnsAsync(JsonConvert.SerializeObject(positions));

        var signalRServiceMock = new Mock<ISignalRService>();
        var loggerMock = new Mock<ILogger<BrokerageService>>();
        var service = new BrokerageService(apiServiceMock.Object, signalRServiceMock.Object, loggerMock.Object);

        // Act
        await service.GetPositionsAsync(7);
        var symbols = service.GetSymbols(7);

        // Assert
        Assert.Equal(new[] { "AAPL", "MSFT" }, symbols);
    }
}
