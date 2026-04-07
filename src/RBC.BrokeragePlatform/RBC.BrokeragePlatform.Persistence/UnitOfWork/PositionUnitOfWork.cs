using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Application.Interfaces.UnitOfWorks;
using RBC.BrokeragePlatform.Domain.Entities;
using RBC.BrokeragePlatform.Persistence.Data;

namespace RBC.BrokeragePlatform.Persistence.UnitOfWork
{
    public class PositionUnitOfWork : IPositionUnitOfWork
    {
        private readonly BrokeragePlatformDbContext _context;
        private readonly IPositionRepository _positionRepository;

        public PositionUnitOfWork(BrokeragePlatformDbContext context, IPositionRepository positionRepository)
        {
            _context = context;
            _positionRepository = positionRepository;       // repository ensures the in-memory database is created and seeded
            //_context.MigrateInMemoryDatabase(); // Ensure the in-memory database is created and seeded
        }

        public void BeginTransaction()
        {
            //_context.Database.BeginTransaction();     // In-memory database does not support transactions, so this method is intentionally left blank.
        }

        public void CommitTransaction()
        {
            //_context.Database.CommitTransaction();        // In-memory database does not support transactions, so this method is intentionally left blank.
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<int> UpdatePositionsPriceAsync(int accountId, decimal priceChange, CancellationToken cancellationToken)
        {
            var positions = await _positionRepository.GetPositionsByAccountIdAsync(accountId, cancellationToken);

            var maxAverageCostPerShare = positions.Max(p => p.AverageCostPerShare);
            
            RecursivePriceChangeUpdate(positions, priceChange);

            _positionRepository.UpdatePositions(positions);
            
            return positions.Count();
        }

        private void RecursivePriceChangeUpdate(IEnumerable<Position> positions, decimal priceChange)
        {
            if(!positions.Any())
            {
                return;
            }

            if(positions.Count() == 1)
            {
                var position = positions.First();
                position.AverageCostPerShare += priceChange;
                return;
            }

            var separatorIndex = positions.Count() / 2;

            RecursivePriceChangeUpdate(positions.Take(separatorIndex), priceChange);
            RecursivePriceChangeUpdate(positions.Skip(separatorIndex + 1), priceChange);

        }
    }
}
