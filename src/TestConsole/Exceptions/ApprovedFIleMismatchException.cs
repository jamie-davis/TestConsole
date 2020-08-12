using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace TestConsoleLib.Exceptions
{
    public class ApprovedFileMismatchException : Exception
    {
        private static string[] _emptyLinesArray;

        public ApprovedFileMismatchException(string receivedFile, string approvedFile) : base(AnalyseFiles(receivedFile, approvedFile))
        {
        }

        private static string AnalyseFiles(string receivedFile, string approvedFile)
        {
            var receivedLines = ReadFile(receivedFile);
            var approvedLines = ReadFile(approvedFile);

            var common = Math.Min(receivedLines.Length, approvedLines.Length);

            var differences = receivedLines.Take(common).Zip(approvedLines.Take(common), (r, a) => new { Received = r, Approved = a})
                .Select((l, ix) => new { Index = ix, l.Received, l.Approved })
                .Where(l => l.Received != l.Approved)
                .ToList();

            if (differences.Count == 0)
                return "Files did not match, but no text differences detected.";

            try
            {
                var output = new Output();

                output.WrapLine("Differences found between approved and received:");
                output.WriteLine();

                output.FormatTable(new[]
                {
                    new {File = "Approved", Length = new FileInfo(approvedFile).Length, Lines = approvedLines.Length},
                    new {File = "Received", Length = new FileInfo(receivedFile).Length, Lines = receivedLines.Length},
                });

                output.WriteLine();
                output.FormatTable(differences.SelectMany(d => new []
                {
                    new { File = "Approved", Line = d.Index + 1, Text = d.Approved },
                    new { File = "Received", Line = d.Index + 1, Text = d.Received },
                }));

                return output.Report;
            }
            catch
            {
                return "Files did not match but no information could be extracted.";
            }
        }

        private static string[] ReadFile(string path)
        {
            try
            {
                _emptyLinesArray = new string[] { };
                return File.Exists(path) ? File.ReadAllLines(path) : _emptyLinesArray;
            }
            catch
            {
                return _emptyLinesArray;
            }
        }
    }
}