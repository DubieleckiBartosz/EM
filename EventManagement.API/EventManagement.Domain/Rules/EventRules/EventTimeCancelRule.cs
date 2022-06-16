using System;
using EventManagement.Domain.Base;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.Rules.EventRules
{
    public class EventTimeCancelRule : BaseRules, IBusinessRule
    {
        private readonly DateTime _startDate;
        private readonly int _daysWithoutCancellation;

        public EventTimeCancelRule(DateTime startDate, int daysWithoutCancellation)
        {
            this._startDate = startDate;
            this._daysWithoutCancellation = daysWithoutCancellation;
        }

        public bool IsBroken()
        {
            var now = DateTime.Now;
            if (this._startDate < now)
            {
                this.Error = "An event that has already taken place cannot be canceled.";
                return true;
            }

            var finalCancellationDate = this._startDate.AddDays(-_daysWithoutCancellation);
            if (finalCancellationDate < now)
            {
                var daysToStart = (this._startDate - now).Days;
                this.Error = $"There are {daysToStart} days left before the event starts, so it's too late to cancel.";
                return true;
            }

            return false;
        }

        public new string ErrorMessage => this.Error;
    }
}