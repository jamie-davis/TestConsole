using System;
using System.Linq;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.ReportDefinitions;

namespace TestConsoleLib.Snapshots
{
    /// <summary>
    /// This class coordinates the comparison of snapshots. It does not do the comparison itself, but it manages the process.
    /// </summary>
    internal static class SnapshotComparer
    {
        internal static void ReportDifferences(SnapshotCollection collection, Snapshot before, Snapshot after, Output output)
        {
            var differences = SnapshotDifferenceAnalyser.ExtractDifferences(collection, before, after);
            foreach (var tableDifference in differences.TableDifferences)
            {
                var report = tableDifference.RowDifferences.AsReport(rep => ReportDifferences(rep, tableDifference, output));
                output.FormatTable(report);
            }

        }

        private static void ReportDifferences(ReportParameters<RowDifference> rep, SnapshotTableDifferences tableDifferences, Output output)
        {
            var allCols = tableDifferences.RowDifferences
                .Where(r => r.Differences?.Differences != null )
                .SelectMany(r => r.Differences.Differences.Select(d => d.Name)).Distinct();
            var key = tableDifferences.RowDifferences.FirstOrDefault()?.Key;
            
            rep.Title(tableDifferences.TableDefinition.TableName);
            rep.RemoveBufferLimit();

            rep.AddColumn(rd => rd.DifferenceType.ToString(), cc => cc.Heading("Difference"));
            
            if (key != null)
            {
                var keyIndex = 0;
                foreach (var keyField in key.GetFieldNames())
                    rep.AddColumn(rd => rd.Key.AllKeys.Skip(keyIndex).First(), cc => cc.Heading(keyField));
            }

            foreach (var col in allCols)
            {
                rep.AddColumn(rep.Lambda(rd => DifferenceDisplay(rd, col)), cc => cc.Heading(col));
            }
        }

        private static string DifferenceDisplay(RowDifference rd, string col)
        {
            var before = rd.Differences?.Differences.FirstOrDefault(fd => fd.Name == col)?.Before;
            var after = rd.Differences?.Differences.FirstOrDefault(fd => fd.Name == col)?.After;
            if (before == null && after == null)
                return string.Empty;

            var beforeString = before?.ToString();
            var afterString = after?.ToString();
            if (before == null || after == null)
                return beforeString ?? afterString;
            return $"{beforeString} ==> {afterString}";
        }
    }
}