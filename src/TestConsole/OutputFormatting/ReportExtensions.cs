using System;
using System.Collections.Generic;
using TestConsole.OutputFormatting.ReportDefinitions;

namespace TestConsole.OutputFormatting
{
    public static class ReportExtensions
    {
        public static Report<T> AsReport<T>(this IEnumerable<T> items, Action<ReportParameters<T>> defineReport)
        {
            var reportParameters = new ReportParameters<T>();
            defineReport(reportParameters);
            return new Report<T>(items, reportParameters);
        }

    }
}