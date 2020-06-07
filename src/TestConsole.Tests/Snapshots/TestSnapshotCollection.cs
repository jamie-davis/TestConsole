using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TestConsoleLib;
using TestConsoleLib.Snapshots;
using TestConsoleLib.Testing;

namespace TestConsole.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotCollection
    {
        #region Types that define tables

        [SnapshotDefinition("LocalTable")]
        public static class TableDef
        {
            [Unpredictable] 
            [Key] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("LocalParentTable")]
        public static class ParentTableDef
        {
            [Unpredictable] 
            [Key] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("LocalReferencingTable")]
        public static class RelatedTableDef
        {
            [Unpredictable]
            [Key] 
            public static int Id { get; set; }
            
            [References("ParentTable", nameof(ParentTableDef.Id))] //implied unpredictable because the referenced property is unpredictable
            public static int ParentId1 { get; set; }
        }

        #endregion

        [Test]
        public void TablesAreDefined()
        {
            //Arrange
            var snapshots = new SnapshotCollection();

            //Act
            snapshots.DefineTable("Customer").PrimaryKey("Id").CompareKey("Surname");
            snapshots.DefineTable("Address").PrimaryKey("AddressId");
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }

        [Test]
        public void TableDefinitionsAreLoadedFromAssembly()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType().Assembly);
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }

        [Test]
        public void TableDefinitionsAreFilteredLoadedFromAssembly()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType().Assembly, t => t != typeof(TestDefinitions.TableDef));
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }

        [Test]
        public void TableDefinitionsAreLoadedFromType()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType());
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }

        [Test]
        public void TableDefinitionsAreFilteredLoadedFromType()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType(), t => t != typeof(TestDefinitions.TableDef));
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }
    }
}
