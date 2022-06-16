using EventManagement.Application.Features.OpinionFeatures.Commands.UpdateOpinion;
using FluentValidation;

namespace EventManagement.Application.Validators.OpinionValidators
{
    public class UpdateOpinionCommandValidator : AbstractValidator<UpdateOpinionCommand>
    {
        public UpdateOpinionCommandValidator()
        {
            RuleFor(r => r.Comment).NotEmpty();
            RuleFor(r => r.Stars).InclusiveBetween(1, 5);
        }
    }
}
