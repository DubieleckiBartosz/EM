using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Settings;
using EventManagement.Infrastructure.Polly;
using Microsoft.Extensions.Options;
using Polly;

namespace EventManagement.Infrastructure.Database
{
    public partial class TransactionSupervisor : ITransaction
    {
        private readonly ILoggerManager<TransactionSupervisor> _loggerManager;
        private readonly ConnectionStrings _connectionString;
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private readonly AsyncPolicy _retryAsyncPolicyConnection;
        private string _transactionId;
        public TransactionSupervisor(ILoggerManager<TransactionSupervisor> loggerManager,
            IOptions<ConnectionStrings> connectionString)
        {
            this._loggerManager = loggerManager;
            this._connectionString = connectionString.Value;
            var policy = new PolicySetup();
            this._retryAsyncPolicyConnection = policy.PolicyConnectionAsync(this._loggerManager);
        }

        public async Task<IDbTransaction> GetOpenOrCreateTransaction()
        {
            this.TransactionIdGenerator();

            this._loggerManager.LogInformation(
                $"Begin transaction ------ TRANSACTION ------- : {this._transactionId}");
            try
            {
                if (this._transaction != null)
                {
                    return this._transaction;
                }

                if (this._connection != null)
                {
                    this._transaction = this._connection.BeginTransaction();
                    return this._transaction;
                }

                this._connection = new SqlConnection(this._connectionString.DefaultConnection);
                await this._retryAsyncPolicyConnection.ExecuteAsync(async () => await this._connection.OpenAsync());
                this._transaction = this._connection.BeginTransaction();
                return this._transaction;
            }
            catch (Exception ex)
            {
                throw new DbException($"CreateTransaction method exception: {ex}");
            }
        }

        public IDbTransaction GetTransactionWhenExist()
        {
            return this._transaction;
        }

        public void Rollback()
        {
            try
            {
                this._loggerManager.LogInformation(null, message: "Rollback start.");
                this._transaction?.Rollback();
                this._loggerManager.LogInformation(null, message: "Rollback OK.");
            }
            catch
            {
                this._loggerManager.LogError("Rollback failed.");
                throw;
            }
            finally
            {
                if (this._transaction != null)
                {
                    this._transaction.Dispose();
                    this._transaction?.Connection?.Dispose();
                    this._transaction = null;
                }
            }
        }

        public bool Commit()
        {
            try
            {
                if (this._transaction == null)
                {
                    this._loggerManager.LogInformation("Transaction is null.");
                    return false;
                }

                this._transaction.Commit();
                this._loggerManager.LogInformation(
                    $"Transaction committed successfully ------ TRANSACTION ------- : {this._transactionId}");
                return true;
            }
            catch
            {
                _loggerManager.LogError("Commit failed.");
                this.Rollback();
                throw;
            }
            finally
            {
                if (this._transaction != null)
                {
                    this._transaction.Dispose();
                    this._transaction?.Connection?.Dispose();
                    this._transaction = null;
                }
            }
        }

        private void TransactionIdGenerator()
        {
            this._transactionId = Guid.NewGuid().ToString();
            this._loggerManager.LogInformation(null, $"New transaction identifier created: {this._transactionId}");
        }
    }
}