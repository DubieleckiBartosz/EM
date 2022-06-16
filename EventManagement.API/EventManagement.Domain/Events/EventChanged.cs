using EventManagement.Domain.Base;

namespace EventManagement.Domain.Events
{
    public class EventChanged : IDomainNotification
    {
        public int EventId { get; }
        public EventChanged(int eventId)
        {
            this.EventId = eventId;
        }
    }
}
