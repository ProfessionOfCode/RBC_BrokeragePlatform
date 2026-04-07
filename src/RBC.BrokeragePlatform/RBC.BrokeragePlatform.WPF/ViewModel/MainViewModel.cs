namespace RBC.BrokeragePlatform.WPF.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RBC.BrokeragePlatform.WPF.Services;

public partial class MainViewModel : ObservableObject
{
    private readonly BrokerageService _brokerageService;
    
    public AccountListViewModel AccountListViewModel { get; }
    public SelectedAccountViewModel SelectedAccountViewModel { get; }
    public PlaceOrderViewModel PlaceOrderViewModel { get; }

    public MainViewModel()
    {
        var apiService = App.Services.GetRequiredService<ApiService>();
        var signalRService = App.Services.GetRequiredService<SignalRService>();
        var loggerService = App.Services.GetRequiredService<ILogger<BrokerageService>>();

        _brokerageService = new BrokerageService(apiService, signalRService, loggerService);
        AccountListViewModel = new AccountListViewModel(_brokerageService);
        SelectedAccountViewModel = new SelectedAccountViewModel(_brokerageService);
        PlaceOrderViewModel = new PlaceOrderViewModel(_brokerageService);

        // Master-detail binding
        AccountListViewModel.PropertyChanged += async (s, e) =>
        {
            if (e.PropertyName == nameof(AccountListViewModel.SelectedAccount))
            {
                await SelectedAccountViewModel.UnRegisterAccountPositionUpdates();  // unsubscribe from previous account position updates if any
                await SelectedAccountViewModel.LoadAccountDetailsAsync(AccountListViewModel.SelectedAccount);
            }               
        };
    }
}
