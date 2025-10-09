using System.Management.Automation;
using System.Text;

namespace PowerShellHandler
{
    /// <summary>
    /// PowerShell command execution handler for running PowerShell scripts and commands.
    /// </summary>
    public static class PowerShellHandler
    {
        // Static PowerShell instance for command execution
        private static PowerShell ps = PowerShell.Create();

        /// <summary>
        /// Executes a PowerShell command and returns the result as a string.
        /// </summary>
        /// <param name="command">The PowerShell command to execute</param>
        /// <returns>The command execution result as a string, or error message if execution fails</returns>
        public static string ExecCommand(string command)
        {
            string errorMsg = string.Empty;

            // Add the command script and output formatting
            ps.AddScript(command);
            ps.AddCommand("Out-String");

            // Create output collection for results
            PSDataCollection<PSObject> outputCollection = new();
            
            // Set up error handling
            ps.Streams.Error.DataAdded += (object sender, DataAddedEventArgs e) =>
            {
                errorMsg = ((PSDataCollection<ErrorRecord>)sender)[e.Index].ToString();
            };

            // Execute the command asynchronously
            IAsyncResult result = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);
            ps.EndInvoke(result);

            // Build result string from output collection
            StringBuilder sb = new();
            foreach (var outputItem in outputCollection)
            {
                sb.AppendLine(outputItem.BaseObject.ToString());
            }

            // Clear commands for next execution
            ps.Commands.Clear();

            // Return error message if any errors occurred, otherwise return the result
            if (!string.IsNullOrEmpty(errorMsg))
                return errorMsg;

            return sb.ToString().Trim();
        }
    }
}
