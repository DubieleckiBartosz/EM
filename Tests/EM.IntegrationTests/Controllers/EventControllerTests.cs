using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using EM.IntegrationTests.Setup;
using EM.IntegrationTests.Setup.Constants;
using EventManagement.API;
using EventManagement.Application.Features.EventFeatures.Commands.CancelEvent;
using EventManagement.Application.Features.EventFeatures.Commands.ChangeVisibilityEvent;
using EventManagement.Application.Features.EventFeatures.Commands.CreateEvent;
using EventManagement.Application.Features.EventFeatures.Commands.UpdateEvent;
using EventManagement.Application.Features.EventFeatures.Queries.GetEventsBySearch;
using EventManagement.Application.Features.EventFeatures.Queries.GetEventWithOpinions;
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

            var query = new SearchEventsQuery()
            {
                PageSize = 2
            };

            try
            {
                var responseMessage = await this.ClientCall(query, HttpMethod.Post, "api/Event/GetEvents");
                var responseData = await this.ReadFromResponse<ResponseList<EventBaseDto>>(responseMessage);

                Assert.NotNull(responseData?.Data);
                Assert.Equal(2, responseData.Data.Count);
            }
            finally
            {
                await this._databaseFixture.DeleteData("Events");
            }
        }

        [Fact]
        public async Task Should_Return_Event_Without_Opinions()
        {
            var eventId = await this._databaseFixture.SetOnlyOneEvent();
            var query = new GetEventWithOpinionsQuery(eventId, null);
            try
            {
                var responseMessage = await this.ClientCall(query, HttpMethod.Post, "api/Event/GetEventWithOpinions");
                var responseData = await this.ReadFromResponse<Response<EventWithOpinionsDto>>(responseMessage);

                Assert.NotNull(responseData?.Data);
                Assert.Empty(responseData.Data.Opinions);
                Assert.Equal(eventId, responseData.Data.Id);
            }
            finally
            {
                await this._databaseFixture.DeleteData("Events");
            }
        }


        [Fact]
        public async Task Should_Return_Event_With_Opinions()
        {
            var count = this.GetRandomInt();
            var eventId = await this._databaseFixture.SetOnlyOneEvent();
            await this._databaseFixture.SetOpinions(eventId, count);

            var query = new GetEventWithOpinionsQuery(eventId, null);
            try
            {
                var responseMessage = await this.ClientCall(query, HttpMethod.Post, "api/Event/GetEventWithOpinions");
                var responseData = await this.ReadFromResponse<Response<EventWithOpinionsDto>>(responseMessage);

                Assert.NotNull(responseData?.Data);
                Assert.NotEmpty(responseData.Data.Opinions);
                Assert.Equal(eventId, responseData.Data.Id);
                Assert.Equal(count, responseData.Data.Opinions.Count);
            }
            finally
            {
                await this._databaseFixture.DeleteData(null, Clear.Delete_Events_Opinions);
            }
        }


        [Fact]
        public async Task Should_Change_Visibility()
        {
            var eventId = await this._databaseFixture.SetOnlyOneEvent();

            var request = new ChangeVisibilityCommand(eventId);
            try
            {
                var responseMessage = await this.ClientCall(request, HttpMethod.Put, "api/Event/ChangeVisibilityEvent");
                var responseData = await this.ReadFromResponse<Response<int>>(responseMessage);

                Assert.NotNull(responseData?.Data);
                Assert.True(responseData?.Success);
            }
            finally
            {
                await this._databaseFixture.DeleteData("Events");
            }
        }


        [Fact]
        public async Task Should_Update_Event_Data()
        {
            var eventId = await this._databaseFixture.SetOnlyOneEvent();
            var newDescription = "New_Description_Test_After_Update";
            var newCategory = EventCategory.Corporate;

            var request = new UpdateEventCommand(eventId, newDescription, categoryType: newCategory);

            try
            {
                var responseMessage = await this.ClientCall(request, HttpMethod.Put, "api/Event/UpdateEventInformation");
                var responseData = await this.ReadFromResponse<Response<string>>(responseMessage);

                Assert.True(responseData?.Success);

                var eventAfterUpdate = await this._databaseFixture.GetEvent(eventId);

                Assert.Equal(newDescription, eventAfterUpdate?.EventDescription);
                Assert.Equal(newCategory, eventAfterUpdate?.EventCategory);

            }
            finally
            {
                await this._databaseFixture.DeleteData("Events");
            }
        }


        [Fact]
        public async Task Should_Cancel_Event()
        {
            var eventId = await this._databaseFixture.SetOnlyOneEvent();

            var request = new CancelEventCommand(eventId);

            try
            {
                var responseMessage = await this.ClientCall(request, HttpMethod.Put, "api/Event/CancelEvent");
                var responseData = await this.ReadFromResponse<Response<string>>(responseMessage);

                Assert.True(responseData?.Success);
            }
            finally
            {
                await this._databaseFixture.DeleteData("Events");
            }
        }
    }
}