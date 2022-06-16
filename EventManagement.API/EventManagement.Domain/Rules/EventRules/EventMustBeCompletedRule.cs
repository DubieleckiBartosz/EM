using System;
using EventManagement.Domain.Base;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Domain.Rules.EventRules
{
    public class EventMustBeCompletedRule : IBusinessRule
    {
        private readonly EventTime _eventTime;

        public EventMustBeCompletedRule(EventTime eventTime)
        {
            this._eventTime = eventTime;
        }

        public bool IsBroken()
        {
            var end = this._eventTime.EndDate;
            return end > DateTime.Now;
        }

        public string ErrorMessage => "The event has not been completed yet, so the status cannot be changed.";
    }
}
