using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.WPF.Services
{
    public class SignalRService : IDisposable
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SignalRService> _logger;
        private HubConnection? _connection;
        private readonly AsyncRetryPolicy _retryPolicy;

        public SignalRService(IConfiguration config, ILogger<SignalRService> logger)
        {
            _config = config;
            _logger = logger;
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Min(10, retryAttempt)),
                    (ex, ts) => _logger.LogWarning(ex, "Retrying SignalR connection in {Delay}s", ts.TotalSeconds));
        }

        public async Task StartAsync()
        {
            var hubUrl = _config["SignalR:HubUrl"]!;
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _connection.Closed += async (error) =>
            {
                _logger.LogWarning(error, "SignalR connection closed. Reconnecting...");
                await _retryPolicy.ExecuteAsync(() => _connection.StartAsync());
            };

            await _retryPolicy.ExecuteAsync(() => _connection.StartAsync());
            _logger.LogInformation("SignalR connected to {HubUrl}", hubUrl);
        }

        public async Task SubscribreToPositionUpdateGroup(int accountId)
        {
            _logger.LogInformation("Subscribing to position updates for AccountId: {AccountId}", accountId);

            if (_connection == null)
            {
                _logger.LogError("SignalR connection is not established. Cannot subscribe to position updates.");
                return;
            }

            await _connection.SendAsync("SubscribeToPositionUpdates", accountId);

            _logger.LogInformation("Subscribed to position updates for AccountId: {AccountId}", accountId);
        }

        public async Task UnsubscribeFromPositionUpdateGroup(int accountId)
        {
            _logger.LogInformation("Unsubscribing from position updates for AccountId: {AccountId}", accountId);
            if (_connection == null)
            {
                _logger.LogError("SignalR connection is not established. Cannot unsubscribe from position updates.");
                return;
            }
            await _connection.SendAsync("UnsubscribeFromPositionUpdates", accountId);
            _logger.LogInformation("Unsubscribed from position updates for AccountId: {AccountId}", accountId);
        }


        public async Task RegisterPositionUpdatesHandlerAsync(Func<List<PositionDto>, Task> handler)
        {
            if (_connection == null)
            {
                _logger.LogError("SignalR connection is not established. Cannot register position updates handler.");
                return;
            }
            _connection.On("PositionUpdated", handler);
            _logger.LogInformation("Registered position updates handler");
        }

        public async Task UnRegisterPositionUpdatesHandler() 
        {
            if (_connection == null)
            {
                _logger.LogError("SignalR connection is not established. Cannot register position updates handler.");
                return;
            }
            _connection.Remove("PositionUpdated");
            _logger.LogInformation("Registered position updates handler");
        }


        public async void Dispose()
        {
            // unsubscribe from all events and stop the connection
            StopConnectionsAsync();
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        private async void StopConnectionsAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
                _logger.LogInformation("SignalR connection stopped and disposed");
            }
        }

        public HubConnection? Connection => _connection;
    }
}
