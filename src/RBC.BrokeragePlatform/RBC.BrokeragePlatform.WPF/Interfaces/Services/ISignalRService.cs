using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.WPF.Interfaces.Services;

public interface ISignalRService : IDisposable
{
    /// <summary>
    /// Starts the SignalR connection.
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// Subscribes to position updates for the provided account.
    /// </summary>
    Task SubscribeToPositionUpdateGroup(int accountId);

    /// <summary>
    /// Unsubscribes from position updates for the provided account.
    /// </summary>
    Task UnsubscribeFromPositionUpdateGroup(int accountId);

    /// <summary>
    /// Registers a handler for position update events.
    /// </summary>
    Task RegisterPositionUpdatesHandlerAsync(Func<List<PositionDto>, Task> handler);

    /// <summary>
    /// Unregisters the handler for position update events.
    /// </summary>
    Task UnregisterPositionUpdatesHandlerAsync();

    /// <summary>
    /// Gets whether a SignalR connection is currently initialized.
    /// </summary>
    bool IsConnected { get; }
}
