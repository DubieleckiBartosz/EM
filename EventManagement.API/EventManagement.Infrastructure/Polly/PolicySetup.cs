using System;
using System.Data.SqlClient;
using EventManagement.Application.Contracts;
using Polly;

namespace EventManagement.Infrastructure.Polly
{
    public class PolicySetup
    {
        public PolicySetup()
        {
        }

        public AsyncPolicy PolicyConnectionAsync<T>(ILoggerManager<T> logger) => Policy
            .Handle<SqlException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger?.LogError(new
                    {
                        RetryAttempt = retryCount,
                        ExceptionMessage = exception?.Message,
                        Waiting = timeSpan.Seconds
                    });
                }
            );


        public AsyncPolicy PolicyQueryAsync<T>(ILoggerManager<T> logger) => Policy.Handle<SqlException>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger?.LogError(new
                    {
                        RetryAttempt = retryCount,
                        ExceptionMessage = exception?.Message,
                        ProcedureName = this.GetProcedure(exception),
                        Waiting = timeSpan.Seconds
                    });
                }
            );


        #region Private

        private string GetProcedure(Exception exception) => exception is SqlException ex ? ex.Procedure : string.Empty;

        #endregion
    }
}