using System;
using EventManagement.Domain.Base;
using EventManagement.Domain.Rules.Common;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Domain.Rules.EventRules
{
    public class UpdatingEventTimeShouldBeEarlyEnoughRule : BaseRules, IBusinessRule
    {
        private readonly EventTime _newTime;
        private readonly DateTime _currentStartTime;

        public UpdatingEventTimeShouldBeEarlyEnoughRule(EventTime newTime, DateTime currentStartTime)
        {
            this._newTime = newTime;
            this._currentStartTime = currentStartTime;
        }

        public bool IsBroken()
        {
            if (DateTime.Now.AddDays(14) > this._currentStartTime)
            {
                this.Error = "There are less than 2 weeks left until the event, so the date cannot be changed anymore.";
                return true;
            }

            var x = this._newTime.StartDate < this._currentStartTime;
            var y = DateTime.Now.AddDays(14) < this._newTime.StartDate;
            if (x && !y)
            {
                this.Error = "Date must be later than 2 weeks from now.";
                return true;
            }

            return false;
        }

        public new string ErrorMessage => this.Error;
    }
}