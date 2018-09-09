namespace TestConsoleLib.Testing
{
    [ApprovalsReporter("meld")]
    public class MeldReporter : IApprovalsReporter
    {
        public string FileName => "meld";
        public string Arguments => "$1 $2";
    }
}