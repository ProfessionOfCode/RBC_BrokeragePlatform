using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using RBC.BrokeragePlatform.WPF.Interfaces.Services;
using RBC.BrokeragePlatform.WPF.Services;
using System.Net;

namespace RBC.BrokeragePlatform.WPF.Tests.Services;

public class ApiServiceTests
{
    [Fact]
    public async Task GetAsync_ReturnsResponseContent_WhenRequestSucceeds()
    {
        // Arrange
        var responseContent = "[{\"accountId\":1}]";
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(request =>
                    request.Method == HttpMethod.Get &&
                    request.RequestUri == new Uri("http://localhost/api/accounts")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(factory => factory.CreateClient(nameof(IApiService)))
            .Returns(httpClient);

        var loggerMock = new Mock<ILogger<ApiService>>();
        var service = new ApiService(httpClientFactoryMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetAsync("api/accounts");

        // Assert
        Assert.Equal(responseContent, result);
        httpClientFactoryMock.Verify(factory => factory.CreateClient(nameof(IApiService)), Times.Once);
    }

    [Fact]
    public async Task PostAsync_ReturnsTrue_WhenResponseIsSuccessful()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(request =>
                    request.Method == HttpMethod.Post &&
                    request.RequestUri == new Uri("http://localhost/api/orders")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(factory => factory.CreateClient(nameof(IApiService)))
            .Returns(httpClient);

        var loggerMock = new Mock<ILogger<ApiService>>();
        var service = new ApiService(httpClientFactoryMock.Object, loggerMock.Object);

        var payload = new { AccountId = 1, Symbol = "AAPL" };

        // Act
        var result = await service.PostAsync("api/orders", payload);

        // Assert
        Assert.True(result);
        httpClientFactoryMock.Verify(factory => factory.CreateClient(nameof(IApiService)), Times.Once);
    }
}
