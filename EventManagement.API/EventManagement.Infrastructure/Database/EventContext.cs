using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Settings;
using EventManagement.Infrastructure.Polly;
using Microsoft.Extensions.Options;
using Polly;

namespace EventManagement.Infrastructure.Database
{
    public class EventContext
    {
        private readonly ILoggerManager<EventContext> _loggerManager;
        private readonly ITransaction _transaction;
        private readonly ConnectionStrings _connectionString;
        private readonly AsyncPolicy _retryAsyncPolicyQuery;
        private readonly AsyncPolicy _retryAsyncPolicyConnection;

        public EventContext(IOptions<ConnectionStrings> connectionString, ILoggerManager<EventContext> loggerManager,
            ITransaction transaction)
        {
            this._transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
            this._connectionString =
                connectionString?.Value ?? throw new ArgumentNullException(nameof(connectionString));

            var policy = new PolicySetup();
            this._retryAsyncPolicyConnection = policy.PolicyConnectionAsync(this._loggerManager);
            this._retryAsyncPolicyQuery = policy.PolicyQueryAsync(this._loggerManager);
        }

        public SqlTransaction CurrentTransaction() => this._transaction.GetTransactionWhenExist() as SqlTransaction;

        public async Task<T> WithConnection<T>(IDbTransaction transaction, Func<IDbConnection, Task<T>> funcData)
        {
            try
            {
                SqlConnection connection;
                if (transaction != null)
                {
                    connection = transaction.Connection as SqlConnection;
                    return await this._retryAsyncPolicyQuery.ExecuteAsync(async () => await funcData(connection));
                }

                await using (connection = new SqlConnection(this._connectionString.DefaultConnection))
                {
                    await this._retryAsyncPolicyConnection.ExecuteAsync(async () => await connection.OpenAsync());
                    return await this._retryAsyncPolicyQuery.ExecuteAsync(async () => await funcData(connection));
                }
            }
            catch (TimeoutException ex)
            {
                if (this.CurrentTransaction() != null)
                {
                    this._transaction.Rollback();
                }

                throw new Exception($"Timeout sql exception: {ex}");
            }
            catch (SqlException ex)
            {
                if (this.CurrentTransaction() != null)
                {
                    this._transaction.Rollback();
                }

                throw new Exception($"Sql exception: {ex}");
            }
        }
    }
}