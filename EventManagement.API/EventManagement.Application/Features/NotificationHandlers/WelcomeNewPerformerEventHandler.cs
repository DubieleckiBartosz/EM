using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Decorators;
using EventManagement.Application.Models.Enums.Auth;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Events;
using MediatR;

namespace EventManagement.Application.Features.NotificationHandlers
{
    public class WelcomeNewPerformerEventHandler : INotificationHandler<DomainNotification<PerformerCreated>>
    {
        private readonly ILoggerManager<WelcomeNewPerformerEventHandler> _loggerManager;
        private readonly IUserRepository _userRepository;

        public WelcomeNewPerformerEventHandler(ILoggerManager<WelcomeNewPerformerEventHandler> loggerManager,
            IUserRepository userRepository)
        {
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task Handle(DomainNotification<PerformerCreated> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            await this._userRepository.AddToRoleAsync(domainEvent.UserId, (int) Roles.Performer);

            //sending an email with a confirmation code
            this._loggerManager.LogInformation(new
            {
                Message = "Welcome message has been sent.",
                To = notification?.DomainEvent?.PerformerName
            });
        }
    }
}