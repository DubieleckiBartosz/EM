using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using FluentValidation;
using MediatR;
using ValidationException = EventManagement.Application.Exceptions.ValidationException;

namespace EventManagement.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILoggerManager<ValidationBehaviour<TRequest, TResponse>> _loggerManager;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators, ILoggerManager<ValidationBehaviour<TRequest, TResponse>> loggerManager)
        {
            this._validators = validators ?? throw new ArgumentNullException(nameof(validators));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var commandType = request.GetType().FullName;
            this._loggerManager.LogInformation($"----- Validating command {commandType}");

            var errorList = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (errorList.Any())
            {
                this._loggerManager.LogWarning(new
                {
                    Message = "Validation errors",
                    Command = commandType,
                    Errors = errorList
                });

                throw new ValidationException(errorList);
            }


            return await next();
        }
    }
}