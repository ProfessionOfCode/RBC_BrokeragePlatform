using Microsoft.EntityFrameworkCore;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Domain.Entities;
using RBC.BrokeragePlatform.Persistence.Data;

namespace RBC.BrokeragePlatform.Persistence.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        private readonly BrokeragePlatformDbContext context;

        public AccountRepository(BrokeragePlatformDbContext context)
        {
            this.context = context;
            this.context.MigrateInMemoryDatabase(); // Ensure the in-memory database is created and seeded
        }
        public Account? GetAccountById(int accountId)
        {
           return context.Accounts.Find(accountId);
        }

        public IEnumerable<Account?> GetAllAccounts()
        {
            return [.. context.Accounts];
        }
        
        public async Task<IEnumerable<Account?>> GetAllAccountsAsync(CancellationToken cancellationToken)
        {
            return await context.Accounts.ToListAsync(cancellationToken);
        }
    }
}
