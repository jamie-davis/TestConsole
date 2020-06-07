using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TestConsole.OutputFormatting.Internal;
using TestConsole.OutputFormatting.ReportDefinitions;
using TestConsole.Tests.OutputFormatting.ReportDefinitions;
using TestConsoleLib.Snapshots;

namespace TestConsole.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotColumnFormatter
    {
        private const string TestTableName = "Test";
        private SnapshotCollection _collection;
        private SnapshotBuilder _snapshot;
        private RowBuilder _row;
        private TableDefinition _table;
        private readonly Expression<Func<object, string>> _stringExp = t => t.ToString();
        private ColumnConfig _cc;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable(TestTableName).CompareKey("Id");
            _table = _collection.GetTableDefinition(TestTableName);
            _snapshot = _collection.NewSnapshot("Test");
            _cc = new ColumnConfig(_stringExp);
            _cc.MakeFormat<string>();
        }

        [Test]
        public void HeadingIsSet()
        {
            //Arrange
            var col = new SnapshotColumnInfo("ColName");
            
            //Act
            SnapshotColumnFormatter.Format(_cc, _table, col);

            //Assert
            _cc.FinalizeColumnSettings();
            _cc.ColumnFormat.Heading.Should().Be("ColName");
        }

        [Test]
        [TestCase(1.5, ColumnAlign.Right)]
        [TestCase(1, ColumnAlign.Right)]
        [TestCase((short)1, ColumnAlign.Right)]
        [TestCase((byte)1, ColumnAlign.Right)]
        [TestCase("Text", ColumnAlign.Left)]
        public void AlignmentIsSetForColumn(object value, object expected)
        {
            //Arrange
            _snapshot.AddNewRow(TestTableName)["Col"] = value;
            var col = _table.Columns.Single(c => c.Name == "Col");
            
            //Act
            SnapshotColumnFormatter.Format(_cc, _table, col);

            //Assert
            _cc.FinalizeColumnSettings();
            _cc.ColumnFormat.Alignment.Should().Be(expected);
        }

        [Test]
        [TestCase(1.5, 1, ColumnAlign.Right)]
        [TestCase(1, "Text", ColumnAlign.Left)]
        public void AlignmentIsSetForMixedColumn(object value1, object value2, object expected)
        {
            //Arrange
            _snapshot.AddNewRow(TestTableName)["Col"] = value1;
            _snapshot.AddNewRow(TestTableName)["Col"] = value2;
            var col = _table.Columns.Single(c => c.Name == "Col");
            
            //Act
            SnapshotColumnFormatter.Format(_cc, _table, col);

            //Assert
            _cc.FinalizeColumnSettings();
            _cc.ColumnFormat.Alignment.Should().Be(expected);
        }
    }
}
