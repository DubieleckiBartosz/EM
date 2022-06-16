using System;
using System.Linq;
using EventManagement.Application.Features.Search;
using FluentValidation;

namespace EventManagement.Application.Validators
{
    public class SortValidator : AbstractValidator<SortModelQuery>
    {
        public SortValidator(string [] availableNames = null)
        {
            When(r => r.Name != null, () =>
            {
                RuleFor(r => r.Name).Custom((sort, context) =>
                {
                    if (!string.IsNullOrWhiteSpace(sort) &&
                        !availableNames.Contains(sort, StringComparer.OrdinalIgnoreCase))
                    {
                        context.AddFailure("SortName",
                            $"Sort name must in [{string.Join(", ", availableNames)}]");
                    }
                });
            });
        }
    }
}