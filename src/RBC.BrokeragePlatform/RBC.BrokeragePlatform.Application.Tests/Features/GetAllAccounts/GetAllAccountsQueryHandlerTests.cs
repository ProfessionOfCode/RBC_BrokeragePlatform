using Microsoft.Extensions.Logging;
using Moq;
using RBC.BrokeragePlatform.Application.Features.GetAllAccounts;
using RBC.BrokeragePlatform.Application.Interfaces.Mapping;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Domain.Entities;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.Application.Tests.Features.GetAllAccounts
{
    public class GetAllAccountsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsMappedAccounts_WhenRepositoryReturnsAccounts()
        {
            var mockRepo = new Mock<IAccountRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<GetAllAccountsQueryHandler>>();
            var accounts = new List<Account> { new () };
            var dtos = new List<AccountDto> { new AccountDto{
                AccountId = 1,
                ClientName = "TAMKO", 
                AccountNumber = "ACC1001", 
                CashBalance = 150000m 
            } };
            mockRepo.Setup(r => r.GetAllAccountsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(accounts);
            mockMapper.Setup(m => m.MapCollection<Account, AccountDto>(accounts)).Returns(dtos);
            var handler = new GetAllAccountsQueryHandler(mockRepo.Object, mockMapper.Object, mockLogger.Object);
            var result = await handler.Handle(new GetAllAccountsQuery(), CancellationToken.None);
            Assert.Single(result);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IAccountRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<GetAllAccountsQueryHandler>>();
            mockRepo.Setup(r => r.GetAllAccountsAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception());
            var handler = new GetAllAccountsQueryHandler(mockRepo.Object, mockMapper.Object, mockLogger.Object);
            var result = await handler.Handle(new GetAllAccountsQuery(), CancellationToken.None);
            Assert.Empty(result);
        }
    }
}
