namespace RBC.BrokeragePlatform.WPF.Model;

using CommunityToolkit.Mvvm.ComponentModel;
using RBC.BrokeragePlatform.SharedCore.DTOs;

public partial class Position : ObservableObject
{
    [ObservableProperty]
    private int positionId;

    [ObservableProperty]
    private int accountId;

    [ObservableProperty]
    private int equityId;

    [ObservableProperty]
    private string symbol = string.Empty;

    [ObservableProperty]
    private int quantity;

    [ObservableProperty]
    private decimal averageCostPerShare;

    [ObservableProperty]
    public decimal currentPrice;        // this is not persisted in the database, but calculated on the fly when fetching positions from the brokerage service

    [ObservableProperty]
    public decimal currentValue;

    public void UpdateFromDto(PositionDto updatedPosition)
    {
        if(updatedPosition.PositionId != PositionId)
        {
            throw new ArgumentException("The provided PositionDto does not match the current Position.", nameof(updatedPosition));
        }
        if(updatedPosition.AccountId != AccountId)
        {
            throw new ArgumentException("The provided PositionDto does not match the current Position.", nameof(updatedPosition));
        }
        if(updatedPosition.EquityId != EquityId)
        {
            throw new ArgumentException("The provided PositionDto does not match the current Position.", nameof(updatedPosition));
        }
        if(updatedPosition.Symbol != Symbol)
        {
            throw new ArgumentException("The provided PositionDto does not match the current Position.", nameof(updatedPosition));
        }
        if(updatedPosition.CurrentPrice != CurrentPrice)
        {
            CurrentPrice = updatedPosition.CurrentPrice;
        }
        if(updatedPosition.CurrentValue != CurrentValue)
        {
            CurrentValue = updatedPosition.CurrentValue;
        }
        if(updatedPosition.Quantity != Quantity)
        {
           Quantity = updatedPosition.Quantity;
        }
        if (updatedPosition.AverageCostPerShare != AverageCostPerShare)
        {
            AverageCostPerShare = updatedPosition.AverageCostPerShare;
        }
    }
}
