﻿using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TestConsoleLib;
using TestConsoleLib.Snapshots;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.Snapshots
{
    [TestFixture]
    public class TestRowBuilder
    {
        private const string TestTableName = "Test";
        private SnapshotCollection _collection;
        private SnapshotBuilder _snapshot;
        private RowBuilder _row;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable("Test").CompareKey("Id");
            _snapshot = _collection.NewSnapshot("Test");
            _row = _snapshot.AddNewRow("Test");
        }

        [Test]
        public void ARowBuilderIsCreated()
        {
            //Assert
            _row.Should().BeOfType<RowBuilder>();
        }

        [Test]
        public void PrimaryKeyCanBeSet()
        {
            //Act
            _row["Id"] = 100;

            //Assert
            var output = new Output();
            _snapshot.ReportContents(output, "Test");
            output.Report.Verify();
        }

        [Test]
        public void NonKeyFieldCanBeSet()
        {
            //Act
            _row["NotKey"] = 100;

            //Assert
            var output = new Output();
            _snapshot.ReportContents(output, "Test");
            output.Report.Verify();
        }
    }
}