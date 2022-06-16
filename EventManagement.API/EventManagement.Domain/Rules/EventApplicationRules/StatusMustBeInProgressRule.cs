using EventManagement.Domain.Base;
using EventManagement.Domain.Base.EnumerationClasses;

namespace EventManagement.Domain.Rules.EventApplicationRules
{
   public  class StatusMustBeInProgressRule : IBusinessRule
    {
        private readonly StatusApplication _currentStatusApplication;

        public StatusMustBeInProgressRule(StatusApplication currentStatusApplication)
        {
            this._currentStatusApplication = currentStatusApplication;
        }
        public bool IsBroken()
        {
            return this._currentStatusApplication != StatusApplication.InProgress;
        }

        public string ErrorMessage =>
            $"The application status is {this._currentStatusApplication}. You cannot change this status.";
    }
}
