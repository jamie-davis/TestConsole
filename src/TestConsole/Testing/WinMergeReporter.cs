namespace TestConsoleLib.Testing
{
    [ApprovalsReporter("winmerge")]
    public class WinMergeReporter : IApprovalsReporter
    {
        public string FileName => "WinMergeU";
        public string Arguments => "$1 $2";
    }
}