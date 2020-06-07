using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TestConsoleLib.Snapshots
{
    internal class RowDataCompareResult
    {
        private readonly ReadOnlyCollection<FieldDifference> _differences;

        public RowDataCompareResult(bool result, IList<FieldDifference> differences, SnapshotRow before, SnapshotRow after)
        {
            Matched = result;
            _differences = differences != null ? new ReadOnlyCollection<FieldDifference>(differences) : null;
            Before = before;
            After = after;
        }

        public bool Matched { get; }
        public IEnumerable<FieldDifference> Differences => _differences;
        public SnapshotRow Before { get; }
        public SnapshotRow After { get; }
    }
}