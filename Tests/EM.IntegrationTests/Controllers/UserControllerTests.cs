using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EM.IntegrationTests.Setup;
using EM.IntegrationTests.Setup.Constants;
using EventManagement.API;
using EventManagement.Application.Models.Authorization;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Mvc.Testing;
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

            try
            {
                var responseMessage =
                    await this.ClientCall(userRegisterModelTest, HttpMethod.Post, "api/User/Register");

                Assert.True(responseMessage.StatusCode == HttpStatusCode.OK);
            }
            finally
            {
                await this._databaseFixture.DeleteData(constScript: Clear.Delete_Tokens_UserRoles_AppUsers);
            }
        }


        [Fact]
        public async Task Should_Returns_AuthenticationModel_When_Trying_To_Login()
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

            await this._databaseFixture.SetCustomUser(userTest, "Test$123");

            var requestLoginModel = new LoginModel()
            {
                Email = $"User@test.com",
                Password = "Test$123"
            };
            try
            {
                var responseMessage = await this.ClientCall(requestLoginModel, HttpMethod.Post, "api/User/Login");

                var responseData = await this.ReadFromResponse<Response<AuthenticationModel>>(responseMessage);

                Assert.True(responseMessage.StatusCode == HttpStatusCode.OK);
                Assert.NotNull(responseData?.Data);
                Assert.NotNull(responseData.Data.Token);
            }
            finally
            {
                await this._databaseFixture.DeleteData(constScript: Clear.Delete_Tokens_UserRoles_AppUsers);
            }
        }


        [Fact]
        public async Task Should_Add_User_To_Role()
        {
            var request = new UserAddToRoleModel()
            {
                Email = "User@test.com",
                Role = "Performer"
            };

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

            await this._databaseFixture.SetCustomUser(userTest, "Test$123");
            try
            {
                var responseMessage = await this.ClientCall(request, HttpMethod.Post, "api/User/AddUserToRole");

                Assert.True(responseMessage.StatusCode == HttpStatusCode.OK);
            }
            finally
            {
                await this._databaseFixture.DeleteData(constScript: Clear.Delete_Tokens_UserRoles_AppUsers);
            }
        }

        [Fact]
        public async Task Should_Revoke_Token()
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

            await this._databaseFixture.SetCustomUser(userTest, "Test$123");

            var requestLoginModel = new LoginModel()
            {
                Email = $"User@test.com",
                Password = "Test$123"
            };
            try
            {
                var responseLoginMessage = await this.ClientCall(requestLoginModel, HttpMethod.Post, "api/User/Login");

                if (!responseLoginMessage.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Login failed.");
                }

                var responseMessage = await this.ClientCall<object>(null, HttpMethod.Get, "api/User/CurrentUserInfo");
                var responseData = await this.ReadFromResponse<Response<UserCurrentIFullInfo>>(responseMessage);

                Assert.NotNull(responseData?.Data);
                Assert.Equal(requestLoginModel.Email, responseData.Data.Email);
            }
            finally
            {
                await this._databaseFixture.DeleteData(constScript: Clear.Delete_Tokens_UserRoles_AppUsers);
            }
        }
    }
}