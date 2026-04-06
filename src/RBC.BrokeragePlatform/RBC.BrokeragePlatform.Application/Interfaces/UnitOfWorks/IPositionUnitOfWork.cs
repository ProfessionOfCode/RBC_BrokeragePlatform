namespace RBC.BrokeragePlatform.Application.Interfaces.UnitOfWorks
{
    public interface IPositionUnitOfWork
    {
        void BeginTransaction();

        void CommitTransaction();

        Task<int> UpdatePositionsPriceAsync(int accountNumber, decimal priceChange, CancellationToken cancellationToken);

        void SaveChanges();
    }
}
