using System.Collections.Generic;
using System.Linq;
using EventManagement.Domain.Base;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Rules.EventRules
{
    public class OnlyOneApplicationPerEventRule : IBusinessRule
    {
        private readonly int _performerId;
        private readonly List<EventApplication> _applications;

        public OnlyOneApplicationPerEventRule(int performerId, List<EventApplication> applications)
        {
            this._performerId = performerId;
            this._applications = applications;
        }

        public bool IsBroken()
        {
            var alreadyExist = this._applications.Any(_ =>
                _.PerformerId == this._performerId && _.CurrentStatus.Id != StatusApplication.Rejected.Id);
            return alreadyExist;
        }

        public string ErrorMessage => "An application has been made to participate in this event.";
    }
}