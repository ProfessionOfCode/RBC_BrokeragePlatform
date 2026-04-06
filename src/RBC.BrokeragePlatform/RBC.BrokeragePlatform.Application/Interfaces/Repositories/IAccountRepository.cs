using RBC.BrokeragePlatform.Domain.Entities;

namespace RBC.BrokeragePlatform.Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account?> GetAllAccounts();
        Task<IEnumerable<Account?>> GetAllAccountsAsync(CancellationToken cancellationToken);
        Account? GetAccountById(int accountId);
    }
}
