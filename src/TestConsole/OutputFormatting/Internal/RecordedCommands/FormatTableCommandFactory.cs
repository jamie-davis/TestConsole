using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestConsole.OutputFormatting.ReportDefinitions;
using TestConsole.Utilities;

namespace TestConsole.OutputFormatting.Internal.RecordedCommands
{
    internal static class FormatTableCommandFactory
    {
        public static FormatTableCommand<T, T> Make<T>(IEnumerable<T> data, SplitCache cache, string columnSeparator = null, ReportFormattingOptions options = ReportFormattingOptions.Default, IEnumerable<BaseChildItem<T>> childReports = null, IEnumerable<ColumnFormat> columns = null)
        {
            return new FormatTableCommand<T, T>(data, options, columnSeparator ?? " ", columns: columns, childReports: childReports, cache: cache);
        }

        internal static IRecordedCommand Make<T>(Report<T> report, SplitCache cache)
        {
            var itemType = report.RowType;
            var parameters = new Object[]
                             {
                                report, cache
                             };
            var genericMethod = typeof(FormatTableCommandFactory).GetMethod("CallMake", BindingFlags.NonPublic | BindingFlags.Static);

            var method = genericMethod.MakeGenericMethod(typeof(T), itemType);

            return MethodInvoker.Invoke(method, null, parameters) as IRecordedCommand;
        }

        // ReSharper disable once UnusedMember.Global
        internal static FormatTableCommand<TReportItem, TOriginal> CallMake<TOriginal, TReportItem>(Report<TOriginal> report, SplitCache cache)
        {
            return new FormatTableCommand<TReportItem, TOriginal>(report.Query.Cast<TReportItem>(), report.Options, report.ColumnDivider, cache, report.Children, report.Columns);
        }
    }
}