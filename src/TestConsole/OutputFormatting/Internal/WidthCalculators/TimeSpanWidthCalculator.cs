using System;

namespace TestConsole.OutputFormatting.Internal.WidthCalculators
{
    /// <summary>
    /// Calculate the defaults for TimeSpan column widths.
    /// </summary>
    internal static class TimeSpanWidthCalculator
    {
        public static int Calculate(ColumnFormat format)
        {
            return ValueFormatter.Format(format, TimeSpan.MinValue).Length;
        }
    }
}