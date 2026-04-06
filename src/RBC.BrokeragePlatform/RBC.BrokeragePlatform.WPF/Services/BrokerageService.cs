namespace RBC.BrokeragePlatform.WPF.Services;

using Microsoft.AspNetCore.SignalR.Client;
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
    public Action<List<PositionDto>>? PositionUpdatedCallback { get; set; }

    public BrokerageService(ApiService apiService, SignalRService signalRService, ILogger<BrokerageService> logger)
    {
        _apiService = apiService;
        _signalRService = signalRService;
        _logger = logger;
        _orderTypes = InitializeOrderTypes();

        SubscribeToSignalR();
    }

    private void SubscribeToSignalR()
    {
        var connection = _signalRService.Connection;
        if (connection != null)
        {
            connection.On<List<PositionDto>>("PositionUpdated", OnPositionUpdated);
            _logger.LogInformation("Subscribed to PositionUpdated SignalR event.");
        }
        else
        {
            _logger.LogWarning("SignalR connection is not initialized.");
        }
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

    private void OnPositionUpdated(List<PositionDto> updatedPositions)
    {
        _logger.LogInformation("Received PositionUpdated push for {Symbols}.", string.Join( ",", updatedPositions.Select(p => p.Symbol).ToList()));
        PositionUpdatedCallback?.Invoke(updatedPositions);
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
}
