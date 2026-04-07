namespace RBC.BrokeragePlatform.WPF.Services;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.SharedCore.Enums;
using RBC.BrokeragePlatform.WPF.Model;

public class BrokerageService : IDisposable
{
    private readonly List<OrderType> _orderTypes;
    private readonly Dictionary<int, List<PositionDto>> _positions = new Dictionary<int, List<PositionDto>>();
    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;
    private readonly ILogger<BrokerageService> _logger;
    private Action<List<PositionDto>>? PositionUpdatedCallback { get; set; } = default!;
    private readonly Func<List<PositionDto>, Task>? PositionUpdatedBrokerageServiceCallback = default!;

    public BrokerageService(ApiService apiService, SignalRService signalRService, ILogger<BrokerageService> logger)
    {
        _apiService = apiService;
        _signalRService = signalRService;
        _logger = logger;
        _orderTypes = InitializeOrderTypes();
        PositionUpdatedBrokerageServiceCallback = async (updatedPositions) => await OnPositionUpdatedAsync(updatedPositions);
    }

    public async Task StartPushNotificationsAsync()
    {
        if (_signalRService.Connection == null)
        {
            await _signalRService.StartAsync();
        }
    }

    public async Task SubscribeToPositionUpdateGroup(int accountId)
    {
        await _signalRService.SubscribreToPositionUpdateGroup(accountId);
    }

    public async Task UnSubscribeToPositionUpdateGroup(int accountId)
    {
        await _signalRService.UnsubscribeFromPositionUpdateGroup(accountId);
    }

    public async Task RegisterPositionUpdateCallbackAsync(Action<List<PositionDto>> callback)
    {
        PositionUpdatedCallback = callback;
        await _signalRService.RegisterPositionUpdatesHandlerAsync(PositionUpdatedBrokerageServiceCallback!);
    }

    public async Task UnregisterPositionUpdateCallbackAsync()
    {
        PositionUpdatedCallback = null;
        await _signalRService.UnRegisterPositionUpdatesHandler();
    }

    private async Task OnPositionUpdatedAsync(List<PositionDto> updatedPositions)
    {
        _logger.LogInformation("Received PositionUpdated push for {Symbols}.", string.Join(",", updatedPositions.Select(p => p.Symbol).ToList()));

        if (RegisterPositionUpdateCallbackAsync == null)
        {
            _logger.LogWarning("PositionUpdatedCallback is not set. Cannot process position updates.");
            return;
        }

        PositionUpdatedCallback?.Invoke(updatedPositions);
    }

    public async Task<List<AccountDto>> GetAllAccountsAsync()
    {
        try
        {
            _logger.LogInformation("Requesting all accounts from API.");

            var result = await _apiService.GetAsync("api/accounts");

            // transform result to List<Account> if necessary, here we assume it's already in the correct format
            var accounts = JsonConvert.DeserializeObject<List<AccountDto>>(result);

            _logger.LogInformation("Received {Count} accounts.", accounts?.Count ?? 0);

            return accounts ?? new List<AccountDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get accounts.");
            return new List<AccountDto>();
        }
    }

    public async Task<List<PositionDto>> GetPositionsAsync(int accountId)
    {
        try
        {
            _logger.LogInformation($"Requesting all positions of account {accountId} from API.");

            var result = await _apiService.GetAsync($"api/positions/{accountId}");

            // transform result to List<Account> if necessary, here we assume it's already in the correct format
            var positionDtos = JsonConvert.DeserializeObject<List<PositionDto>>(result);

            _logger.LogInformation("Received {Count} accounts.", positionDtos?.Count ?? 0);

            if (!_positions.ContainsKey(accountId))
            {
                _positions[accountId] = positionDtos ?? new List<PositionDto>();
            }

            return positionDtos ?? new List<PositionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get positions.");
            return new List<PositionDto>();
        }
    }

    public List<OrderType> GetOrderTypes() => _orderTypes;

    public List<string> GetSymbols(int accountId)
    {
        _positions.TryGetValue(accountId, out var positions);

        return positions?.Select(p => p.Symbol).Distinct().ToList() ?? new List<string>();
    }

    private List<OrderType> InitializeOrderTypes()
    {
        return new()
        {
            new() { OrderTypeId = (int)OrderTypeEnum.BUY, OrderTypeName = "Buy" },
            new() { OrderTypeId = (int)OrderTypeEnum.SELL, OrderTypeName = "Sell" },
        };
    }

    public void Dispose()
    {
        _positions.Clear();

        _apiService.Dispose();

        _signalRService.Dispose();

        GC.SuppressFinalize(this);
        GC.Collect();

    }

    public async Task PlaceOrderAsync(int accountId, string selectedSymbol, int orderTypeId, int quantity, decimal limitPrice)
    {
        // get equityId based on symbol and accountId
        var equityId = _positions.TryGetValue(accountId, out var positions)
            ? positions.FirstOrDefault(p => p.Symbol == selectedSymbol)?.EquityId
            : 0;

        try
        {
            _logger.LogInformation("Placing an order from API.");

            var hasSucceeded = await _apiService.PostAsync("api/orders", new
            {
                AccountId = accountId,
                // add equity id and position id if needed in the future
                Symbol = selectedSymbol,
                OrderType = orderTypeId,
                Quantity = quantity,
                LimitPrice = limitPrice
            });

            if (hasSucceeded)
            {
                _logger.LogInformation("Order placed successfully for AccountId: {AccountId}", accountId);
                // optionally, you can trigger a refresh of positions here if you want to immediately reflect the new order in the UI
                // update the cashbalance and positions immediately after placing the order, or rely on the push notification to update the positions
            }
            else
            {
                _logger.LogWarning("Order placement failed for AccountId: {AccountId}", accountId);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to place order.");

        }
    }

}