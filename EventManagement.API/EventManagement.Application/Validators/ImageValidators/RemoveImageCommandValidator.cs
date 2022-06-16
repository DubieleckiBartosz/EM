using EventManagement.Application.Features.EventImageFeatures.Commands.RemoveImage;
using FluentValidation;

namespace EventManagement.Application.Validators.ImageValidators
{
    public class RemoveImageCommandValidator : AbstractValidator<RemoveImageCommand>
    {
        public RemoveImageCommandValidator()
        {
            this.RuleFor(r => r.ImageId).GreaterThan(0);
        }
    }
}
