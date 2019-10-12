namespace TestConsoleLib.Testing
{
    [ApprovalsReporter("code")]
    public class VisualStudioCodeReporter : IApprovalsReporter
    {
        public string FileName => "code.cmd";
        public string Arguments => "--diff $1 $2";
    }
}