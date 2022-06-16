using System;
using EventManagement.Application.Features.EventProposalFeatures.Commands.CreateProposal;
using FluentValidation;

namespace EventManagement.Application.Validators.ProposalValidators
{
    public class CreateProposalCommandValidator : AbstractValidator<CreateProposalCommand>
    {
        public CreateProposalCommandValidator()
        {
            RuleFor(r => r.PerformerId).GreaterThan(0);
            RuleFor(r => r.EventId).GreaterThan(0);
            RuleFor(r => r.Message).NotEmpty();
            RuleFor(r => r.ActiveTo).Must(_ => _ > DateTime.Now.AddDays(1));
        }
    }
}