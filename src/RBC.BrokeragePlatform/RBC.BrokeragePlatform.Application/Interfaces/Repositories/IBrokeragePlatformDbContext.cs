namespace RBC.BrokeragePlatform.Application.Interfaces.Repositories
{
    public interface IBrokeragePlatformDbContext
    {        
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void MigrateInMemoryDatabase();
    }
}
