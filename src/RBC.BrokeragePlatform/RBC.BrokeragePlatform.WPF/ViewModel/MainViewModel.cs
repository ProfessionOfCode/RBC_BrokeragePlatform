namespace RBC.BrokeragePlatform.WPF.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using RBC.BrokeragePlatform.WPF.Services;

public partial class MainViewModel : ObservableObject
{
    private readonly BrokerageService _brokerageService;
    
    public AccountListViewModel AccountListViewModel { get; }
    public SelectedAccountViewModel SelectedAccountViewModel { get; }
    public PlaceOrderViewModel PlaceOrderViewModel { get; }

    public MainViewModel()
    {
        _brokerageService = new BrokerageService();
        AccountListViewModel = new AccountListViewModel(_brokerageService);
        SelectedAccountViewModel = new SelectedAccountViewModel(_brokerageService);
        PlaceOrderViewModel = new PlaceOrderViewModel(_brokerageService);

        // Master-detail binding
        AccountListViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(AccountListViewModel.SelectedAccount))
                SelectedAccountViewModel.LoadAccountDetails(AccountListViewModel.SelectedAccount);
        };
    }
}
