using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using EventManagement.Application.Models.Authorization;
using EventManagement.Application.Models.Dao.EventDAOs;
using EventManagement.Application.Models.Dao.PerformerDAOs;
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

        public async Task<int> SetOnlyOneEvent()
        {
            var startDate = DateTime.Now.AddDays(this.GetRandomInt(20, 50));
            var endDate = startDate.AddDays(1);

            var param = new DynamicParameters();
            param.Add("@eventName", "Event_Test");
            param.Add("@eventDescription", "Event_Description_Test");
            param.Add("@startDate", startDate);
            param.Add("@endDate", endDate);
            param.Add("@recurringEvent", 0);
            param.Add("@placeType", 1);
            param.Add("@city", "City_Test");
            param.Add("@street", "Street_Test");
            param.Add("@numberStreet", "123a");
            param.Add("@postalCode", "0" + this.GetRandomInt() + "-" + this.GetRandomInt(1000, 1000));
            param.Add("@eventCategory", 2);
            param.Add("@eventType", 2);
            param.Add("@newIdentifier", -1, DbType.Int32, ParameterDirection.Output);

            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await connection.ExecuteAsync("event_createNewEvent_I", param,
                commandType: CommandType.StoredProcedure);
            var eventId = param.Get<int?>("@newIdentifier");

            if (eventId != null)
            {
                return eventId.Value;
            }

            return 0;
        }

        public async Task<List<int>> SetEvents()
        {
            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            var identifiers = new List<int>();
            try
            {
                for (var i = 0; i < this.GetRandomInt(5, 10); i++)
                {
                    var startDate = DateTime.Now.AddDays(this.GetRandomInt(20, 50));
                    var endDate = startDate.AddDays(1);

                    var param = new DynamicParameters();
                    param.Add("@eventName", $"Event_Test{i}");
                    param.Add("@eventDescription", $"Event_Description_Test{i}");
                    param.Add("@startDate", startDate);
                    param.Add("@endDate", endDate);
                    param.Add("@recurringEvent", 0);
                    param.Add("@placeType", 1);
                    param.Add("@city", $"City_Test{i}");
                    param.Add("@street", $"Street_Test{i}");
                    param.Add("@numberStreet", $"123a{i}");
                    param.Add("@postalCode", $"{i}" + this.GetRandomInt() + "-" + this.GetRandomInt(1000, 1000));
                    param.Add("@eventCategory", 2);
                    param.Add("@eventType", 2);
                    param.Add("@newIdentifier", -1, DbType.Int32, ParameterDirection.Output);

                    await connection.ExecuteAsync("event_createNewEvent_I", param,
                        transaction: transaction, commandType: CommandType.StoredProcedure);
                    var eventId = param.Get<int?>("@newIdentifier");
                    if (eventId.HasValue)
                    {
                        identifiers.Add(eventId.Value);
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return identifiers; 
        }

        public async Task SetPerformers(List<int> userIds)
        {
            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                for (var i = 0; i < userIds.Count; i++)
                {
                    var param = new DynamicParameters();

                    param.Add("@userId", userIds[i]);
                    param.Add("@performerName", $"Performer_Test_Name{i}");
                    param.Add("@vip", this._fixture.Create<bool>());
                    param.Add("@performerMail", $"Performer@testPerformer{i}.com");
                    param.Add("@numberOfPeople", this.GetRandomInt(1, 5));

                    await connection.ExecuteAsync("performer_createNewPerformer_I", param,
                        transaction, commandType: CommandType.StoredProcedure);
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task SetEventApplications(int eventId, List<int> performerIds)
        {
            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                for (var i = 0; i < performerIds.Count; i++)
                {
                    var param = new DynamicParameters();
                    param.Add("@eventId", eventId);
                    param.Add("@performerId", performerIds[i]);
                    param.Add("@performerName", $"Performer_Test_Name{i}");
                    param.Add("@typePerformance", this.GetRandomInt(1, 3));
                    param.Add("@durationInMinutes", this.GetRandomInt(20, 50));
                    param.Add("@currentStatus", 1);

                    await connection.ExecuteAsync("application_createNewEventApplication_I", param,
                        transaction, commandType: CommandType.StoredProcedure);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task SetOpinions(int eventId, int count)
        {
            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                for (var i = 0; i < count; i++)
                {
                    var param = new DynamicParameters();
                    param.Add("@eventId", eventId);
                    param.Add("@userId", null);
                    param.Add("@comment", $"Comment test with unique number {this._fixture.Create<string>()}");
                    param.Add("@stars", this.GetRandomInt(0,5));

                    var result = await connection.ExecuteAsync("opinion_createNewOpinion_I", param,
                        transaction: transaction, commandType: CommandType.StoredProcedure);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<List<PerformerDao>> GetCurrentTestPerformers()
        {
            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();
            return (await connection.QueryAsync<PerformerDao>(
                "performer_getAllPerformers_S", commandType: CommandType.StoredProcedure))?.ToList();
        }

        public async Task<EventBaseDao> GetEvent(int eventId)
        {
            await using var connection = new SqlConnection(Connection);
            await connection.OpenAsync();

            var param = new DynamicParameters();

            param.Add("@eventId", eventId);

            var result = (await connection.QueryAsync<EventBaseDao>("event_getEventBaseDataById_S", param,
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
            return result;
        }

        public async Task<List<int>> UserSeedData()
        {
            this._randomInt = this.GetRandomInt();
            var userList = new List<User>();
            var userIds = new List<int>();

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
                    var success = await connection.ExecuteAsync("user_createNewUser_I", param,
                        commandType: CommandType.StoredProcedure, transaction: transaction);

                    if (success <= 0)
                    {
                        throw new ArgumentException("Threw an exception while inserting users.");
                    }

                    var identifier = param.Get<int?>("@new_identity");

                    if (!identifier.HasValue)
                    {
                        throw new ArgumentException("User identifier is null.");
                    }

                    var parameters = new DynamicParameters();
            
                    parameters.Add("@userId", identifier.Value);
                    parameters.Add("@role", (int) Roles.User);

                    await connection.ExecuteAsync("user_addToRole_I", parameters,
                        commandType: CommandType.StoredProcedure,
                        transaction: transaction);

                    userIds.Add(identifier.Value);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return userIds;
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
            else if (!string.IsNullOrEmpty(tableName) && constScript == null)
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