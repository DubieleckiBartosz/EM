using System;
using EventManagement.API.Common;
using EventManagement.API.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventManagement.Application.Configurations;
using EventManagement.Application.Contracts;
using EventManagement.Application.Settings;
using EventManagement.Infrastructure.Configurations;
using Hangfire;

namespace EventManagement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.GetHangfire(Configuration);
            services.GetMediatR();
            services.GetMapper();
            services.GetFluentValidatio n();
            services.GetApplicationDependencyInjection();
            services.GetInfrastructureDependencyInjection();
            services.GetJwtBearer(this.Configuration);
            services.AddControllers();
            services.GetSwaggerConfig();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            //var forwardedHeaderOptions = new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //};
            //forwardedHeaderOptions.KnownNetworks.Clear();
            //forwardedHeaderOptions.KnownProxies.Clear();
            //app.UseForwardedHeaders(forwardedHeaderOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventManagement.API v1"));
            }

            app.UseCustomExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHangfireDashboard("/hangfire-dashboard");

            serviceProvider.GetService<IBackgroundService>()?.StartJobs();
        }
    }
}
