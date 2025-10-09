using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPTLoggerMockService
{
    public class Response
    {
        public string ?Body { get; set; }
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
                    throw new ArgumentException("Request must not be empty");
                }
                _body = value;
            }
        }
        
        public Request(string body)
        {
            Body = body;
        }
    }
    public class ChatGPTService
    {
        public Response sendRequest(Request request)
        {
            // Simulate sending a request to the ChatGPT API
            return new Response { Body = $"Request sent with body: {request.Body}" };
        }
    }
}
