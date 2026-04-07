using Moq;
using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.WPF.Interfaces.Services;
using RBC.BrokeragePlatform.WPF.Model;
using RBC.BrokeragePlatform.WPF.ViewModel;

namespace RBC.BrokeragePlatform.WPF.Tests.ViewModel;

public class SelectedAccountViewModelTests
{
    [Fact]
    public async Task LoadAccountDetailsAsync_LoadsPositionsAndRegistersForUpdates()
    {
        // Arrange
        var account = new Account { AccountId = 3, AccountNumber = "ACC-003", ClientName = "Taylor", CashBalance = 5000m };
        var positions = new List<PositionDto>
        {
            new() { PositionId = 11, AccountId = 3, EquityId = 101, Symbol = "AAPL", Quantity = 4, AverageCostPerShare = 125m, CurrentPrice = 130m, CurrentValue = 520m }
        };

        var brokerageServiceMock = new Mock<IBrokerageService>();
        brokerageServiceMock.Setup(service => service.GetPositionsAsync(account.AccountId)).ReturnsAsync(positions);
        brokerageServiceMock.Setup(service => service.StartPushNotificationsAsync()).Returns(Task.CompletedTask);
        brokerageServiceMock.Setup(service => service.SubscribeToPositionUpdateGroup(account.AccountId)).Returns(Task.CompletedTask);
        brokerageServiceMock.Setup(service => service.RegisterPositionUpdateCallbackAsync(It.IsAny<Action<List<PositionDto>>>())).Returns(Task.CompletedTask);

        var viewModel = new SelectedAccountViewModel(brokerageServiceMock.Object);

        // Act
        await viewModel.LoadAccountDetailsAsync(account);

        // Assert
        Assert.Same(account, viewModel.SelectedAccount);
        Assert.Single(viewModel.Positions);
        Assert.Equal("AAPL", viewModel.Positions[0].Symbol);
        brokerageServiceMock.Verify(service => service.StartPushNotificationsAsync(), Times.Once);
        brokerageServiceMock.Verify(service => service.SubscribeToPositionUpdateGroup(account.AccountId), Times.Once);
        brokerageServiceMock.Verify(service => service.RegisterPositionUpdateCallbackAsync(It.IsAny<Action<List<PositionDto>>>()), Times.Once);
    }

    [Fact]
    public async Task UnRegisterAccountPositionUpdates_UnsubscribesAndUnregisters_WhenAccountIsSelected()
    {
        // Arrange
        var account = new Account { AccountId = 5, AccountNumber = "ACC-005", ClientName = "Jordan", CashBalance = 4000m };
        var brokerageServiceMock = new Mock<IBrokerageService>();
        brokerageServiceMock.Setup(service => service.UnregisterPositionUpdateCallbackAsync()).Returns(Task.CompletedTask);
        brokerageServiceMock.Setup(service => service.UnsubscribeToPositionUpdateGroup(account.AccountId)).Returns(Task.CompletedTask);

        var viewModel = new SelectedAccountViewModel(brokerageServiceMock.Object)
        {
            SelectedAccount = account
        };

        // Act
        await viewModel.UnRegisterAccountPositionUpdates();

        // Assert
        brokerageServiceMock.Verify(service => service.UnregisterPositionUpdateCallbackAsync(), Times.Once);
        brokerageServiceMock.Verify(service => service.UnsubscribeToPositionUpdateGroup(account.AccountId), Times.Once);
    }
}
