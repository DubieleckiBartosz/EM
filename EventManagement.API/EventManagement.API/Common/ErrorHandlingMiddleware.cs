using System;
using System.Net;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace EventManagement.API.Common
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager<ErrorHandlingMiddleware> _loggerManager;

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerManager<ErrorHandlingMiddleware> loggerManager)
        {
            this._next = next;
            this._loggerManager = loggerManager;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this._next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                var model = Response<string>.Error(response.StatusCode > 500
                    ? ResponseStrings.ServerError
                    : ex?.Message);

                response.ContentType = "application/json";
                response.StatusCode = ex switch
                {
                    AuthException e => e.Code,
                    NotFoundException e => (int) HttpStatusCode.NotFound,
                    ArgumentNullException e => (int) HttpStatusCode.BadRequest,
                    ArgumentException e => (int) HttpStatusCode.BadRequest,
                    ValidationException e => this.AssignErrors(e, ref model),
                    DbException e => (int) HttpStatusCode.BadRequest,
                    EventManagementException e => e.Code,
                    _ => (int) HttpStatusCode.InternalServerError
                };

                _loggerManager.LogError(ex?.Message);
                await response.WriteAsJsonAsync(model);
            }
        }

        private int AssignErrors(ValidationException ex, ref Response<string> response)
        {
            response.Errors = ex.Errors;
            return (int) HttpStatusCode.BadRequest;
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}