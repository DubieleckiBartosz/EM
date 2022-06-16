using EventManagement.Domain.Base;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Events
{
    public class ApplicationCreated : IDomainNotification
    {
        public EventApplication EventApplication { get; }
        public Performer Performer { get; }

        public ApplicationCreated(EventApplication eventApplication, Performer performer)
        {
            this.EventApplication = eventApplication;
            this.Performer = performer;
        }
    }
}
