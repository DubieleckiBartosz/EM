using EventManagement.Application.Features.EventApplicationFeatures.Commands.UpdateEventApplication;
using FluentValidation;

namespace EventManagement.Application.Validators.EventApplicationValidators
{
    public class UpdateEventApplicationCommandValidator : AbstractValidator<UpdateEventApplicationCommand>
    {
        public UpdateEventApplicationCommandValidator()
        {
            this.RuleFor(r => r.EventId).GreaterThan(0);
            this.RuleFor(r => r.ApplicationId).GreaterThan(0);
            this.RuleFor(r => r).Must((r) => r.TypePerformance.HasValue || r.DurationInMinutes.HasValue)
                .WithMessage("Performance type or duration must be completed during update.");
     
            When(_ => _.TypePerformance != null, () => { this.RuleFor(r => r.TypePerformance).NotNull(); });
            When(_ => _.DurationInMinutes != null,
                () => { this.RuleFor(r => r.DurationInMinutes).NotNull().ExclusiveBetween(15, 120); });
        }
    }
}