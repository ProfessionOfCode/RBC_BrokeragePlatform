using Moq;
using RBC.BrokeragePlatform.WPF.Interfaces.Services;
using RBC.BrokeragePlatform.WPF.Model;
using RBC.BrokeragePlatform.WPF.ViewModel;

namespace RBC.BrokeragePlatform.WPF.Tests.ViewModel;

public class PlaceOrderViewModelTests
{
    [Fact]
    public async Task LoadAccountDetailsAsync_PopulatesOrderTypesSymbolsAndTitle()
    {
        // Arrange
        var account = new Account { AccountId = 9, AccountNumber = "ACC-009", ClientName = "Chris", CashBalance = 7500m };
        var orderTypes = new List<OrderType>
        {
            new() { OrderTypeId = 1, OrderTypeName = "Buy" },
            new() { OrderTypeId = 2, OrderTypeName = "Sell" }
        };
        var symbols = new List<string> { "AAPL", "MSFT" };

        var brokerageServiceMock = new Mock<IBrokerageService>();
        brokerageServiceMock.Setup(service => service.GetOrderTypes()).Returns(orderTypes);
        brokerageServiceMock.Setup(service => service.GetSymbols(account.AccountId)).Returns(symbols);

        var viewModel = new PlaceOrderViewModel(brokerageServiceMock.Object);

        // Act
        await viewModel.LoadAccountDetailsAsync(account);

        // Assert
        Assert.Same(account, viewModel.SelectedAccount);
        Assert.Equal(2, viewModel.OrderTypes.Count);
        Assert.Equal(2, viewModel.Symbols.Count);
        Assert.Equal("Place Order for Chris (ACC-009)", viewModel.PlaceOrderTitle);
        Assert.Equal(0, viewModel.Quantity);
        Assert.Equal(0m, viewModel.LimitPrice);
    }

    [Fact]
    public async Task PlaceOrder_CallsBrokerageService_WhenConfirmationIsAccepted()
    {
        // Arrange
        var account = new Account { AccountId = 12, AccountNumber = "ACC-012", ClientName = "Morgan", CashBalance = 10000m };
        var selectedOrderType = new OrderType { OrderTypeId = 1, OrderTypeName = "Buy" };

        var brokerageServiceMock = new Mock<IBrokerageService>();
        brokerageServiceMock.Setup(service => service.PlaceOrderAsync(account.AccountId, "AAPL", selectedOrderType.OrderTypeId, 10, 50m)).Returns(Task.CompletedTask);

        var viewModel = new PlaceOrderViewModel(brokerageServiceMock.Object)
        {
            SelectedAccount = account,
            SelectedSymbol = "AAPL",
            SelectedOrderType = selectedOrderType,
            Quantity = 10,
            LimitPrice = 50m,
            ShowPlaceOrderConfirmationDialogCallback = (_, _) => 1
        };

        // Act
        await viewModel.PlaceOrder();

        // Assert
        brokerageServiceMock.Verify(service => service.PlaceOrderAsync(account.AccountId, "AAPL", selectedOrderType.OrderTypeId, 10, 50m), Times.Once);
    }
}
