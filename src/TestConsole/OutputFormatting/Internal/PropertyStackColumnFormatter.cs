using System.Collections.Generic;
using System.Linq;

namespace TestConsole.OutputFormatting.Internal
{
    internal static class PropertyStackColumnFormatter
    {
        public static IEnumerable<string> Format(IEnumerable<PropertyColumnFormat> stackedColumns, object item, 
            int width, SplitCache cache, int tabLength = 4, int firstLineHangingIndent = 0)
        {
            return stackedColumns.SelectMany((c, i) => FormatCol(c, item, width, tabLength, i == 0 ? firstLineHangingIndent : 0, cache));
        }

        public static IEnumerable<string> Format(IEnumerable<PropertyColumnFormat> stackedColumns, IEnumerable<string> preFormattedValues, 
            int width, SplitCache cache, int tabLength = 4, int firstLineHangingIndent = 0)
        {
            var values = preFormattedValues.ToList();
            return stackedColumns.SelectMany((c, i) => FormatColWithPreFormattedValue(c, values[i], width, tabLength, i == 0 ? firstLineHangingIndent : 0, cache));
        }

        public static IEnumerable<string> Format(IEnumerable<PropertyColumnFormat> stackedColumns, 
            IEnumerable<FormattingIntermediate> preFormattedValues, int width, SplitCache cache, int tabLength = 4, int firstLineHangingIndent = 0)
        {
            var values = preFormattedValues.ToList();
            return stackedColumns.SelectMany((c, i) => FormatColWithIntermediate(c, values[i], width, tabLength, i == 0 ? firstLineHangingIndent : 0, cache));
        }

        private static IEnumerable<string> FormatCol(PropertyColumnFormat pcf, object item, 
            int width, int tabLength, int firstLineHangingIndent, SplitCache cache)
        {
            var rawValue = pcf.Property.GetValue(item, null);
            var value = ValueFormatter.Format(pcf.Format, rawValue);
            return FormatColWithPreFormattedValue(pcf, value, width, tabLength, firstLineHangingIndent, cache);
        }

        private static IEnumerable<string> FormatColWithPreFormattedValue(PropertyColumnFormat pcf, string preFormattedValue, 
            int width, int tabLength, int firstLineHangingIndent, SplitCache cache)
        {
            return PropertyStackFormatter.Format(pcf.Format, preFormattedValue, width, cache, tabLength, firstLineHangingIndent);
        }

        private static IEnumerable<string> FormatColWithIntermediate(PropertyColumnFormat pcf, FormattingIntermediate preFormattedValue, 
            int width, int tabLength, int firstLineHangingIndent, SplitCache cache)
        {
            return PropertyStackFormatter.Format(pcf.Format, FormatIntermediateValue(preFormattedValue), width, cache, tabLength, firstLineHangingIndent);
        }

        private static object FormatIntermediateValue(FormattingIntermediate preFormattedValue)
        {
            if (preFormattedValue.TextValue != null)
                return preFormattedValue.TextValue;

            return preFormattedValue.RenderableValue;
        }
    }
}