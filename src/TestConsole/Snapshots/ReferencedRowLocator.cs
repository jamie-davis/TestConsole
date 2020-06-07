using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TestConsoleLib.Snapshots
{
    /// <summary>
    /// This class locates all rows in a set of snapshot differences where a difference references rows from another table which are not present in
    /// the difference set. The purpose of this is to enable those referenced rows to be added to the set for clarity. This should allow the test result
    /// to prove whether or not the reference was set to a valid row.
    /// </summary>
    internal static class ReferencedRowLocator
    {
        public class RequiredTableRows
        {
            internal RequiredTableRows(TableDefinition tableDefinition, List<RowRequest> keys)
            {
                TableDefinition = tableDefinition;
                Keys = new ReadOnlyCollection<RowRequest>(keys);
            }

            public TableDefinition TableDefinition { get; }
            public ReadOnlyCollection<RowRequest> Keys { get; }
        }

        public class RowRequest
        {
            public RowRequest(string columnName, object requestedValue)
            {
                ColumnName = columnName;
                RequestedValue = requestedValue;
            }

            public string ColumnName { get; }
            public object RequestedValue { get; }
        }

        public class AdditionalReferencedRows
        {
            public AdditionalReferencedRows(List<RequiredTableRows> tables)
            {
                Tables = new ReadOnlyCollection<RequiredTableRows>(tables);
            }

            public ReadOnlyCollection<RequiredTableRows> Tables { get; }
        }

        /// <summary>
        /// Scan the differences in a set of table <see cref="SnapshotTableDifferences"/> for columns flagged as referencing rows in the snapshot. If the
        /// referenced row is present, but does not contain any differences, it will be returned as an additional reference. This method reads the differences,
        /// but will not make any changes.
        /// </summary>
        /// <param name="collection">The snapshot collection. Needed for table definitions.</param>
        /// <param name="tableDiffs">The difference set that will be analysed</param>
        /// <returns>A <see cref="AdditionalReferencedRows"/> instance containing the referenced row details. Only rows not currently in the difference set
        /// will be included.</returns>
        public static AdditionalReferencedRows GetMissingRows(SnapshotCollection collection, IReadOnlyCollection<SnapshotTableDifferences> tableDiffs)
        {
            var allReferencedKeysByTable = GetReferencedKeys(collection, tableDiffs)
                .Where(rk => rk.KeyValue != null)
                .GroupBy(rk => new {rk.TableDefinition.TableName, rk.ColumnName})
                .ToList();

            var tableNotPresent =
                allReferencedKeysByTable.Where(g => !tableDiffs.Any(td => ReferenceEquals(td.TableDefinition, g.Key)))
                    .ToList();

            var missingKeys = allReferencedKeysByTable.Join(tableDiffs, kg => kg.Key.TableName, td => td.TableDefinition.TableName,
                (kg, td) => new
                {
                    TableDefinition = kg.Key.TableName, 
                    KeyField = kg.Key.ColumnName,
                    MissingRows = kg.Where(k => !td.RowDifferences.Any(row => ValueComparer.Compare(row.After.GetField(k.ColumnName), k.KeyValue) == 0))
                        .Select(k => k.KeyValue).ToList()
                })
                .GroupBy(req => req.TableDefinition)
                .Select(g => new RequiredTableRows(collection.GetTableDefinition(g.Key), g.SelectMany(r=> r.MissingRows.Select(mr => new RowRequest(r.KeyField, mr))).ToList()))
                .ToList();

            return new AdditionalReferencedRows(missingKeys);
        }

        private static IEnumerable<(TableDefinition TableDefinition, string ColumnName, object KeyValue)> GetReferencedKeys(SnapshotCollection collection, IReadOnlyCollection<SnapshotTableDifferences> tableDiffs)
        {
            foreach (var tableDiff in tableDiffs)
            {
                var referenceColumns = tableDiff.TableDefinition
                    .Columns
                    .Where(n => n.ReferencedTableName != null && n.ReferencedPropertyName != null)
                    .ToList();
                foreach (var referenceColumn in referenceColumns)
                {
                    var diffs = tableDiff
                        .RowDifferences
                        .SelectMany(d => d.Differences.Differences)
                        .Where(d => d.Name == referenceColumn.Name);
                    foreach (var fieldDifference in diffs)
                    {
                        var referencedTableDefinition = collection.GetTableDefinition(referenceColumn.ReferencedTableName);

                        yield return (referencedTableDefinition, referenceColumn.ReferencedPropertyName, fieldDifference.Before);
                        yield return (referencedTableDefinition, referenceColumn.ReferencedPropertyName, fieldDifference.After);
                    }
                }
            }

        }
    }

}