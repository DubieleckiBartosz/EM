using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using MediatR.Pipeline;

namespace EventManagement.Application.Behaviours
{
    public class LoggerBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILoggerManager<LoggerBehaviour<TRequest>> _logger;

        public LoggerBehaviour(ILoggerManager<LoggerBehaviour<TRequest>> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;
            _logger.LogInformation(null, $"EventManagement Request: {name} - {request}");

            return Task.CompletedTask;
        }
    }
}