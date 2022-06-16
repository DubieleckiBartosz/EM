using EventManagement.Application.Features.PerformerFeatures.Commands.UpdatePerformer;
using FluentValidation;

namespace EventManagement.Application.Validators.PerformerValidators
{
    public class UpdatePerformerCommandValidator : AbstractValidator<UpdatePerformerCommand>
    {
        public UpdatePerformerCommandValidator()
        {
            RuleFor(r => r.NumberOfPeople).InclusiveBetween(1, 25);
            When(_ => _.PerformerMail != null,
                () => RuleFor(r => r.PerformerMail).NotEmpty().EmailAddress());
        }
    }
}
