using EventManagement.Application.Contracts;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Infrastructure.Database;
using EventManagement.Infrastructure.EventProcess;
using EventManagement.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Infrastructure.Configurations
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection GetInfrastructureDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IOpinionRepository, OpinionRepository>();
            services.AddScoped<IPerformerRepository, PerformerRepository>();
            services.AddScoped<IPerformanceProposalRepository, PerformanceProposalRepository>();
            services.AddScoped<IEventImageRepository, EventImageRepository>();
            services.AddScoped<IEventApplicationRepository, EventApplicationRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();  
            services.AddSingleton<ITransaction, TransactionSupervisor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<EventContext>();

            return services;
        }
    }
}