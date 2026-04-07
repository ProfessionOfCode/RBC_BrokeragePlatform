using RBC.BrokeragePlatform.Domain.Entities;

namespace RBC.BrokeragePlatform.Application.Interfaces.Repositories
{
    public interface IEquityRepository
    {
        Task<IEnumerable<Equity>> GetAllEquitiesAsync(CancellationToken cancellationToken);
        Equity? GetEquityById(int equityId);
        Task<IEnumerable<Equity>> GetEquitiesByIdsAsync(IEnumerable<int> equityIds, CancellationToken cancellationToken);
    }
}
