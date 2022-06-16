using System;
using System.Collections.Generic;
using EventManagement.Application.Strings.Responses;
using FluentValidation.Results;

namespace EventManagement.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException() : base(ResponseStrings.MessageException)
        {
            Errors = new List<string>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            foreach (var itemFailure in failures)
            {
                Errors.Add(itemFailure.ErrorMessage);
            }
        }
    }
}