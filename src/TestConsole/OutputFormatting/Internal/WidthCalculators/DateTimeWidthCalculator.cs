using System;

namespace TestConsole.OutputFormatting.Internal.WidthCalculators
{
    /// <summary>
    /// Calculate the defaults for DateTime column widths.
    /// </summary>
    internal static class DateTimeWidthCalculator
    {
        public static int Calculate(ColumnFormat format)
        {
            return ValueFormatter.Format(format, DateTime.MinValue).Length;
        }
    }
}