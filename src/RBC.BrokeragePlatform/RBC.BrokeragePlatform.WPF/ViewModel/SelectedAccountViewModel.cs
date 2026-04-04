namespace RBC.BrokeragePlatform.WPF.ViewModel;

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RBC.BrokeragePlatform.WPF.Model;
using RBC.BrokeragePlatform.WPF.Services;

public partial class SelectedAccountViewModel : ObservableObject, IDisposable
{
    private readonly BrokerageService _brokerageService;

    public Action<SelectedAccountViewModel, Account?>? OpenPlaceOrderWindowAsDialog;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand), nameof(RefreshPositionsCommand))]
    private Account? selectedAccount;

    [ObservableProperty]
    private ObservableCollection<Position> positions = new();

    [ObservableProperty]
    private bool isLoading;

    public SelectedAccountViewModel(BrokerageService brokerageService)
    {
        _brokerageService = brokerageService;
    }

    public void LoadAccountDetails(Account? account)
    {
        SelectedAccount = account;
        if (account != null)
            RefreshPositionsInternal();
    }

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    public void RefreshPositions()
    {
        RefreshPositionsInternal();
    }

    [RelayCommand(CanExecute = nameof(CanPlaceOrder))]
    public void PlaceOrder()
    {
        // callback to main view code behind to open place order interface
        OpenPlaceOrderWindowAsDialog?.Invoke(this, SelectedAccount);
    }

    private void RefreshPositionsInternal()
    {
        if (SelectedAccount == null)
            return;

        IsLoading = true;
        try
        {
            var positions = _brokerageService.GetPositions(SelectedAccount.AccountNumber);
            Positions.Clear();
            foreach (var position in positions)
                Positions.Add(position);
        }
        finally
        {
            IsLoading = false;
        }
    }

    internal BrokerageService GetBrokerageService()
    {
        return _brokerageService;
    }

    public void Dispose()
    {
        Positions.Clear();
    }

    private bool CanRefresh => SelectedAccount != null;
    private bool CanPlaceOrder => SelectedAccount != null;
}
