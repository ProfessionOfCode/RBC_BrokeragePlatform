using RBC.BrokeragePlatform.Domain.Entities;

namespace RBC.BrokeragePlatform.Application.Interfaces.Repositories
{
    public interface IPositionRepository
    {
        IEnumerable<Position> GetPositionsByAccountId(int accountId);
        Task<IEnumerable<Position>> GetPositionsByAccountIdAsync(int accountId, CancellationToken cancellationToken = default);
        void UpdatePositions(IEnumerable<Position> positions);
    }
}
