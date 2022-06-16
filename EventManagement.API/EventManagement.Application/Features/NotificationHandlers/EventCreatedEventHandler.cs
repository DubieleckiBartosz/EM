using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Decorators;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Events;
using MediatR;

namespace EventManagement.Application.Features.NotificationHandlers
{
    public class EventCreatedEventHandler : INotificationHandler<DomainNotification<EventCreated>>
    {
        private readonly IPerformerRepository _performerRepository;
        private readonly ILoggerManager<EventCreatedEventHandler> _loggerManager;

        public EventCreatedEventHandler(IPerformerRepository performerRepository,
            ILoggerManager<EventCreatedEventHandler> loggerManager)
        {
            this._performerRepository =
                performerRepository ?? throw new ArgumentNullException(nameof(performerRepository));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
        }

        public async Task Handle(DomainNotification<EventCreated> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            var performers = await this._performerRepository.GetAllPerformersAsync();

            if (performers == null || !performers.Any())
            {
                return;
            }

            var newEvent = domainEvent.Event;
            foreach (var performer in performers)
            {
                var message =
                    $"The new event will take place on {newEvent.EventTime.StartDate.ToShortDateString()} in {newEvent.EventAddress.City}! fill in the application and take part. Regards XYZ.";

                //send mail

                this._loggerManager.LogInformation(new
                {
                    Message = $"Mail was sent to {performer.PerformerMail}."
                });
            }
        }
    }
}