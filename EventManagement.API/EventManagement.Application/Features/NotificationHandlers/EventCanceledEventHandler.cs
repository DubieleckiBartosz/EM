using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Decorators;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Entities;
using EventManagement.Domain.Events;
using MediatR;

namespace EventManagement.Application.Features.NotificationHandlers
{
    public class EventCanceledEventHandler : INotificationHandler<DomainNotification<EventCanceled>>
    {
        private readonly IEventApplicationRepository _applicationRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager<EventCanceledEventHandler> _loggerManager;

        public EventCanceledEventHandler(IEventApplicationRepository applicationRepository, IMapper mapper,
            ILoggerManager<EventCanceledEventHandler> loggerManager)
        {
            this._applicationRepository =
                applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
        }

        public async Task Handle(DomainNotification<EventCanceled> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            var result =
                await this._applicationRepository.GetApplicationsWithDetailsAsync(domainEvent.EventId);

            if (result == null || !result.Any())
            {
                return;
            }


            foreach (var item in result)
            {
                var application = this._mapper.Map<EventApplication>(item.EventApplicationDao);
                var performer = this._mapper.Map<Performer>(item.PerformerDao);

                application.ChangeStatus(StatusApplication.RejectedDueToEventCancellation, false);
                await this._applicationRepository.UpdateAsync(application);

                var message =
                    $"The {domainEvent.EventName} event has unfortunately been canceled. Please contact our office. Regards XYZ.";
                //send mail

                this._loggerManager.LogInformation(new
                {
                    Message = $"An email was sent to {performer.PerformerName}.",
                    MailMessage = message
                });
            }
        }
    }
}