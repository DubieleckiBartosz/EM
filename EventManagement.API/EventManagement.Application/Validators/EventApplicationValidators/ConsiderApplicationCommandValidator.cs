using EventManagement.Application.Features.EventApplicationFeatures.Commands.ConsiderEventApplication;
using FluentValidation;

namespace EventManagement.Application.Validators.EventApplicationValidators
{
    public class ConsiderApplicationCommandValidator : AbstractValidator<ConsiderApplicationCommand>
    {
        public ConsiderApplicationCommandValidator()
        {
            RuleFor(r => r.ApplicationId).GreaterThan(0);
            RuleFor(r => r.EventId).GreaterThan(0);
        }
    }
}
