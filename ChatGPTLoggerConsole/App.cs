using ChatGPTLoggerService;
using ILogging;
using System.Reflection;

namespace ChatGPTLoggerConsole 
{     
    /// <summary>
    /// Main application class for the ChatGPT Logger Console application.
    /// </summary>
    internal class App
    {
        /// <summary>
        /// Entry point of the application. Handles logger plugin loading and chat session management.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            // Initialize ChatBot service
            ChatBotService service = new ChatBotService();
            
            // Display application header
            Console.WriteLine("=== ChatGPTLogger Console ===");
            Console.WriteLine("Type 'exit' to quit.");
            Console.WriteLine();
            
            // Load logger plugins from the LoggerPlugins directory
            string pluginDir = Path.Combine(AppContext.BaseDirectory, "LoggerPlugins");
            var fis = new DirectoryInfo(pluginDir).GetFiles("*.dll");
            List<(string, ILogger)> loggers = new List<(string, ILogger)>();

            // Scan each DLL file for ILogger implementations
            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    // Check if type implements ILogger interface
                    if (type.IsClass && typeof(ILogger).IsAssignableFrom(type))
                    {
                        var item = (ILogger)Activator.CreateInstance(type)!;
                        loggers.Add((fi.Name, item));
                    }
                }
            }

            ILogger? logger = null;

            // Allow user to select a logger if any were found
            if (loggers.Count > 0)
            {
                Console.WriteLine($"Select log type to save:");
                
                // Display available logger options
                for (int i = 0; i < loggers.Count; i++)
                {
                    string loggerType = loggers[i].Item2.GetType().Name;
                    // Remove "Logger" suffix from display name
                    string loggerName = loggerType.Substring(0, loggerType.Length - "Logger".Length);
                    Console.WriteLine($"{i} - {loggerName}");
                }
                
                Console.WriteLine();
                Console.WriteLine("Select type (0 - {0}): ", loggers.Count - 1);
                
                // Get user's logger choice
                int choice = int.Parse(Console.ReadLine()!);
                logger = loggers[choice].Item2;
                
                // Display selected logger
                string chosenLoggerType = logger.GetType().Name;
                string chosenLoggerName = chosenLoggerType.Substring(0, chosenLoggerType.Length - "Logger".Length);
                Console.WriteLine($"Using {chosenLoggerName} logger.");
                Console.WriteLine();
                Console.WriteLine("=================================================");
                Console.WriteLine();
            }

            // Start chat session
            Console.WriteLine("Start chatting with ChatGPT:");
            Console.WriteLine();
            
            // Main chat loop
            while (true)
            {
                Console.Write("You: ");
                string input = Console.ReadLine()!;
                
                // Exit condition
                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                try
                {
                    // Create request and send to ChatBot service
                    var request = new Request(input);
                    var response = service.SendRequest(request);

                    // Process successful response
                    if (response.IsSuccess)
                    {
                        Console.WriteLine($"ChatBot: {response.Body}");
                        Console.WriteLine();
                        
                        // Log the conversation if a logger is selected
                        logger?.Log(request.Body, response.Body!);
                    }
                    else
                    {
                        // Display error message
                        Console.WriteLine($"Error: {response.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            
            // Session end message
            Console.WriteLine("Session end"); 
            Console.WriteLine();
        }
    }
}