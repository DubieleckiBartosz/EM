using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.ValueObjects
{
    public class EventDescription : ValueObject
    {
        public string Description { get; private set; }

        private EventDescription(string eventDescription)
        {
            this.Description = eventDescription;
        }

        public static EventDescription Create(string eventDescription)
        {
            CheckRule(new StringRules(eventDescription, nameof(Description), eventDescription.Length > 1, //For tests
                customError: "Description length should be longer than 100 characters.", canBeNull: true));

            return new EventDescription(eventDescription);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Description;
        }
    }
}