using System;

namespace EventManagement.Application.Exceptions
{
    public class DbException : Exception
    {
        public DbException(string message) : base(message)
        {
        }
    }
}