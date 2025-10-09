using ILogging;

namespace TxtLogger
{
    public class TxtLogger : ILogger 
    {
        public void Log(string request, string response)
        {
            var now = DateTime.Now;
            var filename = $"log_{now.Year}{now.Month}{now.Day}.txt";

            string line = $"{now:yy:MM:dd HH:mm:ss} | Request: {request} | Response: {response}";

            if (!File.Exists(filename))
            {
                using (var writer = new StreamWriter(filename, append: false))
                {
                    writer.WriteLine(line);
                }
            }
            else
            {
                using (var writer = new StreamWriter(filename, append: true))
                {
                    writer.WriteLine(line);
                }
            }
            return ;
        }
    }
}
