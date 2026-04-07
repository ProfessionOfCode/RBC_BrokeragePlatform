using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RBC.BrokeragePlatform.Application.Interfaces.Mapping;
using RBC.BrokeragePlatform.Application.Mapping;

namespace RBC.BrokeragePlatform.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR handlers
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            // Register custom mapper
            services.AddSingleton<IMapper, Mapper>();

            // Register FluentValidation
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            
            return services;
        }
    }
}
