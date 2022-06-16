using EventManagement.Domain.Base;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Events
{
    public class ApplicationAccepted : IDomainNotification
    {
        public int EventId { get; }
        public EventApplication EventApplication { get; set; }

        public ApplicationAccepted(int eventId, EventApplication application)
        {
            this.EventId = eventId;
            this.EventApplication = application;
        }
    }
}