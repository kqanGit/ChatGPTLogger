using ILogging;
using System.Xml;

namespace XmlLogger
{
    /// <summary>
    /// XML file logger implementation that writes log entries to daily XML files.
    /// </summary>
    public class XmlLogger : ILogger
    {
        private XmlElement? _request;
        private XmlElement? _response;

        /// <summary>
        /// Test method for debugging purposes.
        /// </summary>
        public void Test()
        {
            Console.WriteLine("XmlLogger Test Method");
        }

        /// <summary>
        /// Logs a request and response pair to a daily XML file.
        /// </summary>
        /// <param name="request">The request message to log</param>
        /// <param name="response">The response message to log</param>
        public void Log(string request, string response)
        {
            // Get current timestamp for filename
            var now = DateTime.Now;
            var filename = $"log_{now.Year}{now.Month:D2}{now.Day:D2}.xml";

            // Initialize XML document
            XmlDocument doc = new XmlDocument();

            // Create new XML file if it doesn't exist
            if (!File.Exists(filename))
            {
                // Create XML declaration
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(decl);

                // Create root element
                XmlElement root = doc.CreateElement("Logs");
                doc.AppendChild(root);
                doc.Save(filename);
            }
            else
            {
                // Load existing XML file
                doc.Load(filename);
            }

            // Create new log entry element
            XmlElement entry = doc.CreateElement("Entry");

            // Add timestamp element
            XmlElement time = doc.CreateElement("Time");
            time.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            entry.AppendChild(time);

            // Add request element
            _request = doc.CreateElement("Request");
            _request.InnerText = request;
            entry.AppendChild(_request);

            // Add response element
            _response = doc.CreateElement("Response");
            _response.InnerText = response;
            entry.AppendChild(_response);

            // Append entry to root element and save
            XmlElement rootElement = doc.DocumentElement!;
            rootElement.AppendChild(entry);
            doc.Save(filename);

            // Log confirmation message
            Console.WriteLine($"[XmlLogger] Log saved to {filename}");
        }
    }
}

