using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules.EventRules;

namespace EventManagement.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string City { get; private set; }
        public string Street { get; private set; }
        public string NumberStreet { get; private set; }
        public string PostalCode { get; private set; }

        private Address(string city, string street, string numberStreet, string postalCode)
        {
            this.City = city;
            this.Street = street;
            this.PostalCode = postalCode;
            this.NumberStreet = numberStreet;
        }

        public static Address Create(string city, string street, string numberStreet, string postalCode)
        {
            CheckRule(new CreateAddressRules(city, street, numberStreet, postalCode));
            return new Address(city, street, numberStreet, postalCode);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.City;
            yield return this.Street;
            yield return this.NumberStreet;
            yield return this.PostalCode;
        }
    }
}