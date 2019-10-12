namespace TestConsoleLib.Testing
{
    internal class CustomReporter : IApprovalsReporter
    {
        public CustomReporter(ExtractedReporterInfo extracted)
        {
            FileName = extracted.CustomFileName;
            Arguments = extracted.CustomArgTemplate;
        }

        #region Implementation of IApprovalsReporter

        public string FileName { get; }
        public string Arguments { get; }

        #endregion
    }
}