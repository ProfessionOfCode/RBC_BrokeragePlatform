using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Application.Interfaces.UnitOfWorks;
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

            // update the positions based on the price change
            foreach (var position in positions.Reverse().Take(positions.Count()/3))
            {
                position.AverageCostPerShare += priceChange; // Example of updating the price
            }

            _positionRepository.UpdatePositions(positions);
            
            return positions.Count();
        }
    }
}
