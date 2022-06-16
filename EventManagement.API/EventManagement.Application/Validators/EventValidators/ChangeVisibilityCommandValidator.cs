using EventManagement.Application.Features.EventFeatures.Commands.ChangeVisibilityEvent;
using FluentValidation;

namespace EventManagement.Application.Validators.EventValidators
{
    public class ChangeVisibilityCommandValidator : AbstractValidator<ChangeVisibilityCommand>
    {
        public ChangeVisibilityCommandValidator()
        {
            this.RuleFor(r => r.EventId).GreaterThan(0);
        }
    }
}