using System;
using System.Collections.Generic;
using System.Text;

namespace TestConsoleLib.Exceptions
{
    public class UndefinedTableInSnapshotException : Exception
    {
        public string TableName { get; }

        internal UndefinedTableInSnapshotException(string tableName) : base("Undefined table in snapshot. You must define all tables on the collection before populating them in a snapshot.")
        {
            TableName = tableName;
        }
    }
}
