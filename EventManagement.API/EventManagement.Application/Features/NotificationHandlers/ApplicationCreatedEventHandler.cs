using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Decorators;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Events;
using MediatR;

namespace EventManagement.Application.Features.NotificationHandlers
{
    public class ApplicationCreatedEventHandler : INotificationHandler<DomainNotification<ApplicationCreated>>
    {
        private readonly IEventApplicationRepository _eventApplicationRepository;
        private readonly IPerformanceProposalRepository _proposalRepository;

        public ApplicationCreatedEventHandler(
            IEventApplicationRepository eventApplicationRepository,
            IPerformanceProposalRepository proposalRepository)
        {
            this._eventApplicationRepository = eventApplicationRepository ??
                                               throw new ArgumentNullException(nameof(eventApplicationRepository));
            this._proposalRepository = proposalRepository ??
                                       throw new ArgumentNullException(nameof(proposalRepository));
        }

        public async Task Handle(DomainNotification<ApplicationCreated> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification?.DomainEvent;
            if (domainEvent == null)
            {
                throw new ArgumentException(ResponseStrings.EventCannotBeNull);
            }

            var performer = domainEvent.Performer;
            var application = domainEvent.EventApplication;
            var proposals = await this._proposalRepository.GetProposalsByPerformerIdAsync(performer.Id);
            var proposal = proposals?.FirstOrDefault(_ => _.EventId == application.EventId);
            if (proposal != null)
            {
                await this._proposalRepository.RemoveProposalAsync(proposal.Id);
            }

            await this._eventApplicationRepository.CreateNewApplicationAsync(application,
                performer.PerformerName.NameValue);
        }
    }
}