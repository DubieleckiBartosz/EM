using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Decorators;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Events;
using MediatR;

namespace EventManagement.Application.Features.NotificationHandlers
{
    public class EventStatusChangedEventHandler : INotificationHandler<DomainNotification<EventStatusChanged>>
    {
        private readonly IEventRepository _eventRepository;

        public EventStatusChangedEventHandler(IEventRepository performerRepository)
        {
            this._eventRepository =
                performerRepository ?? throw new ArgumentNullException(nameof(performerRepository));
        }

        public async Task Handle(DomainNotification<EventStatusChanged> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            var result =
                await this._eventRepository.GetDataToNotifiedAboutEventStatusChangeAsync(domainEvent.EventId);
            if (result == null)
            {
                return;
            }

            var message = string.Empty;
            var eventName = domainEvent.EventName;

            if (domainEvent.Status == EventCurrentStatus.Active)
            {
                foreach (var item in result)
                {
                    message =
                        $"The event '{eventName}' has been activated!" +
                        "take place! Regards XYZ.";
                    //send mail
                }

                return;
            }

            if (domainEvent.Status == EventCurrentStatus.Suspended)
            {
                foreach (var item in result)
                {
                    var status = Enumeration.GetById<StatusApplication>(item.CurrentStatus);
                    if (status == StatusApplication.InProgress)
                    {
                        message =
                            $"The status of the '{eventName}' event you applied for has been changed to suspended. " +
                            "Please complete the application " +
                            "process as this may affect whether or not the event will " +
                            "take place! Regards XYZ.";
                    }

                    if (status == StatusApplication.NotConsidered || status == StatusApplication.ConsideredPositively)
                    {
                        message = $"The event {eventName} has been suspended. This may change. Regards XYZ.";
                    }

                    //send mail
                }
            }
        }
    }
}