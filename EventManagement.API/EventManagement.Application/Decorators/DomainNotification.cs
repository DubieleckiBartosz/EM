using EventManagement.Domain.Base;
using MediatR;

namespace EventManagement.Application.Decorators
{
    public class DomainNotification<T> : INotification where T: IDomainNotification
    {
        public T DomainEvent { get; }

        public DomainNotification(T @event)
        {
            this.DomainEvent = @event; 
        }
    }
}
