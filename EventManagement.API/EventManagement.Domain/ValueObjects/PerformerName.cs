using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.ValueObjects
{
    public class PerformerName : ValueObject
    {
        public string NameValue { get; private set; }

        private PerformerName(string nameValue)
        {
            this.NameValue = nameValue;
        }

        public static PerformerName Create(string performerName)
        {
            CheckRule(new StringRules(performerName, "Performer name"));
            return new PerformerName(performerName);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.NameValue;
        }
    }
}