using MediatR;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.Application.Features.GetAllAccounts
{
    public class GetAllAccountsQuery: IRequest<IEnumerable<AccountDto>>
    {
    }
}
