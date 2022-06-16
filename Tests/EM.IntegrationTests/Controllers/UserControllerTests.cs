using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EM.IntegrationTests.Setup;
using EventManagement.API;
using EventManagement.Application.Models.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace EM.IntegrationTests.Controllers
{
    public class UserControllerTests : BaseSetup
    {
        private readonly DatabaseFixture _databaseFixture;

        public UserControllerTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
            this._databaseFixture = new DatabaseFixture();
        }

        [Fact]
        public async Task Should_Register_User()
        {
            var userRegisterModelTest = new RegisterModel()
            {
                FirstName = $"UserTest_FirstName",
                LastName = $"UserTest_LastName",
                UserName = $"UserTest_UserName",
                Email = $"User@tesst.com",
                PhoneNumber = this.GetRandomInt(100, 1000).ToString() + "-" +
                              this.GetRandomInt(100, 1000).ToString() + "-" +
                              this.GetRandomInt(100, 1000).ToString(),
                Password = "UserTest$123",
                ConfirmPassword = "UserTest$123"
            }; 
            
            var content = JsonConvert.SerializeObject(userRegisterModelTest);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/User/Register");
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await this._client.SendAsync(request);

            Assert.True(response.StatusCode == HttpStatusCode.OK);

            await this._databaseFixture.DeleteData("ApplicationUsers");
        }
    }
}
