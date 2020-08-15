using System;
using System.Collections.Generic;
using System.Linq;

namespace TestConsole.OutputFormatting.Internal
{
    internal static class MinReportWidthCalculator
    {
        public static int Calculate<T>(IEnumerable<T> rep, int separatorLength, int tabLength = 4)
        {
            return PerformCalculation<T>(tabLength, separatorLength, sizers =>
            {
                foreach (var row in rep)
                {
                    foreach (var sizer in sizers)
                    {
                        var value = sizer.PropertyColumnFormat.Property.GetValue(row, null);
                        sizer.Sizer.ColumnValue(value);
                    }
                }

            });
        }

        public static int Calculate<T>(CachedRows<T> rep, int separatorLength, int tabLength = 4)
        {
            return PerformCalculation<T>(tabLength, separatorLength, sizers =>
            {
                foreach (var row in rep.GetRows())
                {
                    foreach (var sizer in sizers)
                    {
                        var value = row.Columns.FirstOrDefault(c => c.Property == sizer.PropertyColumnFormat.Property);
                        if (value != null && value.Value != null)
                            sizer.Sizer.ColumnValue(value.Value);
                    }
                }
            });
        }

        private static int PerformCalculation<T>(int tabLength, int separatorLength, Action<List<ColumnWidthNegotiator.ColumnSizerInfo>> applyRows)
        {
            var columns = FormatAnalyser.Analyse(typeof(T), null, true);
            var sizers = columns
                .Select(c => new ColumnWidthNegotiator.ColumnSizerInfo(c, tabLength))
                .ToList();

            applyRows(sizers);

            foreach (var columnSizerInfo in sizers)
            {
                columnSizerInfo.Sizer.ColumnValue(columnSizerInfo.PropertyColumnFormat.Format.Heading);
            }

            return sizers.Sum(s => s.Sizer.GetIdealMinimumWidth())
                   + ((sizers.Count - 1) * separatorLength);
        }
    }
}