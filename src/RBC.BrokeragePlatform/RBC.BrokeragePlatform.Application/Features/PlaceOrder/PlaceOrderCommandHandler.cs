using MediatR;

namespace RBC.BrokeragePlatform.Application.Features.PlaceOrder
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, MediatR.Unit>
    {

        public PlaceOrderCommandHandler()
        {
            
        }

        public async Task<Unit> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement the logic to place an order based on the request parameters (Symbol, OrderType, Quantity, LimitPrice).
            // update the account balance and holdings accordingly.

            return await Task.FromResult(Unit.Value);
        }
    }
}
