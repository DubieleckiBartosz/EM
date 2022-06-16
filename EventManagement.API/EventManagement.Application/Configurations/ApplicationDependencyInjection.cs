using EventManagement.Application.Behaviours;
using EventManagement.Application.Contracts;
using EventManagement.Application.Models.Authorization;
using EventManagement.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Application.Configurations
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection GetApplicationDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient(typeof(ILoggerManager<>), typeof(LoggerManager<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            services.AddScoped<IFileService, FileService>();   
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBackgroundService, BackgroundService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}