namespace ChatGPTLoggerService
{
    /// <summary>
    /// Represents a response from the ChatBot service including success status and error handling.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Gets or sets the response body content from the ChatBot service.
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request was processed successfully.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the request failed. Null if successful.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}