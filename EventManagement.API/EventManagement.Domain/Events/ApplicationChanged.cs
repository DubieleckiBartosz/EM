using EventManagement.Domain.Base;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Events
{
    public class ApplicationChanged : IDomainNotification
    {
        public EventApplication EventApplication { get; }

        public ApplicationChanged(EventApplication eventApplication)
        {
            this.EventApplication = eventApplication;
        }
    }
}