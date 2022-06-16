using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using EventManagement.Infrastructure.Database;

namespace EventManagement.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly EventContext Context;

        protected BaseRepository(EventContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected SqlTransaction Transaction => Context.CurrentTransaction();
        protected async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null,
            CommandType? commandType = null)
        {
            return await this.Context.WithConnection<T>(transaction,
                async f => await f.QueryFirstOrDefaultAsync(sql: sql, param: param,
                    transaction: transaction,
                    commandType: commandType));
        }

        protected async Task<T> QuerySingleAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null,
            CommandType? commandType = null)
        {
            return await this.Context.WithConnection<T>(transaction,
                async f => await f.QuerySingleAsync(sql: sql, param: param,
                    transaction: transaction,
                    commandType: commandType));
        }

        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null,
            CommandType? commandType = null)
        {
            return await this.Context.WithConnection(transaction,
                async f => await f.QueryAsync<T>(sql: sql, param: param,
                    transaction: transaction,
                    commandType: commandType));
        }

        protected async Task<int> ExecuteAsync(string sql, object param = null,
            IDbTransaction transaction = null,
            CommandType? commandType = null)
        {
            return await this.Context.WithConnection(transaction,
                async f => await f.ExecuteAsync(sql: sql, param: param,
                    transaction: transaction,
                    commandType: commandType));
        }

        protected async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, TReturn>(string sql, Func<T1, T2, TReturn> map,
            string splitOn = "Id", object param = null,
            IDbTransaction transaction = null,
            CommandType? commandType = null)
        {
            return await this.Context.WithConnection(transaction,
                async f => await f.QueryAsync<T1, T2, TReturn>(sql: sql, map: map, splitOn: splitOn, param: param,
                    transaction: transaction,
                    commandType: commandType));
        }

        protected async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, TReturn>(string sql,
            Func<T1, T2, T3, TReturn> map,
            string splitOn = "Id", object param = null,
            IDbTransaction transaction = null,
            CommandType? commandType = null)
        {
            return await this.Context.WithConnection(transaction,
                async f => await f.QueryAsync<T1, T2, T3, TReturn>(sql: sql, map: map, splitOn: splitOn, param: param,
                    transaction: transaction,
                    commandType: commandType));
        }

        protected async Task<IEnumerable<Tuple<T1, T2, T3>>> QueryMultipleAsync<T1, T2, T3>(string sql,
            string splitOn = "Id", object param = null,
            IDbTransaction transaction = null,
            CommandType? commandType = null)
        {
            return await this.Context.WithConnection(transaction,
                async f => await f.QueryAsync<T1, T2, T3, Tuple<T1, T2, T3>>(sql: sql,
                    (t, t2, t3) => Tuple.Create(t, t2, t3), splitOn: splitOn,
                    param: param,
                    transaction: transaction,
                    commandType: commandType));
        }
    }
}