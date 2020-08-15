using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TestConsole.Utilities;

namespace TestConsole.OutputFormatting.Internal
{
    /// <summary>
    /// Execute a <see cref="Report{T}"/>.
    /// </summary>
    internal static class ReportExecutor
    {
        /// <summary>
        /// Return the lines output by a report.
        /// </summary>
        /// <typeparam name="T">The type of the items that are input to the report.</typeparam>
        /// <param name="report">The report definition.</param>
        /// <param name="availableWidth">The available width for formatting.</param>
        /// <returns>A set of report lines.</returns>
        internal static IEnumerable<string> GetLines<T>(Report<T> report, int availableWidth)
        {
            int width;
            var indent = string.Empty;
            if (availableWidth > report.IndentSpaceCount)
                width = availableWidth - report.IndentSpaceCount;
            else
                width = availableWidth;
            var actualIndent = availableWidth - width;

            var formatMethod = MakeFormatMethodInfo(report);
            var parameters = new object[]
                             {
                                 report.Query,
                                 report.Columns,
                                 width,
                                 0, //rows to use for sizing
                                 report.Options,
                                 report.ColumnDivider,
                                 report.Children
                             };

            var tabular = MethodInvoker.Invoke(formatMethod, null, parameters) as IEnumerable<string>;
            if (actualIndent > 0)
                indent = new string(' ', actualIndent);
            foreach (var line in ProduceTitle(report, availableWidth).Concat(tabular))
            {
                if (actualIndent > 0)
                    foreach (var indented in ApplyIndent(indent, line))
                        yield return indented;
                else
                    yield return line;
            }
        }

        private static IEnumerable<string> ApplyIndent(string indent, string data)
        {
            using (var stream = new StringReader(data))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                    yield return indent + line + Environment.NewLine;
            }
        }

        private static IEnumerable<string> ProduceTitle<T>(Report<T> report, int availableWidth)
        {
            if (string.IsNullOrEmpty(report.TitleText))
                yield break;

            foreach (var line in Internal.TitleLinesGenerator.Generate(report.TitleText, availableWidth))
                yield return line;
        }

        private static MethodInfo MakeFormatMethodInfo<T>(Report<T> report)
        {
            var genericMethod = typeof(TabularReport)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == "Format"
                                     && m.GetParameters()[0].ParameterType.GetInterfaces()
                                         .Any(i => i == typeof(IEnumerable)));
            var formatMethod = genericMethod.MakeGenericMethod(report.RowType, typeof(T));
            return formatMethod;
        }
    }
}