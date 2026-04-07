using RBC.BrokeragePlatform.WPF.Model;
using RBC.BrokeragePlatform.WPF.ViewModel;
using System.Windows;

namespace RBC.BrokeragePlatform.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlaceOrderWindow _placeOrderWindow = null!;

        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += async (s, e) =>
            {
                if (e.NewValue is MainViewModel newViewModel)
                {
                    RegisterCallbackToShowPlaceOrderDialogWindow(newViewModel);
                    RegisterPlaceOrderConfirmationMessage(newViewModel);
                    await newViewModel.AccountListViewModel.LoadAccountsAsync();
                }
            };
                        
            DataContext = new MainViewModel();
        }

        private void RegisterPlaceOrderConfirmationMessage(MainViewModel viewModel)
        {            
            viewModel.PlaceOrderViewModel.ShowPlaceOrderConfirmationDialogCallback += ShowConfirmationDialog;
        }

        private void RegisterCallbackToShowPlaceOrderDialogWindow(MainViewModel viewModel)
        {
            viewModel.SelectedAccountViewModel.ShowPlaceOrderWindowAsDialogCallback += async (selectedAccountViewModel, account) =>
            {
                _placeOrderWindow = new PlaceOrderWindow
                {
                    DataContext = viewModel.PlaceOrderViewModel
                };
                await viewModel.PlaceOrderViewModel.LoadAccountDetailsAsync(account);
                _placeOrderWindow.Owner = this;
                _placeOrderWindow.ShowDialog();
            };
        }

        private int ShowConfirmationDialog(PlaceOrderViewModel model, Account? account)
        {
            var result = MessageBox.Show(_placeOrderWindow, "Are you sure you want to place this order?", "Confirm Order", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes ? 1 : 0;
        }
    }
}