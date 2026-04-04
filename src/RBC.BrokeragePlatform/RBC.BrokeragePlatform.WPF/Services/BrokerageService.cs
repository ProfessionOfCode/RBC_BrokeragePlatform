namespace RBC.BrokeragePlatform.WPF.Services;

using RBC.BrokeragePlatform.WPF.Model;

public class BrokerageService
{
    private readonly List<Account> _accounts;
    private readonly List<OrderType> _orderTypes;
    private readonly Dictionary<string, List<Position>> _positions;

    public BrokerageService()
    {
        _accounts = InitializeAccounts();
        _orderTypes = InitializeOrderTypes();
        _positions = InitializePositions();
    }

    public List<Account> GetAllAccounts() => _accounts;

    public List<Position> GetPositions(string accountNumber)
        => _positions.TryGetValue(accountNumber, out var pos) ? pos : new();

    public List<OrderType> GetOrderTypes() => _orderTypes;
    public List<string> GetSymbols(string accountNumber) => _positions.TryGetValue(accountNumber, out var pos) ? pos.Select(p => p.Symbol).ToList() : new();

    private List<Account> InitializeAccounts()
    {
        return new()
        {
            new() { AccountNumber = "ACC001", ClientName = "John Smith", CashBalance = 150000m },
            new() { AccountNumber = "ACC002", ClientName = "Sarah Johnson", CashBalance = 250000m },
            new() { AccountNumber = "ACC003", ClientName = "Michael Chen", CashBalance = 75000m },
        };
    }
    
    private List<OrderType> InitializeOrderTypes()
    {
        return new()
        {
            new() { OrderTypeId = (int)OrderTypeEnum.BUY, OrderTypeName = "Buy" },
            new() { OrderTypeId = (int)OrderTypeEnum.SELL, OrderTypeName = "Sell" },
        };
    }

    private Dictionary<string, List<Position>> InitializePositions()
    {
        return new()
        {
            {
                "ACC001", new()
                {
                    new() { Symbol = "AAPL", Quantity = 100, AverageCostPerShare = 150.50m, CurrentPrice = 178.45m },
                    new() { Symbol = "MSFT", Quantity = 50, AverageCostPerShare = 300.75m, CurrentPrice = 315.20m },
                    new() { Symbol = "GOOGL", Quantity = 25, AverageCostPerShare = 2500m, CurrentPrice = 2650.30m },
                }
            },
            {
                "ACC002", new()
                {
                    new() { Symbol = "TSLA", Quantity = 30, AverageCostPerShare = 800m, CurrentPrice = 950.50m },
                    new() { Symbol = "AMZN", Quantity = 40, AverageCostPerShare = 3200m, CurrentPrice = 3450.20m },
                }
            },
        };
    }
}
