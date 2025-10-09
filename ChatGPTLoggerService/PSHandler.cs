using System.Management.Automation;
using System.Text;

namespace PowerShellHandler;

/// <summary>
/// Xử lý powershell
/// </summary>
public static class PowerShellHandler
{
    private static PowerShell ps = PowerShell.Create();

    /// <summary>
    /// Chạy command trong powershell
    /// </summary>
    /// <param name="command">Lệnh được gọi</param>
    /// <returns>Kết quả thực thi lệnh ở dạng chuỗi</returns>
    public static string ExecCommand(string command)
    {
        string errorMsg = string.Empty;

        ps.AddScript(command);
        ps.AddCommand("Out-string");

        PSDataCollection<PSObject> outputCollection = new();
        ps.Streams.Error.DataAdded += (object sender, DataAddedEventArgs e) =>
        {
            errorMsg = ((PSDataCollection<ErrorRecord>)sender)[e.Index].ToString();
        };

        IAsyncResult result = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);

        ps.EndInvoke(result);

        StringBuilder sb = new();

        foreach (var outputItem in outputCollection)
        {
            sb.AppendLine(outputItem.BaseObject.ToString());
        }

        ps.Commands.Clear();

        if (!string.IsNullOrEmpty(errorMsg))
            return errorMsg;

        return sb.ToString().Trim();
    }
}
