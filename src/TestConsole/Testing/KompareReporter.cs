namespace TestConsoleLib.Testing
{
    [ApprovalsReporter("kompare")]
    public class KompareReporter : IApprovalsReporter
    {
        public string FileName => "kompare";
        public string Arguments => "$1 $2";
    }
}