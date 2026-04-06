using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RBC.BrokeragePlatform.Application.Features.GetAllAccounts;
using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.WebAPI.Controllers;

namespace RBC.BrokeragePlatform.WebAPI.Tests.Controllers
{
    public class AccountsControllerTests
    {
        [Fact]
        public async Task GetAllAccounts_ReturnsOkWithAccounts()
        {
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<AccountsController>>();
            var accounts = new List<AccountDto> { new AccountDto{
                AccountId = 1,
                ClientName = "TAMKO",
                AccountNumber = "ACC1001",
                CashBalance = 150000m
            }};
            mockMediator.Setup(m => m.Send(It.IsAny<GetAllAccountsQuery>(), default)).ReturnsAsync(accounts);
            var controller = new AccountsController(mockMediator.Object, mockLogger.Object);

            var result = await controller.GetAllAccounts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAccounts = Assert.IsAssignableFrom<IEnumerable<AccountDto>>(okResult.Value);
            Assert.Single(returnedAccounts);
        }

        [Fact]
        public async Task GetAccountById_ReturnsNotFoundWhenAccountDoesNotExist()
        {
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<AccountsController>>();
            mockMediator.Setup(m => m.Send(It.IsAny<GetAllAccountsQuery>(), default)).ReturnsAsync(new List<AccountDto>());
            var controller = new AccountsController(mockMediator.Object, mockLogger.Object);

            var result = await controller.GetAccountById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
