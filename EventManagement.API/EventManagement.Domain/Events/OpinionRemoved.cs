using EventManagement.Domain.Base;

namespace EventManagement.Domain.Events
{
    public class OpinionRemoved : IDomainNotification
    {
        public int OpinionId { get; }

        public OpinionRemoved(int opinionId)
        {
            this.OpinionId = opinionId;
        }
    }
}
