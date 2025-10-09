using System.Text.RegularExpressions;
using PowerShellHandler;

namespace ChatGPTLoggerService
{
    public static class ChatBot
    {
        /// <summary>
        /// Chạy chatbot trên console. Nhập close để kết thúc.
        /// </summary>
        static void Start()
        {
            string message = string.Empty;
            do
            {
                message = Console.ReadLine()!;
                if (message == string.Empty) continue;
                if (message == "close") break;
                string response = Chat(message);
                Console.WriteLine(response);
            } while (message != "close");
        }

        /// <summary>
        /// Gửi request đến ollama
        /// </summary>
        /// <param name="message">Prompt được sử dụng</param>
        /// <returns>Kết quả response</returns>
        static string Chat(string message)
        {
            //Thay bằng địa chỉ ngrok được tạo
            string address = "https://unaggravated-benedictory-paige.ngrok-free.dev";

            string cmd = $"(Invoke-WebRequest -method POST -Body " +
                $"'{{\"model\":\"gpt-oss:20b\", \"prompt\":\"{message}\", \"stream\": false}}' " +
                $"-uri {address}/api/generate).Content | ConvertFrom-json";

            //Chạy lệnh powershell gửi request
            string chatResult = PowerShellHandler.PowerShellHandler.ExecCommand(cmd);

            //Lấy phần response
            int responseStart = chatResult.IndexOf("response             :") + ("response             :".Length);
            int responseEnd = chatResult.IndexOf("thinking             :");
            string chatResponse = chatResult.Substring(responseStart, (responseEnd - responseStart));

            //Loại bỏ space thừa 
            chatResponse = chatResponse.Replace("\r", string.Empty);
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            chatResponse = regex.Replace(chatResponse, " ");

            return chatResponse;
        }
    }
}
