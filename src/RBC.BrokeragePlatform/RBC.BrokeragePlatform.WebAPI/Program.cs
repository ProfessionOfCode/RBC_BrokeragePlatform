using RBC.BrokeragePlatform.Application;
using RBC.BrokeragePlatform.Persistence;
using RBC.BrokeragePlatform.WebAPI.BackgroundServices;
using RBC.BrokeragePlatform.WebAPI.Hubs;
using RBC.BrokeragePlatform.WebAPI.Services;

namespace RBC.BrokeragePlatform.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddApplication();

            builder.Services.AddPersistence(builder.Configuration);

            // Add SignalR services
            builder.Services.AddSignalR();

            // Add scoped services
            builder.Services.AddScoped<IPositionPushService, PositionPushService>();

            // Add hosted background service
            builder.Services.AddHostedService<MarketUpdateSimulatorService>();

            // Add CORS for SignalR
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("SignalRPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Use CORS
            app.UseCors("SignalRPolicy");

            // Map SignalR hub
            app.MapHub<BrokerageHub>("/hubs/brokerage");

            app.MapControllers();

            app.Run();
        }
    }
}
