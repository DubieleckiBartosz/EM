using System;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Domain.DDD;
using EventManagement.Infrastructure.Database;
using EventManagement.Infrastructure.EventProcess;

namespace EventManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EventContext _context;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly ITransaction _transaction;
        private IEventApplicationRepository _eventApplicationRepository;
        private IEventRepository _eventRepository;
        private IEventImageRepository _eventImageRepository;
        private IOpinionRepository _opinionRepository;
        private IPerformerRepository _performerRepository;
        private IPerformanceProposalRepository _proposalRepository;

        public UnitOfWork(EventContext context, IDomainEventDispatcher domainEventDispatcher, ITransaction transaction)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._domainEventDispatcher =
                domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
            this._transaction = transaction;
        }

        public IPerformerRepository Performer
        {
            get
            {
                if (this._performerRepository == null)
                {
                    this._performerRepository = new PerformerRepository(this._context);
                }

                return this._performerRepository;
            }
        }
        
        public IPerformanceProposalRepository ProposalRepository
        {
            get
            {
                if (this._proposalRepository == null)
                {
                    this._proposalRepository = new PerformanceProposalRepository(this._context);
                }

                return this._proposalRepository;
            }
        }

        public IEventApplicationRepository EventApplication
        {
            get
            {
                if (this._eventApplicationRepository == null)
                {
                    this._eventApplicationRepository = new EventApplicationRepository(this._context);
                }

                return this._eventApplicationRepository;
            }
        }

        public IEventRepository EventRepository
        {
            get
            {
                if (this._eventRepository == null)
                {
                    this._eventRepository = new EventRepository(this._context);
                }

                return this._eventRepository;
            }
        }

        public IEventImageRepository EventImageRepository
        {
            get
            {
                if (this._eventImageRepository == null)
                {
                    this._eventImageRepository = new EventImageRepository(this._context);
                }

                return this._eventImageRepository;
            }
        }

        public IOpinionRepository OpinionRepository
        {
            get
            {
                if (this._opinionRepository == null)
                {
                    this._opinionRepository = new OpinionRepository(this._context);
                }

                return this._opinionRepository;
            }
        }

        public void Complete()
        {
            this._transaction.Commit();
        }

        public async Task CompleteAsync<T>(T aggregate) where T : AggregateRoot
        {
            if (this._domainEventDispatcher != null)
            {
                await this._domainEventDispatcher.DispatchEventsAsync(aggregate,//old
                    new Delegates.CommitTransaction(this._transaction.Commit),
                    new Delegates.RollbackTransaction(this._transaction.Rollback));
            }
        }
    }
}