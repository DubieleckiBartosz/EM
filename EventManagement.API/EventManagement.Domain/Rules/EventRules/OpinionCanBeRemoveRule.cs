using EventManagement.Domain.Base;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.Rules.EventRules
{
    public class OpinionCanBeRemoveRule : BaseRules, IBusinessRule
    {
        private readonly int? _createdBy;
        private readonly int _attemptToRemoveBy;
        private readonly bool _hasSuperAccess;

        public OpinionCanBeRemoveRule(int? createdBy, int attemptToRemoveBy, bool hasSuperAccess)
        {
            this._createdBy = createdBy;
            this._attemptToRemoveBy = attemptToRemoveBy;
            this._hasSuperAccess = hasSuperAccess;
        }

        public bool IsBroken()
        {
            if (this._hasSuperAccess)
            {
                return false;
            }

            if (!_createdBy.HasValue)
            {
                this.Error = "The incognito comment cannot be deleted.";
                return true;
            }

            if (this._createdBy != this._attemptToRemoveBy)
            {
                this.Error = "Only your comments can be deleted.";
                return true;
            }

            return false;
        }

        public new string ErrorMessage => this.Error;
    }
}