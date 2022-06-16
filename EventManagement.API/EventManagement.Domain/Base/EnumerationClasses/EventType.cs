namespace EventManagement.Domain.Base.EnumerationClasses
{
    public class EventType : Enumeration
    {
        public static EventType OldStyle = new(1, nameof(OldStyle));
        public static EventType Modern = new(2, nameof(Modern));

        public EventType(int id, string name) : base(id, name)
        {
        }
    }
}