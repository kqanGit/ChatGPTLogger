# ChatGPTLogger

A modular .NET 9 application for logging ChatGPT conversations with support for multiple output formats through a plugin-based architecture.

## Overview

ChatGPTLogger provides a console interface for interacting with ChatGPT while automatically logging conversations in various formats. The application uses a plugin-based architecture that allows for easy extensibility and supports multiple logging formats including text, XML, and JSON.

## Features

- **Interactive Console Interface**: Chat with ChatGPT directly from the command line
- **Plugin-Based Architecture**: Modular design with pluggable logger implementations
- **Multiple Logging Formats**: Built-in support for text, XML, and JSON logging
- **Daily Log Files**: Automatically creates separate log files for each day
- **Error Handling**: Robust error handling with informative error messages
- **Flexible API Integration**: Configurable API endpoint for different ChatGPT services

## Project Structure

```
ChatGPTLogger/
??? ILogging/                    # Core logging interface
?   ??? ILogger.cs              # Interface definition for all loggers
??? TxtLogger/                   # Text file logger implementation
?   ??? TxtLogger.cs            # Logs conversations to daily .txt files
??? XmlLogger/                   # XML file logger implementation
?   ??? XmlLogger.cs            # Logs conversations to daily .xml files
??? JsonLogger/                  # JSON file logger implementation
?   ??? JsonLogger.cs           # Logs conversations to daily .json files
??? ChatGPTLoggerService/        # Core chat service and models
?   ??? ChatBot.cs              # Main ChatBot service for API communication
?   ??? Request.cs              # Request model
?   ??? Response.cs             # Response model
?   ??? PSHandler.cs            # PowerShell command handler
??? ChatGPTLoggerConsole/        # Console application
    ??? App.cs                  # Main application entry point
```

## Requirements

- .NET 9.0 or later
- PowerShell (for API communication)
- Internet connection (for ChatGPT API access)

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/kqanGit/ChatGPTLogger.git
   cd ChatGPTLogger
   ```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Create the LoggerPlugins directory in the console application's output folder:
   ```bash
   mkdir ChatGPTLoggerConsole/bin/Debug/net9.0/LoggerPlugins
   ```

4. Copy the logger DLL files to the plugins directory:
   ```bash
   copy TxtLogger/bin/Debug/net9.0/TxtLogger.dll ChatGPTLoggerConsole/bin/Debug/net9.0/LoggerPlugins/
   copy XmlLogger/bin/Debug/net9.0/XmlLogger.dll ChatGPTLoggerConsole/bin/Debug/net9.0/LoggerPlugins/
   copy JsonLogger/bin/Debug/net9.0/JsonLogger.dll ChatGPTLoggerConsole/bin/Debug/net9.0/LoggerPlugins/
   ```

## Usage

1. Run the console application:
   ```bash
   cd ChatGPTLoggerConsole
   dotnet run
   ```

2. Select your preferred logging format from the available options:
   ```
   Select log type to save:
   0 - Txt
   1 - Xml
   2 - Json
   
   Select type (0 - 2):
   ```

3. Start chatting with ChatGPT:
   ```
   You: Hello, how are you?
   ChatBot: Hello! I'm doing well, thank you for asking. How can I help you today?
   ```

4. Type `exit` to end the session.

## Log File Formats

### Text Logger (`TxtLogger`)
Creates daily log files in format: `log_YYYYMMDD.txt`
```
2024-01-15 14:30:25 | Request: Hello, how are you? | Response: Hello! I'm doing well, thank you for asking. How can I help you today?
```

### XML Logger (`XmlLogger`)
Creates daily log files in format: `log_YYYYMMDD.xml`
```xml
<?xml version="1.0" encoding="utf-8"?>
<Logs>
  <Entry>
    <Time>2024-01-15 14:30:25</Time>
    <Request>Hello, how are you?</Request>
    <Response>Hello! I'm doing well, thank you for asking. How can I help you today?</Response>
  </Entry>
</Logs>
```

### JSON Logger (`JsonLogger`)
Creates daily log files in format: `log_YYYYMMDD.json`
```json
[
  {
    "Time": "2024-01-15 14:30:25",
    "Request": "Hello, how are you?",
    "Response": "Hello! I'm doing well, thank you for asking. How can I help you today?"
  }
]
```

## Configuration

### API Endpoint
The default API endpoint is configured in `ChatBotService`. To use a different endpoint, modify the constructor parameter:
```csharp
public ChatBotService(string apiAddress = "your-api-endpoint-here")
```

### Model Configuration
The ChatGPT model and streaming settings can be configured in the `Request` class.

## Creating Custom Loggers

To create a custom logger:

1. Create a new class library project
2. Reference the `ILogging` project
3. Implement the `ILogger` interface:
   ```csharp
   using ILogging;
   
   public class CustomLogger : ILogger
   {
       public void Log(string request, string response)
       {
           // Your custom logging implementation
       }
   }
   ```
4. Build the project and copy the DLL to the `LoggerPlugins` folder

## Dependencies

- **System.Text.Json**: For JSON serialization
- **System.Xml**: For XML processing
- **PowerShell**: For API communication

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/new-feature`)
3. Commit your changes (`git commit -am 'Add new feature'`)
4. Push to the branch (`git push origin feature/new-feature`)
5. Create a Pull Request

## License

This project is open source. Please refer to the license file for more information.

## Support

If you encounter any issues or have questions, please open an issue on the GitHub repository.

## Changelog

### Version 1.0.0
- Initial release
- Support for Text, XML, and JSON logging
- Plugin-based architecture
- Console interface for ChatGPT interaction
- Daily log file generation