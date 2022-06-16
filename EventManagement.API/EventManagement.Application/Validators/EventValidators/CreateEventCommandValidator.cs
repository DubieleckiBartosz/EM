using System;
using EventManagement.Application.Features.EventFeatures.Commands.CreateEvent;
using EventManagement.Application.Helpers;
using FluentValidation;

namespace EventManagement.Application.Validators.EventValidators
{
    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            this.RuleFor(r => r.EventName).NotEmpty().Length(3, 50);
            this.RuleFor(r => r.City).NotEmpty().Length(3,50);
            this.RuleFor(r => r.Street).NotEmpty().Length(3, 50);
            this.RuleFor(r => r.NumberStreet).NotEmpty().Length(3, 10);
            this.RuleFor(r => r.PostalCode).NotEmpty().Length(3, 10);
            this.RuleFor(r => r.StartDate).Must(r => r.ToDateTime() > DateTime.Now.AddDays(7 * 2));
            this.RuleFor(r => r).Must(r => r.StartDate.ToDateTime() < r.EndDate.ToDateTime());
            this.When(_ => _.EventDescription != null, () => 
                this.RuleFor(r => r.EventDescription).MinimumLength(1)); // For tests
        }
    }
}