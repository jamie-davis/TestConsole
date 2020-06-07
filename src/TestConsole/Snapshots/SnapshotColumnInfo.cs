﻿using System;
using System.Collections.Generic;
using System.Linq;
using TestConsoleLib.Exceptions;

namespace TestConsoleLib.Snapshots
{
    internal class SnapshotColumnInfo
    {
        private readonly object _lock = new object();

        private readonly List<Type> _observedTypes = new List<Type>();

        /// <summary>
        /// The name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// This field is part of the primary key for the object, and can reliably be used to identify a unique instance.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// This field is flagged as forming part of a key that will work for comparison. Generally, this is not the primary key, and is instead a
        /// field that will be unique within the scope of the test data only.
        /// </summary>
        public bool IsCompareKey { get; set; }

        /// <summary>
        /// The field has a value that is programmatically assigned and cannot be predicted under normal circumstances. For example, Guids, Timestamps and
        /// incrementing keys.
        /// </summary>
        public bool IsUnpredictable { get; set; }

        /// <summary>
        /// The name of the table containing a field referenced by this column. If this is set, <see cref="ReferencedPropertyName"/> must also be set.
        /// </summary>
        public string ReferencedTableName { get; private set; }

        /// <summary>
        /// The name of a field referenced by this column. If this is set, <see cref="ReferencedTableName"/> must also be set.
        /// </summary>
        public string ReferencedPropertyName { get; private set; }

        /// <summary>
        /// The C# types containing this field. This is optionally recorded when snapshots are constructed automatically.
        /// </summary>
        public IEnumerable<Type> ObservedTypes
        {
            get
            {
                lock (_lock) 
                    return _observedTypes.ToList();
            }
        }

        public SnapshotColumnInfo(string columnName)
        {
            Name = columnName;
        }

        /// <summary>
        /// Called when the type from which the field receives a value is known.
        /// </summary>
        public void TypeObserved(Type observedType)
        {
            lock (_lock)
            {
                if (!_observedTypes.Contains(observedType))
                    _observedTypes.Add(observedType);
            }
        }

        /// <summary>
        /// Called to indicate a "foreign key" style reference to a field in another table. This is important when obfuscating unpredictable values, and for
        /// ensuring that comparisons are able to show rows that do not have changes but are referenced by an update or insert.
        /// </summary>
        /// <exception cref="AmbiguosReferenceInSnapshotColumn">Thrown if the reference is already set to a different table and field.</exception>
        public void References(string referencedTableName, string referencedPropertyName)
        {
            if (ReferencedTableName == referencedTableName && ReferencedPropertyName == referencedPropertyName)
                return;

            if (ReferencedTableName != null)
                throw new AmbiguosReferenceInSnapshotColumn(Name, ReferencedTableName, ReferencedPropertyName,
                    referencedTableName, referencedPropertyName);

            ReferencedTableName = referencedTableName;
            ReferencedPropertyName = referencedPropertyName;
        }
    }
}