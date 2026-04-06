using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        public HubConnection? Connection => _connection;
    }
}
