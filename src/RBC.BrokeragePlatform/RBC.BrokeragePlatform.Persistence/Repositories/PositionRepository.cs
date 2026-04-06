using Microsoft.EntityFrameworkCore;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Domain.Entities;
using RBC.BrokeragePlatform.Persistence.Data;

namespace RBC.BrokeragePlatform.Persistence.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly BrokeragePlatformDbContext _context;

        public PositionRepository(BrokeragePlatformDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Position> GetPositionsByAccountId(int accountId)
        {
            return _context.Positions.Where(p => p.AccountId == accountId).ToList();
        }

        public async Task<IEnumerable<Position>> GetPositionsByAccountIdAsync(int accountId, CancellationToken cancellationToken = default)
        {
            return await _context.Positions
                .Where(p => p.AccountId == accountId)
                .ToListAsync(cancellationToken);
        }

        public void UpdatePositions(IEnumerable<Position> positions)
        {
            _context.Positions.UpdateRange(positions);            
        }
    }
}
