using MediatR;
using Microsoft.AspNetCore.Mvc;
using RBC.BrokeragePlatform.Application.Features.PlaceOrder;

namespace RBC.BrokeragePlatform.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<string>> PlaceOrder([FromBody] PlaceOrderCommand command)
        {
            _logger.LogInformation("HTTP request received - POST /api/orders - AccountId: {AccountId}, Symbol: {Symbol}, OrderType: {OrderType}, Quantity: {Quantity}, LimitPrice: {LimitPrice}", 
                command.AccountId, command.Symbol, command.OrderType, command.Quantity, command.LimitPrice);

            try
            {
                await _mediator.Send(command);

                _logger.LogInformation("HTTP response sent - POST /api/orders - Order placed successfully for AccountId: {AccountId}", command.AccountId);

                return Ok("Order placed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error handling POST /api/orders - AccountId: {AccountId}, Exception: {ExceptionMessage}", 
                    command.AccountId, ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
