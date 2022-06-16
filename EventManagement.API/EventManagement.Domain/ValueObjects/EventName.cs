using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.ValueObjects
{
    public class EventName : ValueObject
    {
        public string Name { get; private set; }

        private EventName(string eventName)
        {
            this.Name = eventName;
        }

        public static EventName Create(string eventName)
        {
            CheckRule(new StringRules(eventName, "Event name", eventName.Length is > 3 and < 100,
                customError: "Event Name length should be between 3 and 50 characters."));
            return new EventName(eventName);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Name;
        }
    }
}