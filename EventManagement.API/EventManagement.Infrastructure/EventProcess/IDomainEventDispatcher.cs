using System.Threading.Tasks;
using EventManagement.Domain.DDD;
using EventManagement.Infrastructure.Database;

namespace EventManagement.Infrastructure.EventProcess
{
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync<T>(T aggregate, Delegates.CommitTransaction commit = null,
            Delegates.RollbackTransaction rollback = null)
            where T : AggregateRoot;
    }
}