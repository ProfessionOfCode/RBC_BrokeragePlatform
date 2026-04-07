using Moq;
using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.WPF.Interfaces.Services;
using RBC.BrokeragePlatform.WPF.ViewModel;

namespace RBC.BrokeragePlatform.WPF.Tests.ViewModel;

public class AccountListViewModelTests
{
    [Fact]
    public void Constructor_InitializesDefaultState()
    {
        // Arrange
        var brokerageServiceMock = new Mock<IBrokerageService>();

        // Act
        var viewModel = new AccountListViewModel(brokerageServiceMock.Object);

        // Assert
        Assert.Empty(viewModel.Accounts);
        Assert.Null(viewModel.SelectedAccount);
        Assert.Equal(string.Empty, viewModel.SearchText);
    }

    [Fact]
    public async Task LoadAccountsAsync_MapsAccountsIntoCollection()
    {
        // Arrange
        var accounts = new List<AccountDto>
        {
            new() { AccountId = 1, AccountNumber = "ACC-001", ClientName = "Jane Doe", CashBalance = 1500m },
            new() { AccountId = 2, AccountNumber = "ACC-002", ClientName = "John Smith", CashBalance = 2500m }
        };

        var brokerageServiceMock = new Mock<IBrokerageService>();
        brokerageServiceMock
            .Setup(service => service.GetAllAccountsAsync())
            .ReturnsAsync(accounts);

        var viewModel = new AccountListViewModel(brokerageServiceMock.Object);

        // Act
        await viewModel.LoadAccountsAsync();

        // Assert
        Assert.Equal(2, viewModel.Accounts.Count);
        Assert.Equal(1, viewModel.Accounts[0].AccountId);
        Assert.Equal("ACC-001", viewModel.Accounts[0].AccountNumber);
        Assert.Equal("Jane Doe", viewModel.Accounts[0].ClientName);
        Assert.Equal(1500m, viewModel.Accounts[0].CashBalance);
    }
}
