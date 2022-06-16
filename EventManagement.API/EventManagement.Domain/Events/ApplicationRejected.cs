using EventManagement.Domain.Base;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Events
{
    public class ApplicationRejected : IDomainNotification
    {
        public EventApplication EventApplication { get; }
        public ApplicationRejected(EventApplication eventApplication)
        {
            this.EventApplication = eventApplication; 
        }
    }
}