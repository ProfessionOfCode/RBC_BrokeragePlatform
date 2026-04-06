using MediatR;
using RBC.BrokeragePlatform.Application.Features.PlaceOrder;

namespace RBC.BrokeragePlatform.Application.Tests.Features.PlaceOrder
{
    public class PlaceOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsUnitValue_ForValidCommand()
        {
            var handler = new PlaceOrderCommandHandler();
            var command = new PlaceOrderCommand
            {
                AccountId = 1,
                Symbol = "AAPL",
                OrderType = 1,
                Quantity = 10,
                LimitPrice = 100
            };
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public async Task Handle_DoesNotThrow_ForValidCommand()
        {
            var handler = new PlaceOrderCommandHandler();
            var command = new PlaceOrderCommand
            {
                AccountId = 1,
                Symbol = "AAPL",
                OrderType = 1,
                Quantity = 10,
                LimitPrice = 100
            };
            var exception = await Record.ExceptionAsync(() => handler.Handle(command, CancellationToken.None));
            Assert.Null(exception);
        }
    }
}
