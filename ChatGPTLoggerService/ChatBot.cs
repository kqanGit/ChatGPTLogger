using System.Text.RegularExpressions;
using PowerShellHandler;

namespace ChatGPTLoggerService
{
    public class Response
    {
        public string? Body { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class Request
    {
        private string _body = string.Empty;
        public string Body
        {
            get => _body;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Request body cannot be empty");
                }
                _body = value;
            }
        }

        public string Model { get; set; } = "gpt-oss:20b";
        public bool Stream { get; set; } = false;

        public Request(string body)
        {
            Body = body;
        }

        public Request(string body, string model) : this(body)
        {
            Model = model;
        }   
    }

    public class ChatBotService
    {
        private readonly string _apiAddress;

        public ChatBotService(string apiAddress = "https://unglaring-unsavoured-elene.ngrok-free.dev")
        {
            _apiAddress = apiAddress;
        }

        /// <summary>
        /// Gửi request đến ollama và trả về response
        /// </summary>
        /// <param name="request">ChatBot request object</param>
        /// <returns>ChatBot response object</returns>
        public Response SendRequest(Request request)
        {
            try
            {
                // Tạo JSON payload
                var jsonPayload = $"{{\"model\":\"{request.Model}\", \"prompt\":\"{request.Body.Replace("\"", "\\\"").Replace("\\", "\\\\")}\", \"stream\": {request.Stream.ToString().ToLower()}}}";
                
                // Encode thành Base64 để tránh vấn đề escape
                var bytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
                var base64Payload = Convert.ToBase64String(bytes);

                // PowerShell command sử dụng Base64
                string cmd = $@"
                    $payload = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{base64Payload}'))
                    (Invoke-WebRequest -Method POST -Body $payload -Uri {_apiAddress}/api/generate -ContentType 'application/json').Content | ConvertFrom-Json
                ";

                //Chạy lệnh powershell gửi request
                string chatResult = PowerShellHandler.PowerShellHandler.ExecCommand(cmd);

                //Lấy phần response
                string processedResponse = ProcessResponse(chatResult);

                return new Response
                {
                    Body = processedResponse,
                    IsSuccess = true,
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Body = null,
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Xử lý response từ API
        /// </summary>
        /// <param name="rawResponse">Raw response từ API</param>
        /// <returns>Processed response string</returns>
        private static string ProcessResponse(string rawResponse)
        {
            //Lấy phần response
            int responseStart = rawResponse.IndexOf("response             :") + ("response             :".Length);
            int responseEnd = rawResponse.IndexOf("thinking             :");
            
            if (responseStart < 0 || responseEnd < 0)
            {
                return rawResponse; // Trả về raw nếu không tìm thấy pattern
            }

            string chatResponse = rawResponse.Substring(responseStart, (responseEnd - responseStart));

            //Loại bỏ space thừa 
            chatResponse = chatResponse.Replace("\r", string.Empty);
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            chatResponse = regex.Replace(chatResponse, " ");

            return chatResponse.Trim();
        }
    }

    public static class ChatBot
    {
        /// <summary>
        /// Chạy chatbot trên console. Nhập close để kết thúc.
        /// </summary>
        public static void Start()
        {
            var chatService = new ChatBotService();
            string message = string.Empty;
            
            Console.WriteLine("ChatBot started. Type 'close' to exit.");
            
            do
            {
                Console.Write("You: ");
                message = Console.ReadLine()!;
                
                if (string.IsNullOrWhiteSpace(message)) continue;
                if (message.Equals("close", StringComparison.OrdinalIgnoreCase)) break;
                
                try
                {
                    var request = new Request(message);
                    var response = chatService.SendRequest(request);

                    if (response.IsSuccess)
                    {
                        Console.WriteLine($"ChatBot: {response.Body}");
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
            } while (!message.Equals("close", StringComparison.OrdinalIgnoreCase));
            
            Console.WriteLine("ChatBot session ended.");
        }
    }
}
