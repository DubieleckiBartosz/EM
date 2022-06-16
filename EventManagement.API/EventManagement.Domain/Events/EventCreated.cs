using EventManagement.Domain.Base;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Events
{
    public class EventCreated : IDomainNotification
    {
        public Event Event { get; }
        public EventCreated(Event newEvent)
        {
            this.Event = newEvent;
        }
    }
}
