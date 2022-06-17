using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using EM.IntegrationTests.Setup;
using EM.IntegrationTests.Setup.Constants;
using EventManagement.API;
using EventManagement.Application.Features.EventFeatures.Commands.CreateEvent;
using EventManagement.Application.Features.EventFeatures.Queries.GetEventsBySearch;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Models.Enums;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EM.IntegrationTests.Controllers
{
    public class EventControllerTests : BaseSetup
    {
        private readonly DatabaseFixture _databaseFixture;

        public EventControllerTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
            this._databaseFixture = new DatabaseFixture();
        }

        [Fact]
        public async Task Should_Create_Event()
        {
            var startDate = DateTime.Now.AddDays(this.GetRandomInt(20, 50));
            var endDate = startDate.AddDays(1).ToLongDateString();
            var newEventTest = new CreateEventCommand("Event_Test", "Event_Description_Test",
                startDate.ToLongDateString(),
                endDate, PlaceType.Outdoors, this._fixture.Create<bool>(), "City_Test", "Street_Test",
                "123a", "0" + this.GetRandomInt() + "-" + this.GetRandomInt(1000, 1000), EventCategory.Massive,
                EventType.Modern);
            try
            {
                var responseMessage = await this.ClientCall(newEventTest, HttpMethod.Post, "api/Event/CreateEvent");
                var responseData = await this.ReadFromResponse<Response<int>>(responseMessage);
                Assert.True(responseData.Success);
                Assert.True(responseData.Data > 0);
            }
            finally
            {
                await this._databaseFixture.DeleteData("Events");
            }
        }


        [Fact]
        public async Task Should_Return_Event_By_Id()
        {
            var eventId = await this._databaseFixture.SetOnlyOneEvent();
            try
            {
                var responseMessage =
                    await this.ClientCall<object>(null, HttpMethod.Get, $"api/Event/GetEvent/{eventId}");
                var responseData = await this.ReadFromResponse<Response<EventDetailsDto>>(responseMessage);

                Assert.True(responseData.Success);
                Assert.NotNull(responseData.Data);
                Assert.Equal(eventId, responseData.Data.Id);
            }
            finally
            {
                await this._databaseFixture.DeleteData("Events");
            }
        }


        [Fact]
        public async Task Should_Return_EventWithApplications()
        {
            try
            {
                var users = await this._databaseFixture.UserSeedData();
            await this._databaseFixture.SetPerformers(users);
            var performers = (await this._databaseFixture.GetCurrentTestPerformers()).Select(_ => _.Id).ToList();
            var eventId = await this._databaseFixture.SetOnlyOneEvent();
            await this._databaseFixture.SetEventApplications(eventId, performers);
         
                var responseMessage = await this.ClientCall<object>(null, HttpMethod.Get,
                    $"api/Event/GetEventWithApplications/{eventId}");
                var responseData = await this.ReadFromResponse<Response<EventWithApplicationsDto>>(responseMessage);

                Assert.True(responseData.Success);
                Assert.NotNull(responseData.Data);
                Assert.Equal(performers.Count, responseData.Data.EventApplications.Count);
            }
            finally
            {
                await this._databaseFixture.DeleteData(null, Clear.Delete_EventApplications_Performers_Events_AppUsers);
            }
        }


        [Fact]
        public async Task Should_Return_Two_Events()
        {
            await this._databaseFixture.SetEvents();

            var request = new SearchEventsQuery()
            {
                PageSize = 2
            };

            try
            {
                var responseMessage = await this.ClientCall(request, HttpMethod.Post, "api/Event/GetEvents");
                var responseData = await this.ReadFromResponse<ResponseList<EventBaseDto>>(responseMessage);

                Assert.NotNull(responseData?.Data);
                Assert.Equal(2, responseData.Data.Count);
            }
            finally
            {
                await this._databaseFixture.DeleteData("Events");
            }
        }
    }
}