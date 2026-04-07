using Microsoft.EntityFrameworkCore;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Domain.Entities;
using RBC.BrokeragePlatform.Persistence.Data;

namespace RBC.BrokeragePlatform.Persistence.Repositories
{
    public class EquityRepository : IEquityRepository
    {
        private readonly BrokeragePlatformDbContext _context;

        public EquityRepository(BrokeragePlatformDbContext context)
        {
            _context = context;
            _context.MigrateInMemoryDatabase();
        }
        public async Task<IEnumerable<Equity>> GetAllEquitiesAsync(CancellationToken cancellationToken)
        {
            return await _context.Equities.ToListAsync(cancellationToken);
        }

        public Equity? GetEquityById(int equityId)
        {
            return _context.Equities.FirstOrDefault(e => e.EquityId == equityId);
        }

        public async Task<IEnumerable<Equity>> GetEquitiesByIdsAsync(IEnumerable<int> equityIds, CancellationToken cancellationToken)
        {
            return await _context.Equities.Where(e => equityIds.Contains(e.EquityId)).ToListAsync(cancellationToken);
        }
    }
}
