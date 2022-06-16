using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Attributes;
using EventManagement.Application.Contracts;
using MediatR;

namespace EventManagement.Application.Behaviours
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _requestHandler;
        private readonly ILoggerManager<TransactionBehavior<TRequest, TResponse>> _loggerManager;
        private readonly ITransaction _transaction;

        public TransactionBehavior(IRequestHandler<TRequest, TResponse> requestHandler,
            ILoggerManager<TransactionBehavior<TRequest, TResponse>> loggerManager, ITransaction transaction)
        {
            this._requestHandler = requestHandler ?? throw new ArgumentNullException(nameof(requestHandler));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
            this._transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _loggerManager.LogInformation(null, "test log");
            var hasTransactionAttribute = this._requestHandler.GetType()
                .GetCustomAttributes(typeof(WithTransactionAttribute), false).Any();
            if (!hasTransactionAttribute)
            {
                return await next();
            }
            else
            {
                try
                {
                    this._loggerManager.LogInformation(
                        $"The transaction will be created by {request.GetType().FullName} ------ HANDLER WITH TRANSACTION ------- ");

                    await this._transaction.GetOpenOrCreateTransaction();
                    var response = await next();

                    var result = this._transaction.Commit();
                    if (result)
                    {
                        this._loggerManager.LogInformation(
                            $"Committed transaction {request.GetType().FullName} ------ COMMITTED TRANSACTION IN HANDLER ------- ");
                    }
                    
                    return response;
                }
                catch (Exception ex)
                {
                    this._loggerManager.LogError(new
                    {
                        Message = "ERROR Handling transaction.",
                        DataException = ex,
                        Request = request,
                    });

                    throw;
                }
            }
        }
    }
}
