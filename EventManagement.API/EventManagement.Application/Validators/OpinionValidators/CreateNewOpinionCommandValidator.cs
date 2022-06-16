using EventManagement.Application.Features.OpinionFeatures.Commands.CreateOpinion;
using FluentValidation;

namespace EventManagement.Application.Validators.OpinionValidators
{
    public class CreateNewOpinionCommandValidator : AbstractValidator<CreateNewOpinionCommand>
    {
        public CreateNewOpinionCommandValidator()
        {
            RuleFor(r => r.Comment).NotEmpty();
            RuleFor(r => r.EventId).GreaterThan(0);
            RuleFor(r => r.Stars).InclusiveBetween(1, 5);
        }
    }
}
