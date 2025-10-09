using ILogging;
using System.Xml;

namespace XmlLogger
{
    public class XmlLogger : ILogger
    {
        XmlElement _request;
        XmlElement _response;
        public void Test()
        {
            Console.WriteLine("Test");
        }
        public void Log(string request, string response)
        {
            var now = DateTime.Now;
            var filename = $"log_{now.Year}{now.Month}{now.Day}.xml";
            XmlDocument doc = new XmlDocument();
            if (!File.Exists(filename))
            {
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(decl);

                XmlElement root = doc.CreateElement("Logs");
                doc.AppendChild(root);
                doc.Save(filename);
            }
            else
    {
                doc.Load(filename);
            }
            XmlElement entry = doc.CreateElement("Entry");
            XmlElement time = doc.CreateElement("Time");
            time.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            entry.AppendChild(time);

            _request = doc.CreateElement("Request");
            _request.InnerText = request;
            entry.AppendChild(_request);

            _response = doc.CreateElement("Response");
            _response.InnerText = response;
            entry.AppendChild(_response);
            XmlElement rootElement = doc.DocumentElement!;
            rootElement.AppendChild(entry);
            doc.Save(filename);
        }
    }
}

