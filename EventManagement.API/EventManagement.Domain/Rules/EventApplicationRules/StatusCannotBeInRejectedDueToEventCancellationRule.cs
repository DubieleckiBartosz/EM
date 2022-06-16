using EventManagement.Domain.Base;
using EventManagement.Domain.Base.EnumerationClasses;

namespace EventManagement.Domain.Rules.EventApplicationRules
{
    public class StatusCannotBeInRejectedDueToEventCancellationRule : IBusinessRule
    {
        private readonly StatusApplication _currentStatusApplication;

        public StatusCannotBeInRejectedDueToEventCancellationRule(StatusApplication currentStatusApplication)
        {
            this._currentStatusApplication = currentStatusApplication;
        }
        public bool IsBroken()
        {
            return this._currentStatusApplication == StatusApplication.RejectedDueToEventCancellation;
        }

        public string ErrorMessage => "You cannot change the status when the event is canceled.";
    }
}
