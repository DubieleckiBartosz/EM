namespace EventManagement.Domain.Base.EnumerationClasses
{
    public class EventCategory : Enumeration
    {
        public static EventCategory Birthday = new(1, nameof(Birthday));
        public static EventCategory Massive = new(2, nameof(Massive));
        public static EventCategory Private = new(3, nameof(Private));
        public static EventCategory Corporate = new(4, nameof(Corporate));

        public EventCategory(int id, string name) : base(id, name)
        {
        }
    }
}