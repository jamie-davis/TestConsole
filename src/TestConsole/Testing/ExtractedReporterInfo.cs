namespace TestConsoleLib.Testing
{
    internal class ExtractedReporterInfo
    {
        public ExtractedReporterResult Result { get; }
        public string ReporterName { get; }
        public string CustomFileName { get; }
        public string CustomArgTemplate { get; }

        public ExtractedReporterInfo(string reporter)
        {
            ReporterName = reporter;
            Result = ExtractedReporterResult.Selected;
        }

        public ExtractedReporterInfo(string customFileName, string customArgTemplate)
        {
            CustomFileName = customFileName;
            CustomArgTemplate = customArgTemplate;
            Result = ExtractedReporterResult.Custom;
        }

        public ExtractedReporterInfo()
        {
            Result = ExtractedReporterResult.NotSpecified;
        }
    }
}