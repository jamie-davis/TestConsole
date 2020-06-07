using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TestConsoleLib.Snapshots
{
    internal class SnapshotDifferences
    {
        public ReadOnlyCollection<SnapshotTableDifferences> TableDifferences { get; }
        public bool Matched { get; }

        public SnapshotDifferences(IList<SnapshotTableDifferences> tableDiffs)
        {
            TableDifferences = new ReadOnlyCollection<SnapshotTableDifferences>(tableDiffs);
            Matched = !TableDifferences.Any();
        }
    }
}