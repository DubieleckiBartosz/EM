using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Decorators;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Events;
using MediatR;

namespace EventManagement.Application.Features.NotificationHandlers
{
    public class EventUpdatedEventHandler : INotificationHandler<DomainNotification<EventChanged>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerManager<EventUpdatedEventHandler> _loggerManager;

        public EventUpdatedEventHandler(IUnitOfWork unitOfWork, ILoggerManager<EventUpdatedEventHandler> loggerManager)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._loggerManager = loggerManager;
        }

        public async Task Handle(DomainNotification<EventChanged> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            var performerRepository = this._unitOfWork.Performer;

            var performers = await performerRepository.GetPerformersByEventIdAsync(domainEvent.EventId);
            if (performers == null || !performers.Any())
            {
                this._loggerManager.LogInformation(new
                {
                    Message = "Performer list is empty."
                });

                return;
            }

            foreach (var performer in performers)
            {
                //send mail
            }
        }
    }
}