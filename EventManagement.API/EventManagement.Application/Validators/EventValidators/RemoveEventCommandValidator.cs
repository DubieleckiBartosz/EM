using EventManagement.Application.Features.EventFeatures.Commands.CancelEvent;
using FluentValidation;

namespace EventManagement.Application.Validators.EventValidators
{
    public class RemoveEventCommandValidator : AbstractValidator<CancelEventCommand>
    {
        public RemoveEventCommandValidator()
        {
            this.RuleFor(r => r.EventId).GreaterThan(0);
        }
    }
}