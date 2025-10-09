using System.Text.RegularExpressions;
using PowerShellHandler;

namespace ChatGPTLoggerService
{
    /// <summary>
    /// Service class for handling ChatBot requests and responses through Ollama API.
    /// </summary>
    public class ChatBotService
    {
        private readonly string _apiAddress;

        /// <summary>
        /// Initializes a new instance of the ChatBotService class with the specified API address.
        /// </summary>
        /// <param name="apiAddress">The Ollama API endpoint URL</param>
        public ChatBotService(string apiAddress = "https://unglaring-unsavoured-elene.ngrok-free.dev")
        {
            _apiAddress = apiAddress;
        }

        /// <summary>
        /// Sends a request to the Ollama API and returns the processed response.
        /// </summary>
        /// <param name="request">The ChatBot request object containing the prompt and configuration</param>
        /// <returns>A Response object containing the API result or error information</returns>
        public Response SendRequest(Request request)
        {
            try
            {
                // Create JSON payload with proper escaping for special characters
                var jsonPayload = $"{{\"model\":\"{request.Model}\", \"prompt\":\"{request.Body.Replace("\"", "\\\"").Replace("\\", "\\\\")}\", \"stream\": {request.Stream.ToString().ToLower()}}}";
                
                // Encode payload to Base64 to avoid PowerShell escaping issues
                var bytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
                var base64Payload = Convert.ToBase64String(bytes);

                // Build PowerShell command using Base64 encoding for safe transmission
                string cmd = $@"
                    $payload = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String('{base64Payload}'))
                    (Invoke-WebRequest -Method POST -Body $payload -Uri {_apiAddress}/api/generate -ContentType 'application/json').Content | ConvertFrom-Json
                ";

                // Execute PowerShell command to send request to API
                string chatResult = PowerShellHandler.PowerShellHandler.ExecCommand(cmd);

                // Process the raw API response to extract the chat response
                string processedResponse = ProcessResponse(chatResult);

                // Return successful response
                return new Response
                {
                    Body = processedResponse,
                    IsSuccess = true,
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                // Return error response if any exception occurs
                return new Response
                {
                    Body = null,
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Processes the raw API response to extract and clean the chat response text.
        /// </summary>
        /// <param name="rawResponse">The raw response string from the Ollama API</param>
        /// <returns>The processed and cleaned response text</returns>
        private static string ProcessResponse(string rawResponse)
        {
            // Find the response content between specific markers
            int responseStart = rawResponse.IndexOf("response             :") + ("response             :".Length);
            int responseEnd = rawResponse.IndexOf("thinking             :");
            
            // Return raw response if expected markers are not found
            if (responseStart < 0 || responseEnd < 0)
            {
                return rawResponse;
            }

            // Extract the response content
            string chatResponse = rawResponse.Substring(responseStart, (responseEnd - responseStart));

            // Clean up the response text by removing carriage returns and extra spaces
            chatResponse = chatResponse.Replace("\r", string.Empty);
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            chatResponse = regex.Replace(chatResponse, " ");

            return chatResponse.Trim();
        }
    }

    /// <summary>
    /// Static class providing console-based ChatBot interaction functionality.
    /// </summary>
    public static class ChatBot
    {
        /// <summary>
        /// Starts an interactive ChatBot session in the console. Type 'close' to exit.
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
                
                // Skip empty messages
                if (string.IsNullOrWhiteSpace(message)) continue;
                
                // Exit if user types 'close'
                if (message.Equals("close", StringComparison.OrdinalIgnoreCase)) break;
                
                try
                {
                    // Create request and send to ChatBot service
                    var request = new Request(message);
                    var response = chatService.SendRequest(request);

                    // Display response or error message
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
