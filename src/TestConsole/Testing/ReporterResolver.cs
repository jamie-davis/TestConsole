using System;
using System.Collections.Generic;
using System.IO;

namespace TestConsoleLib.Testing
{
    public static class ReporterResolver
    {
        public static IApprovalsReporter Resolve(string receivedFile, Dictionary<string, IApprovalsReporter> reporters)
        {
            var path = Path.GetDirectoryName(receivedFile);
            while (Directory.Exists(path))
            {
                var settingsFile = Path.Combine(path, ".testconsole");
                if (File.Exists(settingsFile))
                {
                    var extracted = ReporterExtractor.Extract(File.ReadAllLines(settingsFile));
                    if (extracted.Result == ExtractedReporterResult.Selected)
                    {
                        if (extracted.ReporterName == null || !reporters.TryGetValue(extracted.ReporterName, out var reporter))
                            return null;
                        return reporter;
                    }

                    if (extracted.Result == ExtractedReporterResult.Custom)
                    {
                        return new CustomReporter(extracted);
                    }

                    return null;
                }

                path = Path.GetDirectoryName(path);
            }

            var reporterName = Environment.GetEnvironmentVariable("TESTREPORTER") ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(reporterName))
            {
                if (reporters.TryGetValue(reporterName, out var reporter))
                    return reporter;
            }

            return null;
        }
    }
}