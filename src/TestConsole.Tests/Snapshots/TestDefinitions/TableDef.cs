﻿using TestConsoleLib.Snapshots;

namespace TestConsole.Tests.Snapshots.TestDefinitions
{
    [SnapshotDefinition("Table")]
    public static class TableDef
    {
        [Unpredictable] 
        [Key]
        public static int Id { get; set; }
    }
}