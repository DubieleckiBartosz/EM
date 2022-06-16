using System;
using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.EventRules;

namespace EventManagement.Domain.ValueObjects
{
    public class EventTime : ValueObject
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        private EventTime(DateTime startDate, DateTime endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        public static EventTime Create(DateTime startDate, DateTime endDate)
        {
            CheckRule(new EventTimeRule(startDate, endDate));
            return new EventTime(startDate, endDate);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.StartDate;
            yield return this.EndDate;
        }
    }
}