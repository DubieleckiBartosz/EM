using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EventManagement.Application.Models.Authorization;
using System.Threading.Tasks;
using Dapper;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Infrastructure.Database;

namespace EventManagement.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(EventContext context) : base(context)
        {
            
        }

        public async Task<bool> AddToRoleAsync(int userId, int roleId, IDbTransaction transaction = null)
        {
            var param = new DynamicParameters();

            param.Add("@userId", userId);
            param.Add("@role", roleId);

            var result =
                await this.ExecuteAsync("user_addToRole_I", param, transaction: transaction,
                    commandType: CommandType.StoredProcedure);
            return result > 0;
        }

        public async Task<int?> CreateAsync(User user)
        {
            var param = new DynamicParameters();

            param.Add("@firstName", user.FirstName);
            param.Add("@lastName", user.LastName);
            param.Add("@userName", user.UserName);
            param.Add("@email", user.Email);
            param.Add("@phoneNumber", user.PhoneNumber);
            param.Add("@passwordHash", user.PasswordHash);
            param.Add("@new_identity", -1, DbType.Int32, ParameterDirection.Output);

            var identifier =
                await this.ExecuteAsync("user_createNewUser_I", param,
                    commandType: CommandType.StoredProcedure);
            return param.Get<int?>("@new_identity");
        }

        public async Task<User> FindUserByTokenAsync(string tokenKey)
        {
            var param = new DynamicParameters();

            param.Add("@tokenKey", tokenKey);
            var dict = new Dictionary<int, User>();
            var result = await this.QueryAsync<User, RefreshToken, User>("user_getUserByToken_S", (u, rt) =>
                {
                    if (!dict.TryGetValue(u.Id, out User user))
                    {
                        user = u;
                        user.RefreshTokens = new List<RefreshToken>();
                        dict.Add(u.Id, user);
                    }

                    if (rt != null)
                    {
                        user.RefreshTokens.Add(rt);
                    }

                    return user;
                },
                param: param, commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var param = new DynamicParameters();

            param.Add("@email", email);

            var dict = new Dictionary<int, User>();
            var result = await this.QueryAsync<User, RefreshToken, User>("user_getUserByEmail_S", (u, rt) =>
                {
                    if (!dict.TryGetValue(u.Id, out User user))
                    {
                        user = u;
                        user.RefreshTokens = new List<RefreshToken>();
                        dict.Add(u.Id, user);
                    }

                    if (rt?.Token != null)
                    {
                        user.RefreshTokens.Add(rt);
                    }

                    return user;
                },
                param: param, splitOn: "Id,Id", commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(User user, IDbTransaction transaction = null)
        {
            var toTableType = user.RefreshTokens.Select(_ => _.MapToBase()).ToList();

            DataTable tokenTable = new DataTable();
            tokenTable.Columns.Add(new DataColumn("Token", typeof(string)));
            tokenTable.Columns.Add(new DataColumn("Expires", typeof(DateTime)));
            tokenTable.Columns.Add(new DataColumn("Created", typeof(DateTime)));
            tokenTable.Columns.Add(new DataColumn("ReplacedByToken", typeof(string)));
            tokenTable.Columns.Add(new DataColumn("Revoked", typeof(string)));

            foreach (var token in toTableType)
            {
                tokenTable.Rows.Add(token.Token, token.Expires, token.Expires, token.ReplacedByToken, token.Revoked);
            }


            var param = new DynamicParameters();

            param.Add("@email", user.Email);
            param.Add("@phoneNumber", user.PhoneNumber);
            param.Add("@userId", user.Id);
            param.Add("@refreshTokens",
                //toTableType.ToDataTable()
                tokenTable.AsTableValuedParameter("UserRefreshTokensTableType"));

            var result =
                await this.ExecuteAsync("user_updateUserData_U", param, transaction: transaction,
                    commandType: CommandType.StoredProcedure);
            return result > 0;
        }

        public async Task<List<string>> GetUserRolesAsync(User user)
        {
            var param = new DynamicParameters();

            param.Add("@userId", user.Id);

            var result =
                await this.QueryAsync<string>("user_getUserRoles_S", param,
                    commandType: CommandType.StoredProcedure);
            return result?.ToList();
        }


        public async Task ClearTokens()
        {
            await this.ExecuteAsync("user_clearRevokedTokens_D",
                commandType: CommandType.StoredProcedure);
        }
    }
}