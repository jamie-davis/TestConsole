using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TestConsoleLib.Snapshots
{
    internal class SnapshotTableDifferences
    {
        public ReadOnlyCollection<RowDifference> RowDifferences { get; set; }
        public bool Matched { get; set; }
        public TableDefinition TableDefinition { get; }

        public SnapshotTableDifferences(IList<RowDifference> rowDiffs, TableDefinition tableDefinition)
        {
            TableDefinition = tableDefinition;
            RowDifferences = new ReadOnlyCollection<RowDifference>(rowDiffs);
            Matched = !RowDifferences.Any();
        }
    }
}