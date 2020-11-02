using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestConsole.OutputFormatting.Internal;

namespace TestConsole.Tests.OutputFormatting.UnitTestutilities
{
    internal static class TabularReportRenderTool
    {
        public static string Report<T>(ColumnWidthNegotiator cwn, IEnumerable<T> items)
        {
            return Report(cwn.Columns, cwn.StackedColumns, cwn.StackedColumnWidth, cwn.TabLength, items);
        }

        public static string Report<T>(ColumnSizingParameters csp, IEnumerable<T> items)
        {
            var stackedColumns = csp.StackSizer == null ? new PropertyColumnFormat[] {} : csp.StackSizer.Columns;
            return Report(csp.Columns, stackedColumns, csp.StackedColumnWidth, csp.TabLength, items);
        }

        private static string Report<T>(List<PropertyColumnFormat> columns, IEnumerable<PropertyColumnFormat> stackedColumns, int stackedColumnWidth, int tabLength, IEnumerable<T> items)
        {
            var cache = new SplitCache();

            var sb = new StringBuilder();

            var widths = columns.Select(c => c.Format.ActualWidth)
                .Concat(stackedColumns.Any() ? new[] { stackedColumnWidth } : new int[] { })
                .ToArray();
            var headings = columns
                .Select(c => ColumnWrapper.WrapValue(c.Format.Heading, c.Format, c.Format.ActualWidth, cache))
                .ToArray();
            if (headings.Any())
            {
                sb.Append(ReportColumnAligner.AlignColumns(widths, headings, ColVerticalAligment.Bottom));

                var unders = columns
                    .Select(
                        c =>
                            ColumnWrapper.WrapValue(new string('-', c.Format.ActualWidth), c.Format, c.Format.ActualWidth, cache))
                    .ToArray();
                sb.Append(ReportColumnAligner.AlignColumns(widths, unders, ColVerticalAligment.Bottom));
            }

            foreach (var item in items)
            {
                var rowValues = columns
                    .Select((c, i) => ColumnWrapper.WrapValue(ValueFormatter.Format(c.Format, item.GetType().GetProperties()[i].GetValue(item, null)), c.Format, c.Format.ActualWidth, cache))
                    .Concat(FormatStackedColumn(stackedColumns, stackedColumnWidth, tabLength, item, cache))
                    .ToArray();
                sb.Append(ReportColumnAligner.AlignColumns(widths, rowValues));
            }
            return sb.ToString();
        }

        private static IEnumerable<string[]> FormatStackedColumn(IEnumerable<PropertyColumnFormat> stackedColumns, int stackedColumnWidth, int tabLength, object item, SplitCache cache)
        {
            if (stackedColumns.Any())
                return new[] { PropertyStackColumnFormatter.Format(stackedColumns, item, stackedColumnWidth, cache, tabLength).ToArray() };

            return new string[][] { };
        }

        public static string ReportSizingData(ColumnWidthNegotiator cwn)
        {
            var rows = cwn.GetSizingValues().Select(v => String.Join(",", v.GetValues().Select(i => i.ToString()))).ToList();
            var output = String.Join(Environment.NewLine, rows);
            return output;
        }
    }
}