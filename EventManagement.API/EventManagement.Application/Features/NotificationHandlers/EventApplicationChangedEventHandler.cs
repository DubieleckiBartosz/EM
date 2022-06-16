using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Decorators;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Events;
using MediatR;

namespace EventManagement.Application.Features.NotificationHandlers
{
    public class EventApplicationChangedEventHandler : INotificationHandler<DomainNotification<ApplicationChanged>>
    {
        private readonly IEventApplicationRepository _eventApplicationRepository;

        public EventApplicationChangedEventHandler(
            IEventApplicationRepository eventApplicationRepository)
        {
            this._eventApplicationRepository = eventApplicationRepository ??
                                               throw new ArgumentNullException(nameof(eventApplicationRepository));
        }

        public async Task Handle(DomainNotification<ApplicationChanged> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            await _eventApplicationRepository.UpdateAsync(domainEvent.EventApplication);
        }
    }
}