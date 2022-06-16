using System;
using EventManagement.Application.Features.EventFeatures.Commands.UpdateEvent;
using EventManagement.Application.Helpers;
using FluentValidation;

namespace EventManagement.Application.Validators.EventValidators
{
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            this.RuleFor(r => r.EventId).GreaterThan(0);
            this.RuleFor(r => r).Must((_) =>
                    _.EventDescription != null || _.StartDate != null || _.EndDate != null || _.PlaceType.HasValue ||
                    _.RecurringEvent.HasValue || _.CategoryType.HasValue || _.EventType.HasValue)
                .WithMessage("Some properties need to be populated to update.");

            this.When(_ => _.EventDescription != null,
                () => RuleFor(r => r.EventDescription).MinimumLength(1)); // Tests

            this.When(_ => _.StartDate != null, () =>
            {
                this.RuleFor(r => r.EndDate).NotNull()
                    .WithMessage("If the start date is not null, the end date should also not be null.");
                this.RuleFor(r => r).Must(_ => _.StartDate.ToDateTime() > DateTime.Now.AddDays(7 * 2));
                this.RuleFor(r => r).Must(_ => _.StartDate.ToDateTime() < _.EndDate.ToDateTime());
            });
        }
    }
}