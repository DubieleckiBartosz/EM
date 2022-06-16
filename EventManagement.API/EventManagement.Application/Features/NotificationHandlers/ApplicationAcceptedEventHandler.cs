using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Decorators;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Events;
using MediatR;

namespace EventManagement.Application.Features.NotificationHandlers
{
    public class ApplicationAcceptedEventHandler : INotificationHandler<DomainNotification<ApplicationAccepted>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationAcceptedEventHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork ??
                               throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(DomainNotification<ApplicationAccepted> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            var applicationRepository = this._unitOfWork.EventApplication;

            await applicationRepository.UpdateAsync(domainEvent.EventApplication);
        }
    }
}