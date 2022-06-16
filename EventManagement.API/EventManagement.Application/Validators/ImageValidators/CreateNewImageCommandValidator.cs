using EventManagement.Application.Features.EventImageFeatures.Commands.AddNewImage;
using FluentValidation;

namespace EventManagement.Application.Validators.ImageValidators
{
    public class CreateNewImageCommandValidator : AbstractValidator<CreateNewImageCommand>
    {
        public CreateNewImageCommandValidator()
        {
            this.RuleFor(r => r.EventId).GreaterThan(0);
            this.RuleFor(r => r.ImagePath).NotEmpty();
            this.RuleFor(r => r.ImageTitle).NotEmpty();
            this.RuleFor(r => r.Image).NotNull();
        }
    }
}