using System;
using System.Reflection;
using System.Text;
using EventManagement.Application.Decorators;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Base;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EventManagement.Application.Configurations
{
    public static class ApplicationConfig
    {
        public static IServiceCollection GetMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }

        public static IServiceCollection GetMediatR(this IServiceCollection services)
        {
            services.AddTransient<IDomainDecorator, MediatRDecorator>();
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }

        public static IServiceCollection GetFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }

        public static IServiceCollection GetHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
                x.UseMediatR();
            });
            
            services.AddHangfireServer();

            return services;
        }

        public static IServiceCollection GetJwtBearer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                    o.Events = new JwtBearerEvents()
                    {
                        OnChallenge = async (context) =>
                        {
                            if (context.AuthenticateFailure != null)
                            {
                                var error = string.IsNullOrEmpty(context.ErrorDescription)
                                    ? context.AuthenticateFailure?.Message
                                    : context.ErrorDescription;
                                context.HandleResponse();
                                context.Response.ContentType = "application/json";
                                var statusCode = context?.AuthenticateFailure is SecurityTokenExpiredException
                                    ? 403
                                    : 401;
                                context.Response.StatusCode = statusCode;
                                await context.Response.WriteAsJsonAsync(Response<string>.Error(statusCode == 403
                                    ? $"403 Expired token: {error}"
                                    : $"401 Not authorized: {error}"));
                            }
                        }
                    };
                });
            return services;
        }
    }
}
