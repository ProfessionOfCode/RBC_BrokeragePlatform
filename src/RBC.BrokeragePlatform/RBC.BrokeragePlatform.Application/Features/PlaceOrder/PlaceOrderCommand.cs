using MediatR;
using RBC.BrokeragePlatform.Application.Interfaces;

namespace RBC.BrokeragePlatform.Application.Features.PlaceOrder
{
    public class PlaceOrderCommand: ICommand<Unit>
    {
        public int AccountId { get; init; }
        public string Symbol { get; init; } = string.Empty;     // Identifies the stock or security being traded, e.g., "AAPL" for Apple Inc.
        public int OrderType { get; init; }
        public int Quantity { get; init; }
        public decimal LimitPrice { get; init; }
    }
}
