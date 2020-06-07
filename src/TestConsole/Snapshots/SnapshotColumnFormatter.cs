using System.Linq;
using TestConsole.OutputFormatting.ReportDefinitions;
using TestConsoleLib.Utilities;

namespace TestConsoleLib.Snapshots
{
    internal static class SnapshotColumnFormatter
    {
        public static void Format(ColumnConfig cc, TableDefinition table, SnapshotColumnInfo column)
        {
            cc.Heading(column.Name);
            if (column.ObservedTypes.All(NumericTypeHelper.IsNumeric))
                cc.RightAlign();
        }
    }
}