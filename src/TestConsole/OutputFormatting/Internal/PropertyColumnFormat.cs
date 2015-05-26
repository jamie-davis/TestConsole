using System.Reflection;

namespace TestConsole.OutputFormatting.Internal
{
    internal class PropertyColumnFormat
    {
        public PropertyInfo Property { get; set; }
        public ColumnFormat Format { get; set; }

        internal PropertyColumnFormat(PropertyInfo property, ColumnFormat format)
        {
            Property = property;
            Format = format;
        }
    }
}