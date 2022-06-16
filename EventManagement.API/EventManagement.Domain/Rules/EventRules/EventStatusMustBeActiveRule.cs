using EventManagement.Domain.Base;
using EventManagement.Domain.Base.EnumerationClasses;

namespace EventManagement.Domain.Rules.EventRules
{
    public class EventStatusMustBeActiveRule : IBusinessRule
    {
        private readonly EventCurrentStatus _currentStatus;

        public EventStatusMustBeActiveRule(EventCurrentStatus currentStatus)
        {
            this._currentStatus = currentStatus;
        }

        public bool IsBroken()
        {
            return this._currentStatus.Id != EventCurrentStatus.Active.Id;
        }

        public string ErrorMessage => $"In {this._currentStatus} status cannot change the event.";
    }
}