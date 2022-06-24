using System;
using EventManagement.Application.Configurations;
using EventManagement.Application.Settings;
using EventManagement.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace EventManagement.API.Configurations
{
    public static class AppConfig
    {
        public static IServiceCollection GetLayersConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.GetCache(configuration);
            services.AddHttpContextAccessor();
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));
            services.Configure<RedisConnection>(configuration.GetSection("EMRedisCacheSettings"));
            services.GetHangfire(configuration);
            services.GetMediatR();
            services.GetMapper();
            services.GetFluentValidation();
            services.GetApplicationDependencyInjection();
            services.GetInfrastructureDependencyInjection();
            services.GetJwtBearer(configuration);

            return services;
        }

        public static IServiceCollection GetCache(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheSettings = new RedisConnection();
            configuration.GetSection(RedisConnection.EMRedisSection).Bind(cacheSettings);

            if (!cacheSettings.Enabled)
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheSettings.EMRedisConnection;
                    options.InstanceName = "EM";
                });
            }

            return services;
        }

        public static void GetSwaggerConfig(this IServiceCollection services)
        {
         
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Event Management API",
                    Version = "v1",
                    Description = "ASP.NET Core 5.0 Web API",
                    Contact = new OpenApiContact
                    {
                    Name = "Github",
                    Url = new Uri("https://github.com/DubieleckiBartosz"),
                }
                });
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "Enter JWT Bearer token",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
    
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
            });
        }
    }
}
