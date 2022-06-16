using FluentValidation;

namespace EventManagement.Application.Validators
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string> PasswordValidator<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.NotEmpty().Length(6, 450)
                .Matches("[A-Z]")
                .Matches("[a-z]")
                .Matches("[0-9]")
                .Matches("[^a-zA-Z0-9]");
        }

        public static IRuleBuilderOptions<T, string> EmailValidator<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.NotEmpty().WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");
        }
    }
}
