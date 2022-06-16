using EventManagement.Domain.Base;

namespace EventManagement.Domain.Events
{
    public class EventCanceled : IDomainNotification
    {
        public int EventId { get; }
        public string EventName { get; }

        public EventCanceled(int eventId, string eventName)
        {
            this.EventId = eventId;
            this.EventName = eventName;
        }
    }
}