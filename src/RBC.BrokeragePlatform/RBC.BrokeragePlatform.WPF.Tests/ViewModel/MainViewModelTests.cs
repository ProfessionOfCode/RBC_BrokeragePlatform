using Moq;
using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.WPF.Interfaces.Services;
using RBC.BrokeragePlatform.WPF.Model;
using RBC.BrokeragePlatform.WPF.ViewModel;

namespace RBC.BrokeragePlatform.WPF.Tests.ViewModel;

public class MainViewModelTests
{
    [Fact]
    public void Constructor_AssignsInjectedViewModels()
    {
        // Arrange
        var accountListViewModel = new AccountListViewModel(new Mock<IBrokerageService>().Object);
        var selectedAccountViewModel = new SelectedAccountViewModel(new Mock<IBrokerageService>().Object);
        var placeOrderViewModel = new PlaceOrderViewModel(new Mock<IBrokerageService>().Object);

        // Act
        var viewModel = new MainViewModel(accountListViewModel, selectedAccountViewModel, placeOrderViewModel);

        // Assert
        Assert.Same(accountListViewModel, viewModel.AccountListViewModel);
        Assert.Same(selectedAccountViewModel, viewModel.SelectedAccountViewModel);
        Assert.Same(placeOrderViewModel, viewModel.PlaceOrderViewModel);
    }

    [Fact]
    public async Task SelectedAccountChange_LoadsAccountDetailsThroughSelectedAccountViewModel()
    {
        // Arrange
        var account = new Account { AccountId = 15, AccountNumber = "ACC-015", ClientName = "Avery", CashBalance = 3000m };
        var positions = new List<PositionDto>
        {
            new() { PositionId = 25, AccountId = 15, EquityId = 300, Symbol = "MSFT", Quantity = 3, AverageCostPerShare = 200m, CurrentPrice = 210m, CurrentValue = 630m }
        };
        var loadCompleted = new TaskCompletionSource();

        var accountListServiceMock = new Mock<IBrokerageService>();
        var selectedAccountServiceMock = new Mock<IBrokerageService>();
        selectedAccountServiceMock.Setup(service => service.GetPositionsAsync(account.AccountId)).ReturnsAsync(positions);
        selectedAccountServiceMock.Setup(service => service.StartPushNotificationsAsync()).Returns(Task.CompletedTask);
        selectedAccountServiceMock.Setup(service => service.SubscribeToPositionUpdateGroup(account.AccountId)).Returns(Task.CompletedTask);
        selectedAccountServiceMock
            .Setup(service => service.RegisterPositionUpdateCallbackAsync(It.IsAny<Action<List<PositionDto>>>() ))
            .Callback(() => loadCompleted.SetResult())
            .Returns(Task.CompletedTask);
        var placeOrderServiceMock = new Mock<IBrokerageService>();

        var accountListViewModel = new AccountListViewModel(accountListServiceMock.Object);
        var selectedAccountViewModel = new SelectedAccountViewModel(selectedAccountServiceMock.Object);
        var placeOrderViewModel = new PlaceOrderViewModel(placeOrderServiceMock.Object);
        _ = new MainViewModel(accountListViewModel, selectedAccountViewModel, placeOrderViewModel);

        // Act
        accountListViewModel.SelectedAccount = account;
        await loadCompleted.Task.WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        Assert.Same(account, selectedAccountViewModel.SelectedAccount);
        Assert.Single(selectedAccountViewModel.Positions);
        Assert.Equal("MSFT", selectedAccountViewModel.Positions[0].Symbol);
        selectedAccountServiceMock.Verify(service => service.StartPushNotificationsAsync(), Times.Once);
        selectedAccountServiceMock.Verify(service => service.SubscribeToPositionUpdateGroup(account.AccountId), Times.Once);
        selectedAccountServiceMock.Verify(service => service.RegisterPositionUpdateCallbackAsync(It.IsAny<Action<List<PositionDto>>>()), Times.Once);
    }
}
