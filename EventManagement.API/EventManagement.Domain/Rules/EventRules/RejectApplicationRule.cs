using EventManagement.Domain.Base;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Rules.EventRules
{
    public class RejectApplicationRule : IBusinessRule
    {
        private readonly EventApplication _eventApplication;

        public RejectApplicationRule(EventApplication eventApplication)
        {
            this._eventApplication = eventApplication;
        }

        public bool IsBroken()
        {
            return this._eventApplication.CurrentStatus == StatusApplication.ConsideredPositively;
        }

        public string ErrorMessage => "This application cannot be rejected.";
    }
}