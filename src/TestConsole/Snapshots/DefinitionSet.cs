﻿using System.Collections.Generic;
using System.Linq;

namespace TestConsoleLib.Snapshots
{
    internal class DefinitionSet
    {
        public DefinitionSet(IEnumerable<TableDefinition> tables)
        {
            Tables = tables.ToList();
        }

        public IReadOnlyList<TableDefinition> Tables { get; }
    }
}