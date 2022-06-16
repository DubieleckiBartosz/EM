namespace EventManagement.Domain.Base.EnumerationClasses
{
    public class EventCurrentStatus : Enumeration
    {
        public static EventCurrentStatus Active = new (1, nameof(Active)); 
        public static EventCurrentStatus Suspended = new (2, nameof(Suspended));
        public static EventCurrentStatus Cancelled = new(3, nameof(Cancelled));
        public static EventCurrentStatus Realized = new(4, nameof(Realized));
        public EventCurrentStatus(int id, string name) : base(id, name)
        {
        }
    }
}
