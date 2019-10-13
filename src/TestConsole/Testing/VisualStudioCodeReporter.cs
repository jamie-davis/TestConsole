using System.Runtime.InteropServices;

namespace TestConsoleLib.Testing
{
    [ApprovalsReporter("code")]
    public class VisualStudioCodeReporter : IApprovalsReporter
    {
        public string FileName => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "code.cmd" : "code";
        public string Arguments => "--diff $1 $2";
    }
}