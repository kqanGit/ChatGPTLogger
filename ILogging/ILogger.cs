namespace ILogging
{
    /// <summary>
    /// Interface for logging messages.
    /// </summary>
    public interface ILogger
    {
        void Log(string request, string response);
    }
}
