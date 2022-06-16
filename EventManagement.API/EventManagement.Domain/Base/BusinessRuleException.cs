using System;

namespace EventManagement.Domain.Base
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(IBusinessRule rule) : base(rule.ErrorMessage)
        {
        }
    }
}