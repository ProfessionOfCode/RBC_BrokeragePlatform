using System.Windows;
using RBC.BrokeragePlatform.WPF.ViewModel;

namespace RBC.BrokeragePlatform.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainViewModel();
            RegisterCallbackToPlaceOrderDialogWindow(viewModel);
            DataContext = viewModel;

        }

        private void RegisterCallbackToPlaceOrderDialogWindow(MainViewModel viewModel)
        {
            viewModel.SelectedAccountViewModel.OpenPlaceOrderWindowAsDialog = (selectedAccountViewModel, account) =>
            {
                var placeOrderWindow = new PlaceOrderWindow();
                var placeOrderViewModel = new PlaceOrderViewModel();      // TODO : refactor to use DI
                placeOrderViewModel.SetBrokerageService(selectedAccountViewModel.GetBrokerageService());
                placeOrderViewModel.LoadAccountDetails(account);
                placeOrderWindow.DataContext = placeOrderViewModel;
                placeOrderWindow.Owner = this;
                placeOrderWindow.ShowDialog();
            };
        }
    }

}