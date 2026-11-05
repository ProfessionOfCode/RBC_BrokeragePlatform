namespace RBC.BrokeragePlatform.WPF.Model;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class OrderType : ObservableObject
{
    [ObservableProperty]
    private int orderTypeId;

    [ObservableProperty]
    private string orderTypeName = string.Empty;
}
