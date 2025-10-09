using System;

namespace ChatGPTLoggerService
{
    /// <summary>
    /// Represents a request to the ChatBot service with validation and configuration options.
    /// </summary>
    public class Request
    {
        private string _body = string.Empty;

        /// <summary>
        /// Gets or sets the request body content. Cannot be null, empty, or whitespace.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the AI model to use for the request. Default is "gpt-oss:20b".
        /// </summary>
        public string Model { get; set; } = "gpt-oss:20b";

        /// <summary>
        /// Gets or sets whether to use streaming response. Default is false.
        /// </summary>
        public bool Stream { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the Request class with the specified body.
        /// </summary>
        /// <param name="body">The request body content</param>
        public Request(string body)
        {
            Body = body;
        }

        /// <summary>
        /// Initializes a new instance of the Request class with the specified body and model.
        /// </summary>
        /// <param name="body">The request body content</param>
        /// <param name="model">The AI model to use</param>
        public Request(string body, string model) : this(body)
        {
            Model = model;
        }   
    }
}