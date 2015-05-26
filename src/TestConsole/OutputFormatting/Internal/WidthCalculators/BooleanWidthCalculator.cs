using System;

namespace TestConsole.OutputFormatting.Internal.WidthCalculators
{
    /// <summary>
    /// Calculate the defaults for Boolean column widths.
    /// </summary>
    internal static class BooleanWidthCalculator
    {
        public static int Max(ColumnFormat format)
        {
            return Math.Max(ValueFormatter.Format(format, true).Length,
                ValueFormatter.Format(format, false).Length);
        }

        public static int Min(ColumnFormat format)
        {
            return Math.Min(ValueFormatter.Format(format, true).Length,
                ValueFormatter.Format(format, false).Length);
        }
    }
}