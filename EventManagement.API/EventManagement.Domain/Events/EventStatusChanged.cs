using EventManagement.Domain.Base;
using EventManagement.Domain.Base.EnumerationClasses;

namespace EventManagement.Domain.Events
{
    public class EventStatusChanged : IDomainNotification
    {
        public int EventId { get; }
        public string EventName { get; }
        public EventCurrentStatus Status { get; }

        public EventStatusChanged(int eventId, EventCurrentStatus status, string eventName)
        {
            this.Status = status;
            this.EventId = eventId;
            this.EventName = eventName;
        }
    }
}