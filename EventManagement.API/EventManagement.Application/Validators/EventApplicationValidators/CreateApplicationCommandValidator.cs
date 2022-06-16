using EventManagement.Application.Features.EventApplicationFeatures.Commands.CreateEventApplication;
using FluentValidation;

namespace EventManagement.Application.Validators.EventApplicationValidators
{
    public class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
    {
        public CreateApplicationCommandValidator()
        {
            this.RuleFor(r => r.DurationInMinutes).ExclusiveBetween(15, 120);
            this.RuleFor(r => r.EventId).GreaterThan(0);
        }
    }
}
