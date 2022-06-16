using EventManagement.Domain.Base;

namespace EventManagement.Domain.Events
{
    public class MainImageChanged : IDomainNotification
    {
        public int EventId { get; }
        public bool IsMain { get; }
        public int ImageId { get; }

        public MainImageChanged(int eventId, int imageId, bool isMain)
        {
            this.EventId = eventId;
            this.IsMain = isMain;
            this.ImageId = imageId;
        }
    }
}