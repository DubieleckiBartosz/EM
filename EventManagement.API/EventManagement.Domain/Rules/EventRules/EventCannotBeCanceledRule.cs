using EventManagement.Domain.Base;
using EventManagement.Domain.Base.EnumerationClasses;

namespace EventManagement.Domain.Rules.EventRules
{
    public class EventCannotBeCanceledRule : IBusinessRule
    {
        private readonly EventCurrentStatus _status;

        public EventCannotBeCanceledRule(EventCurrentStatus status)
        {
            this._status = status;
        }
        public bool IsBroken()
        {
            return this._status == EventCurrentStatus.Cancelled;
        }

        public string ErrorMessage => "Event is in canceled status.";
    }
}
