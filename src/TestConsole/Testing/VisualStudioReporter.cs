namespace TestConsoleLib.Testing
{
    [ApprovalsReporter("devenv")]
    public class VisualStudioReporter : IApprovalsReporter
    {
        public string FileName => "devenv.exe";
        public string Arguments => "/diff $1 $2";
    }
}