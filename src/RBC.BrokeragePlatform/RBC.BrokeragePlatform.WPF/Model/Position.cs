namespace RBC.BrokeragePlatform.WPF.Model;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class Position : ObservableObject
{
    [ObservableProperty]
    private string symbol = string.Empty;

    [ObservableProperty]
    private int quantity;

    [ObservableProperty]
    private decimal averageCostPerShare;

    [ObservableProperty]
    private decimal currentPrice;

    [ObservableProperty]
    public decimal currentValue;

}
