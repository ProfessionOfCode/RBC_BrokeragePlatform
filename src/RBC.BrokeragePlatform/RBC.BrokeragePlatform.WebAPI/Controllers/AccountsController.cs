using MediatR;
using Microsoft.AspNetCore.Mvc;
using RBC.BrokeragePlatform.Application.Features.GetAllAccounts;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAccounts()
        {
            _logger.LogInformation("HTTP request received - GET /api/accounts");

            try
            {
                var accounts = await _mediator.Send(new GetAllAccountsQuery());
                var accountList = accounts.ToList();

                _logger.LogInformation("HTTP response sent - GET /api/accounts - AccountCount: {AccountCount}", accountList.Count);

                return Ok(accountList);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error handling GET /api/accounts - Exception: {ExceptionMessage}", ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetAccountById(int id)
        {
            _logger.LogInformation("HTTP request received - GET /api/accounts/{Id}", id);

            try
            {
                var accounts = await _mediator.Send(new GetAllAccountsQuery());
                var account = accounts.FirstOrDefault(a => a.AccountId == id);

                if (account == null)
                {
                    _logger.LogInformation("Account not found - AccountId: {AccountId}", id);
                    return NotFound();
                }

                _logger.LogInformation("HTTP response sent - GET /api/accounts/{Id} - AccountId: {AccountId}", id, account.AccountId);

                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error handling GET /api/accounts/{Id} - AccountId: {AccountId}, Exception: {ExceptionMessage}", 
                    id, id, ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
