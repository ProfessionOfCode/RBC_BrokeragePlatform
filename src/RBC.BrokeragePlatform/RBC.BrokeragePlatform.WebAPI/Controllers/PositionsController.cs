using MediatR;
using Microsoft.AspNetCore.Mvc;
using RBC.BrokeragePlatform.Application.Features.GetAccountPositions;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PositionsController> _logger;

        public PositionsController(IMediator mediator, ILogger<PositionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{accountId}")]
        public async Task<ActionResult<IEnumerable<PositionDto>>> GetPositionsByAccountId(int accountId)
        {
            _logger.LogInformation("HTTP request received - GET /api/positions/{AccountId}", accountId);

            try
            {
                var positions = await _mediator.Send(new GetAccountPositionsQuery(accountId));
                var positionList = positions.ToList();

                _logger.LogInformation("HTTP response sent - GET /api/positions/{AccountId} - PositionCount: {PositionCount}", 
                    accountId, positionList.Count);

                return Ok(positionList);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error handling GET /api/positions/{AccountId} - AccountId: {AccountId}, Exception: {ExceptionMessage}", 
                    accountId, accountId, ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
