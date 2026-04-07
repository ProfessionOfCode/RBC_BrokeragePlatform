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
                //.UseAsyncSeeding(async (context, _, cancellationToken) => 
                //    { 
                //        await SeedBrokerageDataAsync(context);
                //    });       Is seeded when EnsureCreatedAsync is called on the in-memory database.
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
            var stockSymbols = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "FB", "TSLA", "NVDA", "JPM", "V", "DIS", "TSM", "META", "XOM", "ORCL", "MA" };

            var accounts = Enumerable.Range(1, 5).Select(i => new Account
            {
                AccountId = i,
                ClientName = $"Client {i}",
                AccountNumber = $"ACC{1000+i}",
                CashBalance = 10000 + i * 1000
            }).Distinct().ToList();

            // make sure to check if data already exists to avoid duplicate seeding
            foreach (var account in accounts)
            {
                if (!context.Set<Account>().Any(a => a.AccountId == account.AccountId))
                {
                    context.Set<Account>().Add(account);
                    seedingCount++;
                }
            }            

            var equities = Enumerable.Range(1, 12).Select(i => new Equity
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
            var positions = Enumerable.Range(10, 35).Select(i =>
            {
                var account = accounts[rnd.Next(accounts.Count)];
                var equity = equities[rnd.Next(equities.Count)];
                return new Position
                {
                    PositionId = i,
                    AccountId = account.AccountId,
                    EquityId = equity.EquityId,
                    Symbol = equity.Symbol,
                    Quantity = rnd.Next(5, 100),
                    AverageCostPerShare = equity.CurrentPrice - rnd.Next(1, 10)
                };
            }).ToList();

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

        // The async version of seeding is provided to demonstrate how you can seed data asynchronously if needed.
        //private async static Task SeedBrokerageDataAsync(DbContext context)
        //{
        //    int seedingCount = 0;
        //    var accounts = Enumerable.Range(1, 5).Select(i => new Account
        //    {
        //        AccountId = i,
        //        ClientName = $"Client {i}",
        //        AccountNumber = 1000 + i,
        //        CashBalance = 10000 + i * 1000
        //    }).ToList();

        //    foreach (var account in accounts)
        //    {
        //        if (!context.Set<Account>().Any(a => a.AccountId == account.AccountId))
        //        {
        //            await context.Set<Account>().AddAsync(account);
        //            seedingCount++;
        //        }
        //    }

        //    var equities = Enumerable.Range(1, 10).Select(i => new Equity
        //    {
        //        EquityId = i,
        //        Symbol = $"EQ{i:000}",
        //        CurrentPrice = 50 + i * 10
        //    }).ToList();

        //    foreach (var equity in equities)
        //    {
        //        if (!context.Set<Equity>().Any(a => a.EquityId == equity.EquityId))
        //        {
        //            await context.Set<Equity>().AddAsync(equity);
        //            seedingCount++;
        //        }
        //    }

        //    var rnd = new Random();
        //    var positions = Enumerable.Range(1, 20).Select(i =>
        //    {
        //        var account = accounts[rnd.Next(accounts.Count)];
        //        var equity = equities[rnd.Next(equities.Count)];
        //        return new Position
        //        {
        //            PositionId = i,
        //            AccountId = account.AccountId,
        //            EquityId = equity.EquityId,
        //            Symbol = equity.Symbol,
        //            Quantity = rnd.Next(1, 100),
        //            AverageCostPerShare = equity.CurrentPrice - rnd.Next(1, 10)
        //        };
        //    }).ToList();

        //    foreach (var position in positions)
        //    {
        //        if (!context.Set<Position>().Any(a => a.PositionId == position.PositionId))
        //        {
        //            await context.Set<Position>().AddAsync(position);
        //            seedingCount++;
        //        }
        //    }

        //    if(seedingCount > 0)
        //    {
        //        await context.SaveChangesAsync();
        //    }                
        //}

    }
}
