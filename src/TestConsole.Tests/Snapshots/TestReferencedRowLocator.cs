using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Snapshots;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.Snapshots
{
    [TestFixture]
    public class TestReferencedRowLocator
    {
        #region Types that define tables

        [SnapshotDefinition("Table")]
        public static class TableDef
        {
            [Key] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("Table2")]
        public static class TableDef2
        {
            [Key] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("ReferencingTable")]
        public static class RelatedTableDef
        {
            [Key] 
            public static int Id { get; set; }
            
            [References("Table", nameof(TableDef2.Id))]
            public static int? ParentId { get; set; }
        }

        [SnapshotDefinition("ReferencingTable2")]
        public static class RelatedTableDef2
        {
            [Key] 
            public static int Id { get; set; }
            
            [References("Table2", nameof(TableDef2.Id))]
            public static int? ParentId { get; set; }
        }

        #endregion

        #region Tables to snapshot

        public class Table
        {
            public int Id { get; set; }
            public string Variable { get; set; }
        }

        public class Table2
        {
            public int Id { get; set; }
            public string Variable { get; set; }
        }

        public class ReferencingTable
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string Variable { get; set; }
        }

        public class ReferencingTable2
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string Variable { get; set; }
        }

        #endregion
        
        private SnapshotCollection _collection;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection(GetType());
        }

        private List<T> Make<T>(params int[] keys)
        {
            return keys.Select(k => MakeInstance<T>(k)).ToList();
        }

        private List<T> Make<T>(params (int Id, int? ParentId)[] keys)
        {
            return keys.Select(k => MakeInstance<T>(k.Id, k.ParentId)).ToList();
        }

        private T MakeInstance<T>(int id, int? parentId = null)
        {
            var instance = (T) Activator.CreateInstance<T>();
            var idProp = typeof(T).GetProperty("Id");
            Debug.Assert(idProp != null, nameof(idProp) + " != null");
            idProp.SetValue(instance, id);

            var varProp = typeof(T).GetProperty("Variable");
            Debug.Assert(varProp != null, nameof(varProp) + " != null");
            varProp.SetValue(instance, $"Default variable for Id {id}");

            if (parentId != null)
            {
                var parentProp = typeof(T).GetProperty("ParentId");
                Debug.Assert(parentProp != null, nameof(parentProp) + " != null");
                parentProp.SetValue(instance, parentId);
            }
            return instance;
        }

        [Test]
        public void IfAllReferencesArePresentAnEmptyResultIsReturned()
        {
            //Arrange
            var table1 = Make<Table>(1, 2, 3, 4, 5);
            var table2 = Make<Table2>(1, 2, 3, 4, 5);
            var refTable1 = Make<ReferencingTable>((1, 1),(2, 1));
            var refTable2 = Make<ReferencingTable2>((1, 3),(2, 4));

            void TakeSnapshot(string name)
            {
                var builder = _collection.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                table2.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
                refTable2.ToSnapshotTable(builder);
            }

            TakeSnapshot("Before");
            table1[0].Variable = "edited";
            table1[1].Variable = "edited";
            table2[2].Variable = "edited";
            table2[3].Variable = "edited";
            refTable1[0].Variable = "edited";
            refTable1[1].Variable = "edited";
            refTable2[0].Variable = "edited";
            refTable2[1].Variable = "edited";
            TakeSnapshot("After");

            var before = _collection.GetSnapshot("Before");
            var after = _collection.GetSnapshot("After");

            var diffs = SnapshotDifferenceAnalyser.ExtractDifferences(_collection, before, after);

            //Act
            var result = ReferencedRowLocator.GetMissingRows(_collection, diffs.TableDifferences);

            //Assert
            result.Tables.Should().BeEmpty();
        }

        [Test]
        public void UneditedReferencedRowsAreExtracted()
        {
            //Arrange
            var table1 = Make<Table>(1, 2, 3, 4, 5);
            var table2 = Make<Table2>(1, 2, 3, 4, 5);
            var refTable1 = Make<ReferencingTable>((1, 1),(2, 2));
            var refTable2 = Make<ReferencingTable2>((1, 1),(2, 2));

            void TakeSnapshot(string name)
            {
                var builder = _collection.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                table2.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
                refTable2.ToSnapshotTable(builder);
            }

            TakeSnapshot("Before");
            table1[0].Variable = "edited";
            table2[1].Variable = "edited";
            refTable1[0].Variable = "edited";
            refTable1[1].ParentId = 3; //will required table1 row with key of 3 and also 2 because that is the before
            refTable2[0].Variable = "edited";
            refTable2[1].ParentId = 5; //will required table2 row with key of 5 but not 2 (the before) because it is edited and therefore already present
            TakeSnapshot("After");

            var before = _collection.GetSnapshot("Before");
            var after = _collection.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_collection, before, after);

            //Act
            var result = ReferencedRowLocator.GetMissingRows(_collection, diffs);

            //Assert
            var output = new Output();
            var resultRep = result.Tables.AsReport(rep => rep.RemoveBufferLimit()
                .AddColumn(t => t.TableDefinition.TableName)
                .AddChild(t => t.Keys, trep => trep.RemoveBufferLimit()
                    .AddColumn(c => c.ColumnName)
                    .AddColumn(c => c.RequestedValue)));
            output.FormatTable(resultRep); output.Report.Verify();

        }
    }
}
