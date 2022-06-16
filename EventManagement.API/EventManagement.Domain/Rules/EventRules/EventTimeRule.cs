using System;
using EventManagement.Domain.Base;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.Rules.EventRules
{
    public class EventTimeRule : BaseRules, IBusinessRule
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        public EventTimeRule(DateTime startDate, DateTime endDate)
        {
            this._startDate = startDate;
            this._endDate = endDate;
        }

        public bool IsBroken()
        {
            if (this._startDate == default || this._endDate == default)
            {
                this.Error = "The start date of the event or the end date of the event is not correct.";
                return true;
            }

            return false;
        }

        public new string ErrorMessage => this.Error;
    }
}