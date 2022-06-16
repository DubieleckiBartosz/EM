using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventManagement.Application.Wrappers
{
    public class Response<T>
    {
        public bool Success { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string> Errors { get; set; }

        public T Data { get; }

        //For middleware
        public Response()
        {
            this.Success = false;
        }
        private Response(T data, bool success, string message, IEnumerable<string> errors)
        {
            this.Data = data;
            this.Success = success;
            this.Message = message;
            this.Errors = errors;
        }

        public static Response<T> Ok(string message)
        {
            return new Response<T>(data: default(T), success: true, message: message, errors: null);
        }
        public static Response<T> Ok(T data)
        {
            return new Response<T>(data: data, success: true, message: null, errors: null);
        }

        public static Response<T> Ok(T data, string message)
        {
            return new Response<T>(data: data, success: true, message: message, errors: null);
        }

        public static Response<T> Error(string message)
        {
            return new Response<T>(default(T), success: false, message: message, errors: null);
        }

        public static Response<T> Error(IEnumerable<string> errors)
        {
            return new Response<T>(default(T), success: false, message: null, errors: errors);
        }
    }
}