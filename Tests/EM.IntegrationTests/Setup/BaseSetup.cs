using System;
using System.Linq;
using System.Net.Http;
using EventManagement.API;
using EventManagement.Application.Settings;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace EM.IntegrationTests.Setup
{
    public abstract class BaseSetup : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected HttpClient _client;

        private const string Connection = "Server=host.docker.internal,1440;Database=EventManagementTests;User Id=sa;Password=Password_123BD;";
        protected BaseSetup(WebApplicationFactory<Startup> factory)
        {
            this._client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var storage = services.SingleOrDefault(service => service.ServiceType == typeof(SqlServerStorage));
                    services.Remove(storage);
                    services.AddHangfire(c => c.UseMemoryStorage());

                    services.Configure<ConnectionStrings>(opts =>
                    {
                        opts.DefaultConnection = Connection;
                    });

                });
            }).CreateClient();
        }

        protected int GetRandomInt(int a = 1, int b = 10) => new Random().Next(a, b);

        protected void SetupConnectionDb()
        {
            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock
                .SetupGet(_ => _[It.Is<string>(s => s == "DefaultConnection")])
                .Returns(Connection);
        }
    }
}