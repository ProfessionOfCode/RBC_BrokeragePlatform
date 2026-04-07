using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RBC.BrokeragePlatform.WPF.Services;

namespace RBC.BrokeragePlatform.WPF.Tests.Services;

public class SignalRServiceTests
{
    [Fact]
    public void IsConnected_ReturnsFalse_WhenConnectionIsNotInitialized()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<SignalRService>>();
        var service = new SignalRService(configurationMock.Object, loggerMock.Object);

        // Act
        var result = service.IsConnected;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SubscribeToPositionUpdateGroup_Completes_WhenConnectionIsNotInitialized()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<SignalRService>>();
        var service = new SignalRService(configurationMock.Object, loggerMock.Object);

        // Act
        var exception = await Record.ExceptionAsync(() => service.SubscribeToPositionUpdateGroup(42));

        // Assert
        Assert.Null(exception);
        Assert.False(service.IsConnected);
    }
}
