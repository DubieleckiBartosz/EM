using System.Collections.Generic;
using EventManagement.Domain.Base;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Rules.PerformerRules
{
    public class OnlyOneProposalForPerformerPerEventRule : IBusinessRule
    {
        private readonly List<PerformanceProposal> _performanceProposals;
        private readonly int _eventId;

        public OnlyOneProposalForPerformerPerEventRule(List<PerformanceProposal> performanceProposals,
            int eventId)
        {
            this._performanceProposals = performanceProposals;
            this._eventId = eventId;
        }

        public bool IsBroken()
        {
            var alreadyExist = this._performanceProposals.Exists(_ => _.EventId == this._eventId);
            return alreadyExist;
        }

        public string ErrorMessage => "The proposal has already been made.";
    }
}