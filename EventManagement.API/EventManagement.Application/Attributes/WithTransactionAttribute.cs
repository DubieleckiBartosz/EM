using System;

namespace EventManagement.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class WithTransactionAttribute : Attribute
    {
    }
}