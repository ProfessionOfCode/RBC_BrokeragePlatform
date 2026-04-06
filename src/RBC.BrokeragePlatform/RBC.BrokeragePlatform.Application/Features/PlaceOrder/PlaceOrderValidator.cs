using FluentValidation;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.SharedCore.Enums;

namespace RBC.BrokeragePlatform.Application.Features.PlaceOrder
{
    public class PlaceOrderValidator : AbstractValidator<PlaceOrderCommand>
    {

        public PlaceOrderValidator(IAccountRepository accountRepository)
        {
            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("AccountId must be greater than zero.")
                .MustAsync(async (accountId, cancellation) =>
                {
                    var accountExists = accountRepository.GetAccountById(accountId);
                    return accountExists != null;
                }).WithMessage((order) => $"Account with the specified AccountId {order.AccountId} does not exist.");


            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Symbol is required.")
                .MaximumLength(10).WithMessage("Symbol cannot exceed 10 characters.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.LimitPrice)
                .GreaterThan(0).WithMessage("LimitPrice must be greater than zero.");

            RuleFor(x=>x)
                .Must((order) =>
                {
                    // Example: For BUY orders, LimitPrice should not exceed a certain threshold (e.g., $1000)
                    var account = accountRepository.GetAccountById(order.AccountId);

                    return account != null && account.CashBalance >= order.LimitPrice * order.Quantity;                                           
                })
                .When(x => x.OrderType == (int)OrderTypeEnum.BUY);
        }

    }
}
