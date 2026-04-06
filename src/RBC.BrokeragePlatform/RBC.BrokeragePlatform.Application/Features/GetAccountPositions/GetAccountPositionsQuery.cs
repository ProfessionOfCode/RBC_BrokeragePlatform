using MediatR;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.Application.Features.GetAccountPositions
{
    public class GetAccountPositionsQuery : IRequest<IEnumerable<PositionDto>>
    {
        public int AccountId { get; set; }

        public GetAccountPositionsQuery(int accountId)
        {
            AccountId = accountId;
        }
    }
}
