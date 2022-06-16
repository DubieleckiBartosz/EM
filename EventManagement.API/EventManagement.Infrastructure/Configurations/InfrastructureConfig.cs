using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Infrastructure.Configurations
{
    public static class InfrastructureConfig
    {
        public static IServiceCollection SetupDatabase(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services;
        }
    }
}