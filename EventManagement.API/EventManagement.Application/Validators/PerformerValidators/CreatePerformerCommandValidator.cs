using EventManagement.Application.Features.PerformerFeatures.Commands.CreatePerformer;
using FluentValidation;

namespace EventManagement.Application.Validators.PerformerValidators
{
    public class CreatePerformerCommandValidator : AbstractValidator<CreatePerformerCommand>
    {
        public CreatePerformerCommandValidator()
        {
            RuleFor(r => r.NumberOfPeople).InclusiveBetween(1, 25);
            RuleFor(r => r.PerformerName).NotEmpty();
            When(_ => _.PerformerMail != null, 
                () => RuleFor(r => r.PerformerMail).NotEmpty().EmailAddress());
        }
    }
}