using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EM.IntegrationTests.Setup;
using EM.IntegrationTests.Setup.Constants;
using EventManagement.API;
using EventManagement.Application.Features.EventApplicationFeatures.Commands.CreateEventApplication;
using EventManagement.Application.Features.EventApplicationFeatures.Queries.GetEventApplicationsBySearch;
using EventManagement.Application.Models.Authorization;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Models.Enums;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EM.IntegrationTests.Controllers
{
    public class EventApplicationControllerTests : BaseSetup
    {
        private readonly DatabaseFixture _databaseFixture;

        public EventApplicationControllerTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
            this._databaseFixture = new DatabaseFixture();
        }

        [Fact]
        public async Task Should_Return_Some_Applications_By_Search()
        {
            try
            {
                var query = new GetEventApplicationsQuery();
                var users = await this._databaseFixture.UserSeedData();
                await this._databaseFixture.SetPerformers(users);
                var performers = (await this._databaseFixture.GetCurrentTestPerformers()).Select(_ => _.Id).ToList();
                var eventId = await this._databaseFixture.SetOnlyOneEvent();
                await this._databaseFixture.SetEventApplications(eventId, performers);

                var responseMessage = await this.ClientCall(query, HttpMethod.Post,
                    $"api/EventApplication/GetEventApplicationsBySearch");
                var responseData = await this.ReadFromResponse<Response<List<EventApplicationDto>>>(responseMessage);

                Assert.True(responseData.Success);
                Assert.NotNull(responseData.Data);
                Assert.Equal(performers.Count, responseData.Data.Count);
            }
            finally
            {
                await this._databaseFixture.DeleteData(null, Clear.Delete_EventApplications_Performers_Events_AppUsers);
            }
        }


        [Fact]
        public async Task Should_Create_EventApplication()
        {
            try
            {
                await this._databaseFixture.SetPerformer(1);
                var eventId = await this._databaseFixture.SetOnlyOneEvent();
                var request = new CreateApplicationCommand(eventId, TypePerformance.Acrobatics, 60);

                var responseMessage = await this.ClientCall(request, HttpMethod.Post,
                    $"api/EventApplication/CreateEventApplication");
                var responseData = await this.ReadFromResponse<Response<string>>(responseMessage);

                Assert.True(responseData.Success);
            }
            finally
            {
                await this._databaseFixture.DeleteData(null, Clear.Delete_EventApplications_Performers_Events_AppUsers);
            }
        }
    }
}
