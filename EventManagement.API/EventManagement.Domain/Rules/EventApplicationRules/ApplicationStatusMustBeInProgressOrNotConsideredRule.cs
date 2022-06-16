using System;
using EventManagement.Domain.Base;
using EventManagement.Domain.Base.EnumerationClasses;

namespace EventManagement.Domain.Rules.EventApplicationRules
{
    public class ApplicationStatusMustBeInProgressOrNotConsideredRule : IBusinessRule
    {
        private readonly StatusApplication _currentStatusApplication;

        public ApplicationStatusMustBeInProgressOrNotConsideredRule(StatusApplication currentStatusApplication)
        {
            this._currentStatusApplication = currentStatusApplication ?? throw new ArgumentNullException(nameof(currentStatusApplication));
        }

        public bool IsBroken()
        {
            if (this._currentStatusApplication.Id == StatusApplication.InProgress.Id ||
                this._currentStatusApplication.Id == StatusApplication.NotConsidered.Id)
            {
                return false;
            }

            return true;
        }

        public string ErrorMessage => $"You cannot update in '{ this._currentStatusApplication.Name}' status.";
    }
}