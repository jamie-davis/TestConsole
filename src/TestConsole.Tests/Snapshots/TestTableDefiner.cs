using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TestConsoleLib.Snapshots;

namespace TestConsole.Tests.Snapshots
{
    [TestFixture]
    public class TestTableDefiner
    {
        private const string TestTableName = "Test";
        private SnapshotCollection _collection;
        private TableDefiner _definer;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _definer = _collection.DefineTable(TestTableName);
        }

        [Test]
        public void PrimaryKeyCanBeSet()
        {
            //Act
            _definer.PrimaryKey("Key1");

            //Assert
            _collection.GetTableDefinition(TestTableName)
                .PrimaryKeys
                .Single()
                .Should().Be("Key1");
        }

        [Test]
        public void CompoundPrimaryKeyCanBeSet()
        {
            //Act
            _definer.PrimaryKey("Key1").PrimaryKey("Key2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).PrimaryKeys)
                .Should().Be("Key1, Key2");
        }

        [Test]
        public void CompareKeyCanBeSet()
        {
            //Act
            _definer.CompareKey("Key1");

            //Assert
            _collection.GetTableDefinition(TestTableName)
                .CompareKeys
                .Single()
                .Should().Be("Key1");
        }

        [Test]
        public void CompoundCompareKeyCanBeSet()
        {
            //Act
            _definer.CompareKey("Key1").CompareKey("Key2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).CompareKeys)
                .Should().Be("Key1, Key2");
        }

        [Test]
        public void FieldCanBeFlaggedAsUnpredictable()
        {
            //Act
            _definer.IsUnpredictable("Key1").IsUnpredictable("Key2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).Unpredictable)
                .Should().Be("Key1, Key2");
        }
    }
}