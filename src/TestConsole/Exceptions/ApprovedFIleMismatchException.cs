using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TestConsole.OutputFormatting;

namespace TestConsoleLib.Exceptions
{
    public class ApprovedFileMismatchException : Exception
    {
        private const int MaxDifferences = 25;

        private static string[] _emptyLinesArray;

        public ApprovedFileMismatchException(string receivedFile, string approvedFile) : base(AnalyseFiles(receivedFile, approvedFile))
        {
        }

        private static string AnalyseFiles(string receivedFile, string approvedFile)
        {
            var receivedLines = ReadFile(receivedFile);
            var approvedLines = ReadFile(approvedFile);

            var common = Math.Min(receivedLines.Length, approvedLines.Length);
            var longest = Math.Max(receivedLines.Length, approvedLines.Length);

            var differenceLines = receivedLines.Take(common).Zip(approvedLines.Take(common), (r, a) => new { Received = r, Approved = a})
                .Select((l, ix) => new { Index = ix, l.Received, l.Approved });

            if (receivedLines.Length > common)
            {
                differenceLines = differenceLines.Concat(receivedLines.Skip(common).Select((l, ix) => new {Index = ix + common, Received = l, Approved = string.Empty}));
            }

            if (approvedLines.Length > common)
            {
                differenceLines = differenceLines.Concat(approvedLines.Skip(common).Select((l, ix) => new {Index = ix + common, Received = string.Empty, Approved = l}));
            }

            var differences = differenceLines
                .Where(l => l.Received != l.Approved)
                .ToList();

            if (differences.Count == 0 && longest == common)
                return "Files did not match, but no text differences detected.";

            try
            {
                var output = new Output();

                output.WrapLine($"{differences.Count} Differences found between approved and received:");
                output.WriteLine();

                output.FormatTable(new[]
                {
                    new {File = "Approved", Length = new FileInfo(approvedFile).Length, Lines = approvedLines.Length},
                    new {File = "Received", Length = new FileInfo(receivedFile).Length, Lines = receivedLines.Length},
                });

                output.WriteLine();
                var diffRowLimit = MaxDifferences*2;
                var diffLines = differences.SelectMany(d => new []
                {
                    new { File = "Approved", Line = (int?)d.Index + 1, Text = d.Approved },
                    new { File = "Received", Line = (int?)d.Index + 1, Text = d.Received },
                }).Take(diffRowLimit);

                if (MaxDifferences < differences.Count)
                {
                    diffLines = diffLines.Concat(new [] { new { File = "...", Line = (int?)null, Text = string.Empty } });
                }

                var report = diffLines.AsReport(rep => rep.RemoveBufferLimit()
                    .AddColumn(d => d.File)
                    .AddColumn(d => d.Line, cc => cc.RightAlign())
                    .AddColumn(d => d.Text)
                );

                output.FormatTable(report);

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