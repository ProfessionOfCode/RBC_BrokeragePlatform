using FluentValidation.TestHelper;
using Moq;
using RBC.BrokeragePlatform.Application.Features.PlaceOrder;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Domain.Entities;

namespace RBC.BrokeragePlatform.Application.Tests.Features.PlaceOrder
{
    public class PlaceOrderValidatorTests
    {
        [Fact]
        public async Task Validator_Passes_ForValidCommand()
        {
            var mockRepo = new Mock<IAccountRepository>();
            mockRepo.Setup(r => r.GetAccountById(It.IsAny<int>())).Returns(new Account{ CashBalance = 10000m });
            var validator = new PlaceOrderValidator(mockRepo.Object);
            var command = new PlaceOrderCommand
            {
                AccountId = 1,
                Symbol = "AAPL",
                OrderType = 1,
                Quantity = 10,
                LimitPrice = 100
            };
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validator_Fails_ForNegativeQuantity()
        {
            var mockRepo = new Mock<IAccountRepository>();
            mockRepo.Setup(r => r.GetAccountById(It.IsAny<int>())).Returns(new Account{ CashBalance = 10000m });
            var validator = new PlaceOrderValidator(mockRepo.Object);
            var command = new PlaceOrderCommand
            {
                AccountId = 1,
                Symbol = "AAPL",
                OrderType = 1,
                Quantity = -5,
                LimitPrice = 100
            };
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public async Task Validator_Fails_ForCashBalanceLessThanQuantityTimesLimitPrice()
        {
            var mockRepo = new Mock<IAccountRepository>();
            mockRepo.Setup(r => r.GetAccountById(It.IsAny<int>())).Returns(new Account { CashBalance = 100m });
            var validator = new PlaceOrderValidator(mockRepo.Object);
            var command = new PlaceOrderCommand
            {
                AccountId = 1,
                Symbol = "AAPL",
                OrderType = 1,      // BUY order
                Quantity = 5000,
                LimitPrice = 10000
            };
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x);
        }
    }
}
