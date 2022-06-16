using EventManagement.Application.Features.EventImageFeatures.Queries.GetImageById;
using FluentValidation;

namespace EventManagement.Application.Validators.ImageValidators
{
    public class GetImageByIdQueryValidator : AbstractValidator<GetImageByIdQuery>
    {
        public GetImageByIdQueryValidator()
        {
            this.RuleFor(r => r.ImageId).GreaterThan(0);
        }
    }
}
