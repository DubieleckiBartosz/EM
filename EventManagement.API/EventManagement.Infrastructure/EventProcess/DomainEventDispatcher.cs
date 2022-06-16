using System;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Domain.Base;
using EventManagement.Domain.DDD;
using EventManagement.Infrastructure.Database;

namespace EventManagement.Infrastructure.EventProcess
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IDomainDecorator _decorator;
        private readonly ILoggerManager<DomainEventDispatcher> _loggerManager;

        public DomainEventDispatcher(IDomainDecorator decorator, ILoggerManager<DomainEventDispatcher> loggerManager)
        {
            this._decorator = decorator ?? throw new ArgumentNullException(nameof(decorator));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
        }

        public async Task DispatchEventsAsync<T>(T aggregate, Delegates.CommitTransaction commit = null,
            Delegates.RollbackTransaction rollback = null)
            where T : AggregateRoot
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }

            try
            {
                var events = aggregate?.DomainEvents;
                if (events?.Any() == true)
                {
                    this._loggerManager.LogInformation(null, message: "Sending events to handlers has started.");
                    foreach (var @event in events)
                    {
                        await this._decorator.Publish(@event);
                    }

                    this._loggerManager.LogInformation(null, message: "All events were sent positively.");

                    aggregate.ClearDomainEvents();
                }

                commit?.Invoke();
            }
            catch (Exception ex)
            {
                this._loggerManager.LogInformation(new
                {
                    Messsage = "Error sending events to handlers.",
                    Error = ex?.Message
                });

                rollback?.Invoke();
                throw;
            }
        }
    }
}