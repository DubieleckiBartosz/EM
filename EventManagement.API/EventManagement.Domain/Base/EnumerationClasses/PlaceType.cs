namespace EventManagement.Domain.Base.EnumerationClasses
{
    public class PlaceType : Enumeration
    {
        public static PlaceType Outdoors = new(1, nameof(Outdoors));
        public static PlaceType Local = new(2, nameof(Local));
        public static PlaceType Magazine = new(3, nameof(Magazine));
        public static PlaceType Stadium = new(4, nameof(Stadium));

        public PlaceType(int id, string name) : base(id, name)
        {
        }
    }
}