namespace EventManagement.Domain.Base.EnumerationClasses
{
    public class TypePerformance : Enumeration
    {
        public static TypePerformance StandUp = new(1, nameof(StandUp));
        public static TypePerformance Musical = new(2, nameof(Musical));
        public static TypePerformance Acrobatics = new(3, nameof(Acrobatics));
        public TypePerformance(int id, string name) : base(id, name)
        {
        }
    }
}