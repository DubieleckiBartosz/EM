using System;
using System.Collections.Generic;
using System.Linq;
using EventManagement.Domain.Base;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Rules.EventRules
{
    public class ApplicationCanBeAcceptedRules : IBusinessRule
    {
        private readonly List<EventApplication> _acceptedApplications;
        private readonly int _durationPerformanceToAccepted;
        private readonly DateTime _startEvent;
        private readonly DateTime _endEvent;

        public ApplicationCanBeAcceptedRules(List<EventApplication> acceptedApplications,
            int durationPerformanceToAccepted, DateTime startEvent, DateTime endEvent)
        {
            this._acceptedApplications = acceptedApplications;
            this._durationPerformanceToAccepted = durationPerformanceToAccepted;
            this._startEvent = startEvent;
            this._endEvent = endEvent;
        }

        public bool IsBroken()
        {
            var allMinutesPerformances = this._acceptedApplications?.Sum(_ => _.DurationInMinutes) ?? 0;
            var result = this._endEvent - this._startEvent;
            var remainingEventTime = result.Subtract(TimeSpan.FromMinutes(allMinutesPerformances));
            var time = TimeSpan.FromMinutes(this._durationPerformanceToAccepted);
            var timeToCheck = new TimeSpan(0, (int) time.TotalHours, (int) time.Minutes);

            return remainingEventTime < timeToCheck;
        }

        public string ErrorMessage => "The event is too short to add such a performance.";
    }
}