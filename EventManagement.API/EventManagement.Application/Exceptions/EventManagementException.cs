using System;
using System.Collections.Generic;
using System.Net;

namespace EventManagement.Application.Exceptions
{
    public class EventManagementException : Exception
    {
        public int Code { get; }
        public IEnumerable<string> Errors { get; }

        public EventManagementException(string error, HttpStatusCode code = HttpStatusCode.BadRequest) : base(
            message: error)
        {
            this.Code = (int) code;
            this.Errors = new List<string>();
        }

        public EventManagementException(string error, IEnumerable<string> errors, HttpStatusCode code) : this(error,
            code)
        {
            this.Errors = errors;
        }
    }
}