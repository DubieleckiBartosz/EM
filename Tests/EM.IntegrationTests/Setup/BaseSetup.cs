﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
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
using Newtonsoft.Json;
using Xunit;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace EM.IntegrationTests.Setup
{
    public abstract class BaseSetup : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string CombinePath = "appsettings.json";
        protected HttpClient _client;
        protected Fixture _fixture;

        //private const string Connection = "Server=host.docker.internal,1440;Database=EventManagementTests;User Id=sa;Password=Password_123BD;";
        protected BaseSetup(WebApplicationFactory<Startup> factory)
        {
            this._fixture = new Fixture();
            this._client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    var projectDir = Directory.GetCurrentDirectory();
                    var configPath = Path.Combine(projectDir, CombinePath);
                    conf.AddJsonFile(configPath);
                });
                builder.ConfigureServices(services =>
                {
                    var storage = services.SingleOrDefault(service => service.ServiceType == typeof(SqlServerStorage));
                    services.Remove(storage);
                    services.AddHangfire(c => c.UseMemoryStorage());

                    //services.Configure<ConnectionStrings>(opts =>
                    //{
                    //    opts.DefaultConnection = Connection;
                    //});

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.AddMvc(_ => _.Filters.Add(new FakeUserActionFilter()));
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

        protected async Task<HttpResponseMessage> ClientCall<TRequest>(TRequest obj, HttpMethod methodType,
            string requestUri)
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