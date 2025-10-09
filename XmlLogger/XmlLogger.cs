using ILogging;
using System.Xml;

namespace XmlLogger
{
    public class XmlLogger : ILogger
    {
        public string Log(string message)
        {
            var now = DateTime.Now;
            var filename = $"log{now.Year}{now.Month}{now.Day}.xml";

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

            XmlElement msg = doc.CreateElement("Message");
            msg.InnerText = message;
            entry.AppendChild(msg);

            XmlElement rootElement = doc.DocumentElement!;
            rootElement.AppendChild(entry);

            doc.Save(filename);

            return "";
        }
    }
}

