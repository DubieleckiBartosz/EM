using EventManagement.Application.Features.EventFeatures.Queries.GetEventDetails;
using FluentValidation;

namespace EventManagement.Application.Validators.EventValidators
{
    public class GetEventDetailsQueryValidator : AbstractValidator<GetEventDetailsQuery>
    {
        public GetEventDetailsQueryValidator()
        {
            this.RuleFor(r => r.EventId).GreaterThan(0);
        }
    }
}