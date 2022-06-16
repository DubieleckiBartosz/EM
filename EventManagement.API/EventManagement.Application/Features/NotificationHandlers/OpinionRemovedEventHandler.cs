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
    public class OpinionRemovedEventHandler : INotificationHandler<DomainNotification<OpinionRemoved>>
    {
        private readonly IOpinionRepository _opinionRepository;

        public OpinionRemovedEventHandler(IOpinionRepository opinionRepository)
        {
            this._opinionRepository = opinionRepository;
        }

        public async Task Handle(DomainNotification<OpinionRemoved> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            await this._opinionRepository.RemoveOpinion(domainEvent.OpinionId);
        }
    }
}