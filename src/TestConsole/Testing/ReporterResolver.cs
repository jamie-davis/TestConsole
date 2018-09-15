using System;
using System.IO;

namespace TestConsoleLib.Testing
{
    public static class ReporterResolver
    {
        public static string Resolve(string receivedFile)
        {
            var path = Path.GetDirectoryName(receivedFile);
            while (Directory.Exists(path))
            {
                var settingsFile = Path.Combine(path, ".testconsole");
                if (File.Exists(settingsFile))
                {
                    if (ReporterExtractor.Extract(File.ReadAllLines(settingsFile), out var reporter))
                        return reporter;
                }

                path = Path.GetDirectoryName(path);
            }

            var reporterName = Environment.GetEnvironmentVariable("TESTREPORTER") ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(reporterName))
                return reporterName;

            return null;
        }
    }
}