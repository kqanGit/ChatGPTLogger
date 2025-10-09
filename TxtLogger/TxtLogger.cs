using ILogging;

namespace TxtLogger
{
    /// <summary>
    /// Text file logger implementation that writes log entries to daily text files.
    /// </summary>
    public class TxtLogger : ILogger 
    {
        /// <summary>
        /// Logs a request and response pair to a daily text file.
        /// </summary>
        /// <param name="request">The request message to log</param>
        /// <param name="response">The response message to log</param>
        public void Log(string request, string response)
        {
            // Get current timestamp for filename and log entry
            var now = DateTime.Now;
            var filename = $"log_{now.Year}{now.Month:D2}{now.Day:D2}.txt";

            // Format log entry with timestamp
            string line = $"{now:yyyy-MM-dd HH:mm:ss} | Request: {request} | Response: {response}";

            // Create or append to the log file
            using (var writer = new StreamWriter(filename, append: true))
            {
                writer.WriteLine(line);
            }

            // Log confirmation message
            Console.WriteLine($"[TxtLogger] Log saved to {filename}");
        }
    }
}
