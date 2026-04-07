using MediatR;
using RBC.BrokeragePlatform.Application.Features.AccountPositionsUpdated;
using RBC.BrokeragePlatform.Application.Features.GetAllAccounts;
using RBC.BrokeragePlatform.WebAPI.Services;

namespace RBC.BrokeragePlatform.WebAPI.BackgroundServices
{
    public class MarketUpdateSimulatorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MarketUpdateSimulatorService> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(4);
        private readonly Random _random = new();

        public MarketUpdateSimulatorService(IServiceProvider serviceProvider, ILogger<MarketUpdateSimulatorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Market Update Simulator service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PerformMarketUpdateAsync(stoppingToken);
                    await Task.Delay(_updateInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Market Update Simulator service cancellation requested.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Error in Market Update Simulator - Exception: {ExceptionMessage}", ex.Message);
                }
            }

            _logger.LogInformation("Market Update Simulator service stopped.");
        }

        private async Task PerformMarketUpdateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Market tick started - Interval: 1 second");

            using (var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var positionPushService = scope.ServiceProvider.GetRequiredService<IPositionPushService>();

                try
                {
                    var accountsQuery = new GetAllAccountsQuery();
                    var accounts = await mediator.Send(accountsQuery, cancellationToken);
                    var accountList = accounts.ToList();

                    _logger.LogInformation("Market tick processing accounts - AccountCount: {AccountCount}", accountList.Count);

                    foreach (var account in accountList)
                    {
                        try
                        {
                            _logger.LogInformation("Processing account - AccountId: {AccountId}, ClientName: {ClientName}", 
                                account.AccountId, account.ClientName);

                            // Simulate market update by randomly adjusting equity prices (this is just a placeholder for actual market logic)
                            await mediator.Publish(new PositionUpdatedEvent { AccountId = account.AccountId, PriceChange = GetDeltaPriceChangeFromMarket() }, cancellationToken);
                            await Task.Delay(100, cancellationToken); // Simulate processing time
                            await positionPushService.PushAccountPositionsAsync(account.AccountId, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation("Error pushing positions for account - AccountId: {AccountId}, Exception: {ExceptionMessage}", 
                                account.AccountId, ex.Message);
                        }
                    }

                    _logger.LogInformation("Market tick completed - ProcessedAccounts: {ProcessedAccounts}", accountList.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Error during market update - Exception: {ExceptionMessage}", ex.Message);
                }
            }
        }

        private decimal GetDeltaPriceChangeFromMarket()
        {            
            decimal lowerBound = -1.5m;
            decimal upperBound = 1.5m;

            var isNegative = _random.Next(0, 13) % 3 == 0;

            var priceChange = lowerBound + ((decimal)(_random.NextDouble() / Double.MaxValue) * (upperBound - lowerBound));
            
            return isNegative ? -priceChange : priceChange;
        }
    }
}
