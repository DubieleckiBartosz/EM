using EventManagement.Application.Features.EventProposalFeatures.Commands.RemoveProposal;
using FluentValidation;

namespace EventManagement.Application.Validators.ProposalValidators
{
    public class RemoveProposalCommandValidator : AbstractValidator<RemoveProposalCommand>
    {
        public RemoveProposalCommandValidator()
        {
            RuleFor(r => r.ProposalId).GreaterThan(0);
        }
    }
}
