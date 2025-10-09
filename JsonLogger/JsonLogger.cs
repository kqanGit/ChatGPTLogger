using System;
using System.IO;
using System.Text.Json;
using ILogging;

namespace JsonLogger
{
    /// <summary>
    /// JSON file logger implementation that writes log entries to daily JSON files.
    /// </summary>
    public class JsonLogger : ILogger
    {
        /// <summary>
        /// Logs a request and response pair to a daily JSON file.
        /// </summary>
        /// <param name="request">The request message to log</param>
        /// <param name="response">The response message to log</param>
        public void Log(string request, string response)
        {
            // Create log entry object with timestamp
            var logEntry = new
            {
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Request = request,
                Response = response
            };

            // Serialize log entry to JSON string with proper formatting
            string json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Generate filename for daily log file
            string filename = $"log_{DateTime.Now:yyyyMMdd}.json";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            // Initialize empty JSON array if file doesn't exist
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
            }

            // Read existing log entries from file
            string existing = File.ReadAllText(filePath);
            var logs = JsonSerializer.Deserialize<List<object>>(existing) ?? new List<object>();
            
            // Add new log entry to collection
            logs.Add(logEntry);

            // Write updated log collection back to file
            File.WriteAllText(filePath, JsonSerializer.Serialize(logs, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));

            // Log confirmation message
            //Console.WriteLine($"[JsonLogger] Log saved to {filename}");
        }
    }
}
