using System;
using System.IO;
using System.Text.Json;
using ILogging;

namespace JsonLogger
{
    public class JsonLogger : ILogger
    {
        public void Log(string request, string response)
        {
            // Tạo đối tượng log
            var logEntry = new
            {
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Request = request,
                Response = response
            };

            // Chuyển sang chuỗi JSON
            string json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Ghi ra file JSON
            string filename = $"log_{DateTime.Now:yyyyMMdd}.json";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            // Nếu file chưa có thì tạo mảng JSON rỗng
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
            }

            // Đọc file hiện tại, thêm log mới
            string existing = File.ReadAllText(filePath);
            var logs = JsonSerializer.Deserialize<List<object>>(existing) ?? new List<object>();
            logs.Add(logEntry);

            // Ghi lại toàn bộ file
            File.WriteAllText(filePath, JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true }));

            Console.WriteLine($"[JsonLogger] Log saved to {filename}");
        }
    }
}
