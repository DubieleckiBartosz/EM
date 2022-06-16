using System.Collections.Generic;
using EventManagement.Domain.Base;

namespace EventManagement.Domain.DDD
{
    public abstract class AggregateRoot : Entity
    {
        private List<IDomainNotification> _domainEvents;
        public IReadOnlyCollection<IDomainNotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(IDomainNotification eventItem)
        {
            _domainEvents ??= new List<IDomainNotification>();
            lock (_domainEvents)
            {
                _domainEvents.Add(eventItem);
            }
        }

        public void RemoveDomainEvent(IDomainNotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}