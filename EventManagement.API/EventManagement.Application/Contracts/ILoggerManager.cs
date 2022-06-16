namespace EventManagement.Application.Contracts
{
    public interface ILoggerManager<T>
    {
        void LogInformation(object obj, string message = null);
        void LogWarning(object obj, string message = null);
        void LogError(object obj, string message = null);
        void LogCritical(object obj, string message = null);
    }
}