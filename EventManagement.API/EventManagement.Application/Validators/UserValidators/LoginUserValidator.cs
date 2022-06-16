using EventManagement.Application.Models.Authorization;
using FluentValidation;

namespace EventManagement.Application.Validators.UserValidators
{
    public class LoginUserValidator : AbstractValidator<LoginModel>
    {
        public LoginUserValidator()
        {
            RuleFor(r => r.Email).NotEmpty().EmailAddress();
            RuleFor(r => r.Password).NotEmpty();
        }
    }
}
