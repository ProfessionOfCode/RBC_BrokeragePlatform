namespace RBC.BrokeragePlatform.WPF.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class MainViewModel : ObservableObject
{
    public AccountListViewModel AccountListViewModel { get; }
    public SelectedAccountViewModel SelectedAccountViewModel { get; }
    public PlaceOrderViewModel PlaceOrderViewModel { get; }

    public MainViewModel(
        AccountListViewModel accountListViewModel,
        SelectedAccountViewModel selectedAccountViewModel,
        PlaceOrderViewModel placeOrderViewModel)
    {
        AccountListViewModel = accountListViewModel;
        SelectedAccountViewModel = selectedAccountViewModel;
        PlaceOrderViewModel = placeOrderViewModel;

        AccountListViewModel.PropertyChanged += async (s, e) =>
        {
            if (e.PropertyName == nameof(AccountListViewModel.SelectedAccount))
            {
                await SelectedAccountViewModel.UnRegisterAccountPositionUpdates();
                await SelectedAccountViewModel.LoadAccountDetailsAsync(AccountListViewModel.SelectedAccount);
            }
        };
    }
}
