using ChatGPTLoggerMockService;
using ILogging;
using System;
using System.Reflection;

namespace ChatGPTLoggerConsole {     
    internal class App
    {
        static void Main(string[] args)
        {
            ChatGPTService service = new ChatGPTService();
            Console.WriteLine("=== ChatGPTLogger Console ===");
            Console.WriteLine("Type 'exit' to quit.");
            Console.WriteLine();
            // Select plugins to load
            string pluginDir = Path.Combine(AppContext.BaseDirectory, "LoggerPlugins");
            var fis = new DirectoryInfo(pluginDir).GetFiles("*.dll");
            List<(string, ILogger)> loggers = new List<(string, ILogger)>();

            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass
                        && typeof(ILogger).IsAssignableFrom(type))
                    {
                        var item = (ILogger)Activator.CreateInstance(type)!;
                        loggers.Add((fi.Name, item));
                    }
                }
            }

            ILogger? logger = null;

            if (loggers.Count > 0)
            {
                Console.WriteLine($"Select log type to save:");
                for (int i = 0; i < loggers.Count; i++)
                {
                    string loggerType = loggers[i].Item2.GetType().Name;
                    string loggerName = loggerType.Substring(0, loggerType.Length - "Logger".Length);
                    {
                        Console.WriteLine($"{i} - {loggerName}");
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Select type (0 - {0}): ", loggers.Count - 1);
                int choice = int.Parse(Console.ReadLine()!);
                logger = loggers[choice].Item2;
                string chosenLoggerType = logger.GetType().Name;
                string chosenLoggerName = chosenLoggerType.Substring(0, chosenLoggerType.Length - "Logger".Length);
                Console.WriteLine($"Using {chosenLoggerName} logger.");
                Console.WriteLine();
                Console.WriteLine("=================================================");
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine("Start chatting with ChatGPT:");
            Console.WriteLine();
            while (true)
            {
                Console.Write("You: ");
                string input = Console.ReadLine()!;
                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                Request request = new Request(input);
                Response response = service.sendRequest(request);

                Console.WriteLine($"ChatGPT Response: {response.Body}");
                Console.WriteLine();
                logger!.Log(request.Body, response.Body!);
            }
            Console.WriteLine("Session end"); 
            Console.WriteLine();
        }
    }
}