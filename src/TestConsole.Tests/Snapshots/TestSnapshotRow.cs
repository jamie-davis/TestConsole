using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TestConsoleLib;
using TestConsoleLib.Snapshots;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotRow
    {
        private SnapshotCollection _collection;
        private SnapshotBuilder _snapshot;
        private RowBuilder _row;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable("Test").CompareKey("Key");
            _snapshot = _collection.NewSnapshot("TestSnapshot");
            _row = _snapshot.AddNewRow("Test");
        }

        [Test]
        public void FieldsAreSetInTheRow()
        {
            //Act
            _row["Key"] = "key";
            _row["Field"] = "value";

            //Assert
            var output = new Output();
            _snapshot.ReportContents(output, "Test");
            output.Report.Verify();
        }
    }
}
