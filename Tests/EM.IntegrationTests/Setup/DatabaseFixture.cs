using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using EventManagement.Application.Models.Authorization;
using EventManagement.Application.Models.Enums.Auth;
using Microsoft.AspNetCore.Identity;

namespace EM.IntegrationTests.Setup
{
    public class DatabaseFixture
    {
        private const string Connection =
            "Server=host.docker.internal,1440;Database=EventManagementTests;User Id=sa;Password=Password_123BD;";

        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Fixture _fixture;
        private int _randomInt;

        public DatabaseFixture()
        {
            this._fixture = new Fixture();
            this._passwordHasher = new PasswordHasher<User>();
        }

        public async Task SetCustomUser(User user, string password)
        {
            user.PasswordHash = this._passwordHasher.HashPassword(user, password);
            var param = this.GetUserParameters(user);

            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                var success = await connection.ExecuteAsync("user_createNewUser_I", param,
                    commandType: CommandType.StoredProcedure, transaction: transaction);

                if (success <= 0)
                {
                    throw new ArgumentException("Threw an exception while inserting users.");
                }
                var identifier = param.Get<int?>("@new_identity");
                var parameters = new DynamicParameters();

                parameters.Add("@userId", identifier);
                parameters.Add("@role", (int) Roles.User);

                await connection.ExecuteAsync("user_addToRole_I", parameters, commandType: CommandType.StoredProcedure,
                    transaction: transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }

        public async Task UserSeedData()
        {
            this._randomInt = this.GetRandomInt();
            var userList = new List<User>();
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
                userList.Add(userTest);
            }

            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                foreach (var user in userList)
                {
                    var param = this.GetUserParameters(user);
                    var identifier = await connection.ExecuteAsync("user_createNewUser_I", param,
                        commandType: CommandType.StoredProcedure, transaction: transaction);

                    if (identifier <= 0)
                    {
                        throw new ArgumentException("Threw an exception while inserting users.");
                    }

                    var parameters = new DynamicParameters();

                    parameters.Add("@userId", identifier);
                    parameters.Add("@role", (int) Roles.User);

                    await connection.ExecuteAsync("user_addToRole_I", parameters, commandType: CommandType.StoredProcedure,
                        transaction: transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task DeleteData(string tableName = null, string constScript = null)
        {
            var tablesList = new List<string>
            {
                "Opinions", "EventImages", "UserRoles", "RefreshTokens",
                "PerformanceProposals", "EventApplications",
                "Performers", "Events", "ApplicationUsers"
            };

            string script;
            if (string.IsNullOrEmpty(tableName) && constScript == null)
            {
                script = @"BEGIN TRANSACTION 
                           DELETE FROM Opinions
	                       DELETE FROM EventImages
	                       DELETE FROM UserRoles 
	                       WHERE UserId != 1 
	                       DELETE FROM RefreshTokens
	                       DELETE FROM PerformanceProposals
	                       DELETE FROM EventApplications
	                       DELETE FROM Performers
	                       DELETE FROM [Events]
	                       DELETE FROM ApplicationUsers 
	                       WHERE Email != 'SuperUser@test.com'  
                           COMMIT TRANSACTION";
            }
            else if(!string.IsNullOrEmpty(tableName) && constScript == null)
            {
                var result =
                    tablesList.FirstOrDefault(_ => _.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));
                if (result == null)
                {
                    throw new ArgumentException(nameof(tableName));
                }

                script = $"DELETE FROM {result}";
            }
            else
            {
                script = constScript;
            }

            if (string.IsNullOrEmpty(script))
            {
                throw new ArgumentNullException(nameof(script));
            }

            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();

            await connection.ExecuteAsync(script);
        }
 

        private int GetRandomInt(int a = 1, int b = 10) => new Random().Next(a, b);

        private DynamicParameters GetUserParameters(User user)
        {
            var param = new DynamicParameters();

            param.Add("@firstName", user.FirstName);
            param.Add("@lastName", user.LastName);
            param.Add("@userName", user.UserName);
            param.Add("@email", user.Email);
            param.Add("@phoneNumber", user.PhoneNumber);
            param.Add("@passwordHash", user.PasswordHash);
            param.Add("@new_identity", -1, DbType.Int32, ParameterDirection.Output);

            return param;
        }
    }
}