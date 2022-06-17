using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EM.IntegrationTests.Common;
using EventManagement.API;
using EventManagement.Application.Settings;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

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

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                });
              

            }).CreateClient();
        }

        protected JsonSerializerSettings SerializerSettings() => new JsonSerializerSettings
        {
            ContractResolver = new PrivateResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore
        };

        protected int GetRandomInt(int a = 1, int b = 10) => new Random().Next(a, b);

        protected void SetupConnectionDb()
        {
            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock
                .SetupGet(_ => _[It.Is<string>(s => s == "DefaultConnection")])
                .Returns(Connection);
        }

        protected async Task<HttpResponseMessage> ClientCall<TRequest>(TRequest obj, HttpMethod methodType, string requestUri)
        {
            var request = new HttpRequestMessage(methodType, requestUri);
            if (obj != null)
            {
                var serializeObject = JsonConvert.SerializeObject(obj);
                request.Content = new StringContent(serializeObject);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return await this._client.SendAsync(request);
        }


        protected async Task<TResponse> ReadFromResponse<TResponse>(HttpResponseMessage response)
        {
            var contentString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(contentString, this.SerializerSettings());
        }
    }
}