using System;
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
    public class ImageStatusChangedEventHandler : INotificationHandler<DomainNotification<MainImageChanged>>
    {
        private readonly IEventImageRepository _eventImageRepository;
        private readonly ILoggerManager<ImageStatusChangedEventHandler> _loggerManager;

        public ImageStatusChangedEventHandler(IEventImageRepository eventImageRepository, ILoggerManager<ImageStatusChangedEventHandler> loggerManager)
        {
            this._eventImageRepository = eventImageRepository ?? throw new ArgumentNullException(nameof(eventImageRepository));
            this._loggerManager = loggerManager;
        }
        public async Task Handle(DomainNotification<MainImageChanged> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            await this._eventImageRepository.UpdateMainStatus(domainEvent.ImageId, domainEvent.IsMain);
            this._loggerManager.LogInformation(new
            {
                Message = "Image status changed.",
                Identifier = domainEvent.EventId,
                NewStatus = $"IsMain: {domainEvent.IsMain}"
            });
        }
    }
}
