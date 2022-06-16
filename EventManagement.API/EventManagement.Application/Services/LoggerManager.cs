using System;
using EventManagement.Application.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventManagement.Application.Services
{
    public class LoggerManager<T> : ILoggerManager<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerManager(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(object obj, string message = null) =>
            _logger.LogInformation(this.GetMessage(obj, message));

        public void LogWarning(object obj, string message = null) => _logger.LogWarning(this.GetMessage(obj, message));

        public void LogError(object obj, string message = null) => _logger.LogError(this.GetMessage(obj, message));

        public void LogCritical(object obj, string message = null) =>
            _logger.LogCritical(this.GetMessage(obj, message));

        private string GetMessage(object obj, string message) => obj == null && message == null
            ? throw new ArgumentNullException(nameof(message))
            : message ?? JsonConvert.SerializeObject(obj);
    }
}