using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using EventManagement.Application.Models.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EM.IntegrationTests.Setup
{
    public class DatabaseFixture
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Fixture _fixture;
        private int _randomInt;

        public DatabaseFixture()
        {
        }

        public DatabaseFixture(IPasswordHasher<User> passwordHasher)
        {
            this._fixture = new Fixture();
            this._passwordHasher = passwordHasher;
        }

        public async Task UserSeedData()
        {
            this._randomInt = this.GetRandomInt();
            for (var i = 0; i < this._randomInt; i++)
            {
                var userTest = new User()
                {
                    FirstName = $"Test_FirstName{i}",
                    LastName = $"Test_LastName{i}",
                    UserName = $"Test_UserName{i}",
                    Email = $"Test{i}@test.com",
                    PhoneNumber = this.GetRandomInt(100, 1000).ToString() + "-" +
                                  this.GetRandomInt(100, 1000).ToString() + "-" +
                                  this.GetRandomInt(100, 1000).ToString(),
                };
                var passwordTest = userTest.UserName + "$" + "123";
                userTest.PasswordHash = this._passwordHasher.HashPassword(userTest, passwordTest);
            }
        }

        private int GetRandomInt(int a = 1, int b = 10) => new Random().Next(a, b);


        public async Task DeleteData(string tableName = null)
        {
            var tablesList = new List<string>
            {
                "Opinions", "EventImages", "UserRoles", "RefreshTokens",
                "PerformanceProposals", "EventApplications",
                "Performers", "Events", "ApplicationUsers"
            };

            var script = string.Empty;
            if (string.IsNullOrEmpty(tableName))
            {
                script = @"BEGIN TRANSACTION 
                       DELETE FROM Opinions
                       DELETE FROM EventImages
                       DELETE FROM UserRoles
                       DELETE FROM RefreshTokens
                       DELETE FROM PerformanceProposals
                       DELETE FROM EventApplications
                       DELETE FROM Performers
                       DELETE FROM [Events]
                       DELETE FROM ApplicationUsers
                       COMMIT TRANSACTION";
            }
            else
            {
                var result =
                    tablesList.FirstOrDefault(_ => _.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));
                if (result == null)
                {
                    throw new ArgumentException(nameof(tableName));
                }

                script = $"DELETE FROM {result}";
            }

            if (string.IsNullOrEmpty(script))
            {
                throw new ArgumentNullException(nameof(script));
            }

            await using var connection = new SqlConnection(
                "Server=host.docker.internal,1440;Database=EventManagementTests;User Id=sa;Password=Password_123BD;");
            await connection.OpenAsync();

            await connection.ExecuteAsync(script);
        }
    }
}