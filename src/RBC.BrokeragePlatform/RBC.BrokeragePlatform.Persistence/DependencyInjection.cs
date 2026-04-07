using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RBC.BrokeragePlatform.Application.Interfaces.Repositories;
using RBC.BrokeragePlatform.Application.Interfaces.UnitOfWorks;
using RBC.BrokeragePlatform.Domain.Entities;
using RBC.BrokeragePlatform.Persistence.Data;

namespace RBC.BrokeragePlatform.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BrokeragePlatformDbContext>(options =>
            {
                options.UseInMemoryDatabase("BrokeragePlatformDb")
                    .UseSeeding((context, _) =>
                    {
                        SeedBrokerageData(context);
                    });
            });

            services.AddScoped<IBrokeragePlatformDbContext>(provider => provider.GetRequiredService<BrokeragePlatformDbContext>());

            services.AddScoped<IAccountRepository, Repositories.AccountRepository>();
            services.AddScoped<IPositionRepository, Repositories.PositionRepository>();
            services.AddScoped<IEquityRepository, Repositories.EquityRepository>();
            services.AddScoped<IPositionUnitOfWork, UnitOfWork.PositionUnitOfWork>();

            // Add other repositories here as needed

            return services;
        }

        private static void SeedBrokerageData(DbContext context)
        {
            int seedingCount = 0;
            var stockSymbols = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "FB", "TSLA", "NVDA", "JPM", "V", "DIS", "TSM", "META", "XOM", "ORCL", "MA", "WMT", "NFLX" };

            var random = new Random();

            var accounts = Enumerable.Range(10000, 5).Select(i => new Account
            {
                AccountId = i,
                ClientName = $"Client {i}",
                AccountNumber = $"ACC{10000+i}",
                CashBalance = 500 + i * random.Next(15, 25) * 100
            }).Distinct().ToList();

            foreach (var account in accounts)
            {
                if (!context.Set<Account>().Any(a => a.AccountId == account.AccountId))
                {
                    context.Set<Account>().Add(account);
                    seedingCount++;
                }
            }            

            var equities = Enumerable.Range(1, 15).Select(i => new Equity
            {
                EquityId = i,
                Symbol = stockSymbols[i],
                CurrentPrice = 50 + i * 12
            }).Distinct().ToList();            
            

            foreach (var equity in equities)
            {
                if (!context.Set<Equity>().Any(a => a.EquityId == equity.EquityId)
                    && !context.Set<Equity>().Any(a => a.Symbol.ToLower() == equity.Symbol.ToLower()))
                {
                    context.Set<Equity>().Add(equity);
                    seedingCount++;
                }                
            }
     


            var rnd = new Random();
            var positions = GenerateUniquePositionsPerAccountEquities(accounts, equities, 20, rnd);

            foreach (var position in positions)
            {
                if (!context.Set<Position>().Any(a => a.PositionId == position.PositionId)
                    && !context.Set<Position>().Any(p => p.AccountId == position.AccountId && p.EquityId == position.EquityId)
                    )
                {
                    context.Set<Position>().Add(position);
                    seedingCount++;
                }
            }

            if(seedingCount > 0)
            {
                context.SaveChanges();
            }
        }

        private static IEnumerable<Position> GenerateUniquePositionsPerAccountEquities(List<Account> accounts, List<Equity> equities, int maximumNumberOfPositionsPerAccount, Random rnd)
        {
            if(accounts == null || equities == null || accounts.Count == 0 || equities.Count == 0)
            {
                yield break;
            }            

            foreach (var account in accounts) 
            {
                var numberOfPositionsForAccount = rnd.Next(1, Math.Min(maximumNumberOfPositionsPerAccount, equities.Count) + 1);
                for (int i = 0; i < numberOfPositionsForAccount; i++)
                {
                    var equity = equities[i];
                    yield return new Position
                    {
                        PositionId = account.AccountId * 100 + equity.EquityId,
                        AccountId = account.AccountId,
                        EquityId = equity.EquityId,
                        Symbol = equity.Symbol,
                        Quantity = rnd.Next(1, 100),
                        AverageCostPerShare = equity.CurrentPrice - rnd.Next(1, 10)
                    };

                }
            }
        }

    }
}
