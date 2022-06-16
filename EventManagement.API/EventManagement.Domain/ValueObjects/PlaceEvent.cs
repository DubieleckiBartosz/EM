using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.EventRules;

namespace EventManagement.Domain.ValueObjects
{
    public class PlaceEvent : ValueObject
    {
        public string City { get; private set; }
        public string PostalCode { get; private set; }
        public string Street { get; private set; }
        public string NumberStreet { get; private set; }

        private PlaceEvent(string city, string postalCode, string street, string numberStreet)
        {
            (this.City, this.PostalCode, this.Street, this.NumberStreet) = (city, postalCode, street, numberStreet);
        }

        public static PlaceEvent Create(string city, string postalCode, string street, string numberStreet)
        {
            CheckRule(new PlaceEventRule(city, postalCode, street, numberStreet));
            return new PlaceEvent(city, postalCode, street, numberStreet);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.City;
            yield return this.PostalCode;
            yield return this.Street;
            yield return this.NumberStreet;
        }
    }
}