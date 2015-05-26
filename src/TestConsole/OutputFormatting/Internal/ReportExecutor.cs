using System.Collections;
using System.Collections.Generic;
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
        /// <param name="width">The available width for formatting.</param>
        /// <returns>A set of report lines.</returns>
        internal static IEnumerable<string> GetLines<T>(Report<T> report, int width)
        {
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
            return tabular;
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