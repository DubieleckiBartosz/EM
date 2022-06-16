using EventManagement.Application.Features.OpinionFeatures.Commands.RemoveOpinion;
using FluentValidation;

namespace EventManagement.Application.Validators.OpinionValidators
{
    public class RemoveOpinionCommandValidator : AbstractValidator<RemoveOpinionCommand>
    {
        public RemoveOpinionCommandValidator()
        {
            RuleFor(r => r.OpinionId).GreaterThan(0);
            RuleFor(r => r.EventId).GreaterThan(0);
        }
    }
}