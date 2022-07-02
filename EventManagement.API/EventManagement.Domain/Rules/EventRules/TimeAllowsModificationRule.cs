using System;
using EventManagement.Domain.Base;

namespace EventManagement.Domain.Rules.EventRules
{
    public class TimeAllowsModificationRule : IBusinessRule
    {
        private readonly DateTime _eventStart;
        private readonly int _daysToStartEventWithoutModification;

        public TimeAllowsModificationRule(DateTime eventStart, int daysToStartEventWithoutModification)
        {
            this._eventStart = eventStart;
            this._daysToStartEventWithoutModification = daysToStartEventWithoutModification;
        }

        public bool IsBroken()
        {
            var finalModificationDate = this._eventStart.AddDays(-this._daysToStartEventWithoutModification);
            if (finalModificationDate < DateTime.Now)
            {
                return true;
            }

            return false;
        }

        public string ErrorMessage => RuleErrorMessage.TimeAllowsModificationRuleMessage;
    }
}
