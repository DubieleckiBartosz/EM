using EventManagement.Application.Models.Authorization;
using FluentValidation;

namespace EventManagement.Application.Validators.UserValidators
{
    public class RegisterUserValidator : AbstractValidator<RegisterModel>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Name is required.");
            RuleFor(c => c.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(s => s.UserName).NotEmpty().WithMessage("Field UserName is required.");
            RuleFor(c => c.ConfirmPassword).NotEmpty().Equal(x => x.Password)
                .WithMessage("Your passwords are different.");
            RuleFor(x => x.Email).EmailValidator();
            RuleFor(c => c.Password).PasswordValidator();
        }
    }
}
