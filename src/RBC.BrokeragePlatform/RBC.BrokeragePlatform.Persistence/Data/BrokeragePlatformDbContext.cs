using Microsoft.EntityFrameworkCore;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Domain.Entities;


namespace RBC.BrokeragePlatform.Persistence.Data
{
    public class BrokeragePlatformDbContext : DbContext, IBrokeragePlatformDbContext
    {
        public BrokeragePlatformDbContext(DbContextOptions<BrokeragePlatformDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Equity> Equities => Set<Equity>();
        public DbSet<Position> Positions => Set<Position>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>().HasKey(a => a.AccountId);
            modelBuilder.Entity<Equity>().HasKey(e => e.EquityId);
            modelBuilder.Entity<Position>().HasKey(p => p.PositionId);
        }

        public void MigrateInMemoryDatabase()
        {
            // In-memory database does not support migrations, so this method is intentionally left blank.
            // In a real implementation with a relational database, you would call Database.Migrate() here.            
            this.Database.EnsureCreated(); // Ensure the in-memory database is created
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
