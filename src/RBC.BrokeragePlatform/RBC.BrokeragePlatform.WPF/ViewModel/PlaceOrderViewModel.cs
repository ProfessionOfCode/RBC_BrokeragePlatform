namespace RBC.BrokeragePlatform.WPF.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RBC.BrokeragePlatform.WPF.Model;
using RBC.BrokeragePlatform.WPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;


public partial class PlaceOrderViewModel : ObservableValidator, IDisposable
{
    private BrokerageService _brokerageService = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
    private Account? selectedAccount;

    [ObservableProperty]
    private ObservableCollection<OrderType> orderTypes = new();

    [ObservableProperty]
    private ObservableCollection<string> symbols = new();

    [ObservableProperty]
    private ObservableCollection<string?> validationErrors = new();

    [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
    [ObservableProperty]
    private string selectedSymbol;

    [ObservableProperty]
    private string placeOrderTitle;

    [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
    [ObservableProperty]
    private OrderType selectedOrderType;

    [NotifyDataErrorInfo]
    [CustomValidation(typeof(PlaceOrderViewModel), nameof(ValidateQuantity))]
    [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
    [ObservableProperty]
    private int quantity;

    [NotifyDataErrorInfo]
    [CustomValidation(typeof(PlaceOrderViewModel), nameof(ValidateLimitPrice))]
    [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
    [ObservableProperty]
    private decimal limitPrice;

    public static ValidationResult ValidateLimitPrice(decimal limitPrice, ValidationContext context)
    {
        if (limitPrice <= 0){
            return new ValidationResult("Limit price must be greater than zero.");
        }
        
        

        return ValidationResult.Success!;
    }

    public static ValidationResult ValidateQuantity(int quantity, ValidationContext context)
    {
        if (quantity <= 0)
            return new ValidationResult("Quantity must be greater than zero.");

        return ValidationResult.Success!;
    }

    public PlaceOrderViewModel()
    {
        ErrorsChanged += (s, e) =>
        {
            ValidationErrors.Clear();
            foreach (var error in GetErrors(e.PropertyName).Select(e=> e.ErrorMessage).ToList() ?? Enumerable.Empty<string?>())
                ValidationErrors.Add(error);
        };
    }


    

    public void SetBrokerageService(BrokerageService brokerageService)
    {
        _brokerageService = brokerageService;
    }

    public void LoadAccountDetails(Account? account)
    {
        SelectedAccount = account;
        if (account != null)
            RefreshPlaceOrderInterface();
    }


    [RelayCommand(CanExecute = nameof(CanPlaceOrder))]
    public void PlaceOrder()
    {
        // call brokerage service to send order to back-end hub

    }

    private void RefreshPlaceOrderInterface()
    {
        if (SelectedAccount == null)
            return;

        try
        {
            var orderTypes = _brokerageService.GetOrderTypes();
            OrderTypes.Clear();
            foreach (var orderType in orderTypes)
                OrderTypes.Add(orderType);

            var symbols = _brokerageService.GetSymbols(selectedAccount.AccountNumber);
            Symbols.Clear();
            foreach (var symbol in symbols)
                Symbols.Add(symbol);

            PlaceOrderTitle = $"Place Order for {SelectedAccount.ClientName} ({SelectedAccount.AccountNumber})";

        }
        catch (Exception ex)
        {
            // log the error
        }
    }

    public void Dispose()
    {
        OrderTypes.Clear();
        Symbols.Clear();
        SelectedAccount = null;
    }

    private bool CanPlaceOrder()
    {
        var hasSelectedAccount = SelectedAccount != null;
        
        if(!hasSelectedAccount)
            return false;

        var hasValidationErrors = GetErrors(nameof(Quantity))?.Cast<ValidationResult>().Any() == true
            || GetErrors(nameof(LimitPrice))?.Cast<ValidationResult>().Any() == true;

        var hasSelectedSymbol = !string.IsNullOrEmpty(SelectedSymbol);

        var hasSelectedOrderType = SelectedOrderType != default;
        
        if(hasValidationErrors || !hasSelectedSymbol || !hasSelectedOrderType)
        return false;

        var isCashBalanceSufficient = SelectedAccount?.CashBalance >= Quantity * LimitPrice;

        if(SelectedOrderType?.OrderTypeId == (int)OrderTypeEnum.BUY && !isCashBalanceSufficient)
        {
            ValidationErrors.Clear();
            ValidationErrors.Add("Insufficient cash balance to place this order.");
            return false;
        }

        if(Quantity * LimitPrice == 0)
        {
            ValidationErrors.Clear();
            ValidationErrors.Add("Quantity and Limit Price must be greater than zero.");
            return false;
        }

        ValidationErrors.Clear();
        return true;

    }
}
