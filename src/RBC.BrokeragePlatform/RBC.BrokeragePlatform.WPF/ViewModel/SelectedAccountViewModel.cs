namespace RBC.BrokeragePlatform.WPF.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.WPF.Interfaces.Services;
using RBC.BrokeragePlatform.WPF.Model;
using System.Collections.ObjectModel;

public partial class SelectedAccountViewModel : ObservableObject, IDisposable
{
    private readonly IBrokerageService _brokerageService;

    public Action<SelectedAccountViewModel, Account?>? ShowPlaceOrderWindowAsDialogCallback;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand), nameof(RefreshPositionsCommand))]
    private Account? selectedAccount;

    [ObservableProperty]
    private ObservableCollection<Position> positions = new();

    [ObservableProperty]
    private bool isLoading;

    public SelectedAccountViewModel(): this(App.Services.GetRequiredService<IBrokerageService>())
    {
        
    }

    public SelectedAccountViewModel(IBrokerageService brokerageService)
    {
        _brokerageService = brokerageService;
    }

    public async Task LoadAccountDetailsAsync(Account? account)
    {
        SelectedAccount = account;
        if (account != null)
        {
            await RefreshAccountPositionsAsync();
            await _brokerageService.StartPushNotificationsAsync();
            await _brokerageService.SubscribeToPositionUpdateGroup(account!.AccountId);

            await _brokerageService.RegisterPositionUpdateCallbackAsync(UpdatePositionInCollection);
        }
    }

    private void UpdatePositionInCollection(List<PositionDto> updatedPositions)
    {
        foreach (var updatedPosition in updatedPositions)
        {
            var positionFound = Positions.FirstOrDefault(p => p.PositionId == updatedPosition.PositionId);

            if (positionFound != null && positionFound is Position positionToUpdate)
            {
                positionToUpdate.UpdateFromDto(updatedPosition);
            }
            else
            {
                Positions.Add(new Position()
                {
                    PositionId = updatedPosition.PositionId,
                    AccountId = updatedPosition.AccountId,
                    EquityId = updatedPosition.EquityId,
                    Symbol = updatedPosition.Symbol,
                    AverageCostPerShare = updatedPosition.AverageCostPerShare,
                    CurrentPrice = updatedPosition.CurrentPrice,
                    Quantity = updatedPosition.Quantity,
                    CurrentValue = updatedPosition.CurrentValue

                });
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    public async Task RefreshPositions()
    {
       await RefreshAccountPositionsAsync();
    }

    [RelayCommand(CanExecute = nameof(CanPlaceOrder))]
    public void PlaceOrder()
    {
        ShowPlaceOrderWindowAsDialogCallback?.Invoke(this, SelectedAccount);
    }

    private async Task RefreshAccountPositionsAsync()
    {
        if (SelectedAccount == null)
            return;

        IsLoading = true;
        try
        {
            var positionDtos = await _brokerageService.GetPositionsAsync(SelectedAccount.AccountId);
            var positions =  positionDtos.Select(p => new Position()
                            {
                                PositionId = p.PositionId,
                                AccountId = p.AccountId,
                                EquityId = p.EquityId,
                                Symbol = p.Symbol,
                                AverageCostPerShare = p.AverageCostPerShare,
                                CurrentPrice = p.CurrentPrice,
                                Quantity = p.Quantity,
                                CurrentValue = p.CurrentValue
                            });

            Positions.Clear();
            foreach (var position in positions)
            {
                Positions.Add(position);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Dispose()
    {
        Positions.Clear();
        _brokerageService.Dispose();
        GC.SuppressFinalize(this);
        GC.Collect();
    }

    public async Task UnRegisterAccountPositionUpdates()
    {
        if (SelectedAccount == null)
            return;

        await _brokerageService.UnregisterPositionUpdateCallbackAsync();
        await _brokerageService.UnsubscribeToPositionUpdateGroup(SelectedAccount.AccountId);
    }

    private bool CanRefresh => SelectedAccount != null;
    private bool CanPlaceOrder => SelectedAccount != null;
}
