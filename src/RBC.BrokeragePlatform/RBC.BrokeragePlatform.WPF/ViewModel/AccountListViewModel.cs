namespace RBC.BrokeragePlatform.WPF.ViewModel;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using RBC.BrokeragePlatform.WPF.Model;
using RBC.BrokeragePlatform.WPF.Services;

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
        LoadAccounts();
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterAccounts();
    }

    private void LoadAccounts()
    {
        var allAccounts = _brokerageService.GetAllAccounts();
        Accounts = new ObservableCollection<Account>(allAccounts);
    }

    private void FilterAccounts()
    {
        var allAccounts = _brokerageService.GetAllAccounts();
        var filtered = allAccounts.Where(a =>
            a.AccountNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            a.ClientName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
        ).ToList();

        Accounts.Clear();
        foreach (var account in filtered)
            Accounts.Add(account);
    }
}
