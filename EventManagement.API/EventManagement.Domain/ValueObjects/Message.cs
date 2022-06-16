using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.ValueObjects
{
    public class Message : ValueObject
    {
        public string MessageValue { get; private set; }

        private Message(string message)
        {
            this.MessageValue = message;
        }

        public static Message CreateMessage(string message)
        {
            CheckRule(new StringRules(message, "Message"));
            return new Message(message);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.MessageValue;
        }
    }
}