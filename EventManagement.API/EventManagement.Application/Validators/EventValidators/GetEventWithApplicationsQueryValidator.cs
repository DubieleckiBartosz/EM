using EventManagement.Application.Features.EventFeatures.Queries.GetEventWithApplications;
using FluentValidation;

namespace EventManagement.Application.Validators.EventValidators
{
    public class GetEventWithApplicationsQueryValidator : AbstractValidator<GetEventWithApplicationsQuery>
    {
        public GetEventWithApplicationsQueryValidator()
        {
            this.RuleFor(r => r.EventId).GreaterThan(0);
        }
    }
}
