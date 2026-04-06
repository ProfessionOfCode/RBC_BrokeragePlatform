using MediatR;

namespace RBC.BrokeragePlatform.Application.Features.AccountPositionsUpdated
{
    public class PositionUpdatedEvent : INotification
    {
        public int AccountId { get; init; }

        public decimal PriceChange { get; set; }    // TODO: have a collection of equities and changes, for now the change in price for the equity, used to calculate the new value of the position and determine if there are unrealized gains or losses
    }
}
