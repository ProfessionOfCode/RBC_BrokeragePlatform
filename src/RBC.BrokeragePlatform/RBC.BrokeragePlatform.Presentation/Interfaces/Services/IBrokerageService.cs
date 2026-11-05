using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.WPF.Model;

namespace RBC.BrokeragePlatform.WPF.Interfaces.Services;

public interface IBrokerageService : IDisposable
{
    /// <summary>
    /// Starts real-time push notifications.
    /// </summary>
    Task StartPushNotificationsAsync();

    /// <summary>
    /// Subscribes the account to position update notifications.
    /// </summary>
    Task SubscribeToPositionUpdateGroup(int accountId);

    /// <summary>
    /// Unsubscribes the account from position update notifications.
    /// </summary>
    Task UnsubscribeToPositionUpdateGroup(int accountId);

    /// <summary>
    /// Registers a callback invoked when positions are updated.
    /// </summary>
    Task RegisterPositionUpdateCallbackAsync(Action<List<PositionDto>> callback);

    /// <summary>
    /// Unregisters the current position update callback.
    /// </summary>
    Task UnregisterPositionUpdateCallbackAsync();

    /// <summary>
    /// Retrieves all accounts.
    /// </summary>
    Task<List<AccountDto>> GetAllAccountsAsync();

    /// <summary>
    /// Retrieves positions for the specified account.
    /// </summary>
    Task<List<PositionDto>> GetPositionsAsync(int accountId);

    /// <summary>
    /// Gets supported order types.
    /// </summary>
    List<OrderType> GetOrderTypes();

    /// <summary>
    /// Gets symbols available for the specified account.
    /// </summary>
    List<string> GetSymbols(int accountId);

    /// <summary>
    /// Places an order.
    /// </summary>
    Task PlaceOrderAsync(int accountId, string selectedSymbol, int orderTypeId, int quantity, decimal limitPrice);
}
