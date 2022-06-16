namespace EventManagement.Domain.Base.EnumerationClasses
{
    public class StatusApplication : Enumeration
    {
        public static StatusApplication NotConsidered = new(1, nameof(NotConsidered));
        public static StatusApplication InProgress = new(2, nameof(InProgress));
        public static StatusApplication Rejected = new(3, nameof(Rejected));
        public static StatusApplication ConsideredPositively = new(4, nameof(ConsideredPositively));
        public static StatusApplication RejectedDueToEventCancellation = new(5, nameof(RejectedDueToEventCancellation));

        public StatusApplication(int id, string name) : base(id, name)
        {
        }
    }
}