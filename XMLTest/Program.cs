using System;
using ILogging;
using XmlLogger;

namespace XMLTest
{
    class Program 
    {
        static void Main(string[] args)
        {
            ILogger logger = new XmlLogger.XmlLogger();
            string request = "Sample request data";
            string response = "Sample response data";
            logger.Log(request, response);
            Console.WriteLine("Log entry created.");

        }
    }
}