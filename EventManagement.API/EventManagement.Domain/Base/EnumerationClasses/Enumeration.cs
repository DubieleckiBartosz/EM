using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventManagement.Domain.Base.EnumerationClasses
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        protected Enumeration(int id, string name)
        {
            (Id, Name) = (id, name);
        }

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                .Select(field => field.GetValue(null))
                .Cast<T>();

        public static T GetById<T>(int value) where T : Enumeration =>
            GetAll<T>()?.FirstOrDefault(item => item.Id == value) ??
            throw new ArgumentNullException($"{value} does not exist in {typeof(T)}");

        public static bool operator ==(Enumeration a, Enumeration b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Enumeration s, Enumeration b) => !(s == b);

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration) other).Id);
    }
}