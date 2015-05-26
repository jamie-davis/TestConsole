using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestConsole.OutputFormatting.Internal
{
    /// <summary>
    /// Data cached from an <see cref="IEnumerable{T}"/> collection. This store can be used to repeatedly format
    /// the data at different widths without having to enumerate the original collection multiple times.
    /// <seealso cref="CachedRow{T}"/>
    /// <seealso cref="CachedColumn"/>
    /// </summary>
    /// <typeparam name="T">The original row type.</typeparam>
    internal class CachedRows<T>
    {
        private readonly List<CachedRow<T>> _rows;

        public CachedRows(IEnumerable<T> rows)
        {
            _rows = rows.Select(r => new CachedRow<T>(r)).ToList();
        }

        public IEnumerable<CachedRow<T>> GetRows()
        {
            return _rows;
        }
    }

    /// <summary>
    /// Static class exposing a factory method that allows caches of anonymous types to be constructed.
    /// </summary>
    internal static class CachedRowsFactory
    {
        public static CachedRows<T> Make<T>(IEnumerable<T> rows)
        {
            return new CachedRows<T>(rows);
        }
    }

    /// <summary>
    /// A cached row of data. Used in <see cref="CachedRows{T}"/>.
    /// <seealso cref="CachedColumn"/>
    /// </summary>
    /// <typeparam name="T">The original row's type.</typeparam>
    internal class CachedRow<T>
    {
        public CachedRow(T item)
        {
            RowItem = item;
            Columns = typeof (T).GetProperties().Select(p => new CachedColumn(p, p.GetValue(item))).ToList();
        }

        public T RowItem { get; set; }

        public IEnumerable<CachedColumn> Columns { get; private set; }
    }

    /// <summary>
    /// A cached column value. Used in <see cref="CachedRow{T}"/>.
    /// <seealso cref="CachedRows{T}"/>
    /// </summary>
    internal class CachedColumn
    {
        public PropertyInfo Property { get; private set; }
        public object Value { get; private set; }

        public CachedColumn(PropertyInfo property, object value)
        {
            Property = property;
            Value = value;
        }

        public string Format(ColumnFormat format)
        {
            return ValueFormatter.Format(format, Value);
        }
    }
}