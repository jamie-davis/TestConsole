using System;
using System.Collections.Generic;
using System.Text;
using TestConsoleLib.Snapshots;

namespace TestConsole.Tests.Snapshots.TestDefinitions
{
    [SnapshotDefinition("RelatedTable")]
    public static class RelatedTableDef
    {
        [Unpredictable] 
        [Key]
        public static int Id { get; set; }
            
        [References("ParentTable", nameof(TestSnapshotDefinitionLoader.ParentTableDef.Id))] //implied unpredictable because the referenced property is unpredictable
        public static int ParentId1 { get; set; }
    }
}
