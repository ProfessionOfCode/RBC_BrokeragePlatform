namespace RBC.BrokeragePlatform.WPF.Model;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class Account : ObservableObject
{
    [ObservableProperty]
    private string accountNumber = string.Empty;

    [ObservableProperty]
    private string clientName = string.Empty;

    [ObservableProperty]
    private decimal cashBalance;
}
