using EventManagement.Application.Features.EventFeatures.Queries.GetEventsBySearch;
using FluentValidation;

namespace EventManagement.Application.Validators.EventValidators
{
    public class SearchEventsQueryValidator : AbstractValidator<SearchEventsQuery>
    {
        private readonly string[] _availableName = new[]
        {
            "EventId", "EventName", "StartDate", "EndDate",
            "RecurringEvent", "City", "EventCategory", "EventType",
            "CurrentStatus"
        };

        public SearchEventsQueryValidator()
        {
            this.When(_ => _.SortModel != null,
                () => this.RuleFor(r => r.SortModel).SetValidator(new SortValidator(this._availableName)));
        }
    }
}