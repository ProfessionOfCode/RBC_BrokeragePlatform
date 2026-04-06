namespace RBC.BrokeragePlatform.WPF.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using RBC.BrokeragePlatform.WPF.Model;
using RBC.BrokeragePlatform.WPF.Services;
using System.Collections.ObjectModel;

public partial class AccountListViewModel : ObservableObject
{
    private readonly BrokerageService _brokerageService;

    [ObservableProperty]
    private ObservableCollection<Account> accounts = new();

    [ObservableProperty]
    private Account? selectedAccount;

    [ObservableProperty]
    private string searchText = string.Empty;

    public AccountListViewModel(BrokerageService brokerageService)
    {
        _brokerageService = brokerageService;
        
    }

    public async Task OnSearchTextChangedAsync(string value)
    {
        await FilterAccountsAsync();
    }

    public async Task LoadAccountsAsync()
    {
        var allAccounts = await _brokerageService.GetAllAccountsAsync();
        Accounts = new ObservableCollection<Account>([.. allAccounts.Select(a => new Account()
        {
            AccountId = a.AccountId,
            AccountNumber = a.AccountNumber,
            CashBalance = a.CashBalance,
            ClientName = a.ClientName
        })]);
    }

    private async Task FilterAccountsAsync()
    {
        var allAccounts = await _brokerageService.GetAllAccountsAsync();
        var filtered = allAccounts.Where(a =>
            a.AccountNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            a.ClientName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
        ).Select(a => new Account()
        {
            AccountNumber = a.AccountNumber,
            CashBalance = a.CashBalance,
            ClientName = a.ClientName
        }).ToList();

        Accounts.Clear();
        foreach (var account in filtered)
            Accounts.Add(account);
    }
}
