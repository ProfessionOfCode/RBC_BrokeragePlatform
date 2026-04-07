namespace RBC.BrokeragePlatform.WPF.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RBC.BrokeragePlatform.SharedCore.Enums;
using RBC.BrokeragePlatform.WPF.Model;
using RBC.BrokeragePlatform.WPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;


public partial class PlaceOrderViewModel : ObservableValidator, IDisposable
{
    private BrokerageService _brokerageService = default!;

    public Func<PlaceOrderViewModel, Account?, int>? ShowPlaceOrderConfirmationDialogCallback;

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
    private string selectedSymbol = string.Empty;

    [ObservableProperty]
    private string placeOrderTitle = string.Empty;

    [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
    [ObservableProperty]
    private OrderType selectedOrderType = new();

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

    [ObservableProperty]
    private bool isPlacingOrder;

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
            foreach (var error in GetErrors(e.PropertyName).Select(e => e.ErrorMessage).ToList() ?? Enumerable.Empty<string?>())
                ValidationErrors.Add(error);
        };     
    }
    public PlaceOrderViewModel(BrokerageService brokerageService): this()
    {
        _brokerageService = brokerageService;
    }

    public async Task LoadAccountDetailsAsync(Account? account)
    {
        SelectedAccount = account;
        if (account != null)
        {
            await RefreshPlaceOrderInterfaceAsync();
        }
    }


    [RelayCommand(CanExecute = nameof(CanPlaceOrder))]
    public async Task PlaceOrder()
    {
        // call brokerage service to send order to back-end hub
        try
        {
            IsPlacingOrder = true;

            var confirmationResult = ShowPlaceOrderConfirmationDialogCallback?.Invoke(this, SelectedAccount);

            if (confirmationResult != 1)
                return;    
            
            await _brokerageService.PlaceOrderAsync(SelectedAccount!.AccountId, SelectedSymbol, SelectedOrderType.OrderTypeId, Quantity, LimitPrice);

        }
        catch (Exception)
        {
            // log the error
            ValidationErrors.Clear();
            ValidationErrors.Add("An error occurred while placing the order. Please try again.");
        }
        finally
        {
            IsPlacingOrder = false;
        }   
    }

    private async Task RefreshPlaceOrderInterfaceAsync()
    {
        if (SelectedAccount == null)
            return;

        try
        {
            var orderTypes = _brokerageService.GetOrderTypes();
            OrderTypes.Clear();
            foreach (var orderType in orderTypes)
                OrderTypes.Add(orderType);

            var symbols = _brokerageService.GetSymbols(SelectedAccount.AccountId);
            Symbols.Clear();
            foreach (var symbol in symbols)
                Symbols.Add(symbol);

            PlaceOrderTitle = $"Place Order for {SelectedAccount.ClientName} ({SelectedAccount.AccountNumber})";

            Quantity = default!;

            LimitPrice = default!;

        }
        catch (Exception)
        {
            // log the error
            ValidationErrors.Clear();
            ValidationErrors.Add("An error occurred while placing the order. Please try again.");
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
        if (IsPlacingOrder)
            return false;

        var hasSelectedAccount = SelectedAccount != null;

        if (!hasSelectedAccount)
        {
            ValidationErrors.Clear();
            ValidationErrors.Add("An account must be selected.");
            return false;
        }

        var hasSelectedSymbol = !string.IsNullOrEmpty(SelectedSymbol);
        if (!hasSelectedSymbol)
        {
            ValidationErrors.Clear();
            ValidationErrors.Add("A symbol must be selected.");
            return false;
        }        

        var hasSelectedOrderType = SelectedOrderType != default;
        if (!hasSelectedOrderType)
        {
            ValidationErrors.Clear();
            ValidationErrors.Add("An order type must be selected.");
            return false;
        }

        var hasValidationErrors = GetErrors(nameof(Quantity))?.Cast<ValidationResult>().Any() == true
            || GetErrors(nameof(LimitPrice))?.Cast<ValidationResult>().Any() == true;

        if (hasValidationErrors)
        {
            // custom validation errors will be automatically added to ValidationErrors collection via ErrorsChanged event handler
            return false;
        }

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
            ValidationErrors.Add("Quantity and Limit Price must be greater than zero.");        //TODO: validate this scenario is not a possible request - we can not buy/sell 0 shares or at $0 limit price
            return false;
        }

        ValidationErrors.Clear();

        return true;
    }
}
