using System;
using System.Net;

namespace EventManagement.Application.Exceptions
{
    public class AuthException : Exception
    {
        public int Code { get; set; }

        public AuthException(string error, HttpStatusCode code = HttpStatusCode.BadRequest) : base(error)
        {
            this.Code = (int)code;
        }
    }
}