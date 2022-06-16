using EventManagement.Domain.Base;
using EventManagement.Domain.Base.EnumerationClasses;

namespace EventManagement.Domain.Rules.EventRules
{
    public class ApplicantHasAccessWhenStatusIsInProgressRule : IBusinessRule
    {
        private readonly StatusApplication _currentStatusApplication;

        public ApplicantHasAccessWhenStatusIsInProgressRule(StatusApplication currentStatusApplication)
        {
            this._currentStatusApplication = currentStatusApplication;
        }


        public bool IsBroken()
        {
            return this._currentStatusApplication.Id != StatusApplication.InProgress.Id;
        }

        public string ErrorMessage => "You cannot change status.";
    }
}