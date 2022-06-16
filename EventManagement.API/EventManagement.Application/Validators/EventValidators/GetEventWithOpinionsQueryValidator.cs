using EventManagement.Application.Features.EventFeatures.Queries.GetEventWithOpinions;
using FluentValidation;

namespace EventManagement.Application.Validators.EventValidators
{
    public class GetEventWithOpinionsQueryValidator : AbstractValidator<GetEventWithOpinionsQuery>
    {
        private readonly string[] _availableName = new[]
        {
            "OpinionId", "New", "Stars"
        };

        public GetEventWithOpinionsQueryValidator()
        {
            this.RuleFor(r => r.EventId).GreaterThan(0);
            this.When(_ => _.SortModel != null,
                () => this.RuleFor(r => r.SortModel).SetValidator(new SortValidator(this._availableName)));
        }
    }
}