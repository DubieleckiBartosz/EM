using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using EM.IntegrationTests.Setup;
using EM.IntegrationTests.Setup.Constants;
using EventManagement.API;
using EventManagement.Application.Features.EventProposalFeatures.Commands.CreateProposal;
using EventManagement.Application.Features.EventProposalFeatures.Commands.RemoveProposal;
using EventManagement.Application.Models.Authorization;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EM.IntegrationTests.Controllers
{
    public class PerformanceProposalsControllerTests : BaseSetup
    {
        private readonly DatabaseFixture _databaseFixture;

        public PerformanceProposalsControllerTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
            this._databaseFixture = new DatabaseFixture();
        }

        [Fact]
        public async Task Should_Create_New_Proposal()
        {
            try
            {
                await this.SetData();
                var testMessage = this._fixture.Create<string>();
                var eventId = await this._databaseFixture.SetOnlyOneEvent();
                var performer = (await this._databaseFixture.GetCurrentTestPerformers()).First();
                var request = new CreateProposalCommand(performer.Id, eventId, testMessage, DateTime.Now.AddDays(2));

                var responseMessage = await this.ClientCall(request, HttpMethod.Post,
                    $"api/PerformanceProposal/CreateProposal");
                var responseData = await this.ReadFromResponse<Response<string>>(responseMessage);

                Assert.True(responseData.Success);
            }
            finally
            {
                await this._databaseFixture.DeleteData(null,
                    Clear.Delete_PerformanceProposals_Performers_Events_ApplicationUsers);
            }
        }


        [Fact]
        public async Task Should_Returns_Proposals()
        {
            try
            {
                await this.SetData();
                var testMessage = this._fixture.Create<string>();
                var eventId = await this._databaseFixture.SetOnlyOneEvent();
                var performer = (await this._databaseFixture.GetCurrentTestPerformers()).First();
                await this._databaseFixture.SetProposal(eventId, performer.Id, testMessage);

                var responseMessage = await this.ClientCall<object>(null, HttpMethod.Get,
                    $"api/PerformanceProposal/GetProposals");
                var responseData = await this.ReadFromResponse<Response<List<PerformanceProposalDto>>>(responseMessage);

                Assert.NotNull(responseData);
                Assert.Equal(1, responseData.Data?.Count);
                Assert.Equal(testMessage, responseData.Data?.First().Message);
            }
            finally
            {
                await this._databaseFixture.DeleteData(null,
                    Clear.Delete_PerformanceProposals_Performers_Events_ApplicationUsers);
            }
        }


        [Fact]
        public async Task Should_Remove_Proposal()
        {
            try
            {
                await this.SetData();
                var testMessage = this._fixture.Create<string>();
                var eventId = await this._databaseFixture.SetOnlyOneEvent();
                var performer = (await this._databaseFixture.GetCurrentTestPerformers()).First();
                var proposalId = await this._databaseFixture.SetProposal(eventId, performer.Id, testMessage);
                var request = new RemoveProposalCommand(proposalId);

                var responseMessage = await this.ClientCall(request, HttpMethod.Delete,
                    $"api/PerformanceProposal/RemoveProposal");
                var responseData = await this.ReadFromResponse<Response<string>>(responseMessage);

                Assert.NotNull(responseData);
                Assert.True(responseData.Success);
            }
            finally
            {
                await this._databaseFixture.DeleteData(null,
                    Clear.Delete_PerformanceProposals_Performers_Events_ApplicationUsers);
            }
        }


        private async Task SetData()
        {
            var userTest = new User()
            {
                FirstName = "UserTest_FirstName",
                LastName = "UserTest_LastName",
                UserName = "UserTest_UserName",
                Email = "User@test.com",
                PhoneNumber = this.GetRandomInt(100, 1000).ToString() + "-" +
                              this.GetRandomInt(100, 1000).ToString() + "-" +
                              this.GetRandomInt(100, 1000).ToString()
            };

            var userId = await this._databaseFixture.SetCustomUser(userTest, "Test$123");

            await this._databaseFixture.SetPerformer(userId);
        }
    }
}