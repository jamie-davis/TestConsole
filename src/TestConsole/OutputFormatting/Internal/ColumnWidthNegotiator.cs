using System.Collections.Generic;
using System.Linq;

namespace TestConsole.OutputFormatting.Internal
{
    /// <summary>
    /// Calculate a width for each column in a tabular report.
    /// </summary>
    internal class ColumnWidthNegotiator
    {
        internal class ColumnSizerInfo
        {
            public PropertyColumnFormat PropertyColumnFormat { get; private set; }
            public ColumnSizer Sizer { get; private set; }

            public ColumnSizerInfo(PropertyColumnFormat pcf, int tabLength, SplitCache cache)
            {
                var propertyType = pcf.Property == null ? pcf.Format.Type : pcf.Property.PropertyType;
                Sizer = new ColumnSizer(propertyType, cache, pcf.Format, tabLength);
                PropertyColumnFormat = pcf;
            }

            public override string ToString()
            {
                return string.Format("ColumnSizerInfo: {0}, W:{1}", PropertyColumnFormat.Format.Heading, PropertyColumnFormat.Format.ActualWidth);
            }

            public bool WidthIsProportional()
            {
                return PropertyColumnFormat.Format.FixedWidth == 0 && PropertyColumnFormat.Format.ProportionalWidth > 0.0;
            }
        }

        private readonly ColumnSizingParameters _parameters = new ColumnSizingParameters();
        private readonly SplitCache _splitCache = new SplitCache();
        private int _sizeRows;
        private int _headingsRowIndex = -1;
        private List<object> _rowItems = new List<object>();
        public List<PropertyColumnFormat> Columns { get { return _parameters.Columns; } }
        public int AvailableWidth { get; private set; }
        public int BaseWidth { get; private set; }

        public List<PropertyColumnFormat> StackedColumns
        {
            get { return _parameters.StackSizer == null ? new List<PropertyColumnFormat>() : _parameters.StackSizer.Columns.ToList(); }
        }

        public int StackedColumnWidth { get { return _parameters.StackedColumnWidth; } }
        public int TabLength { get { return _parameters.TabLength; } }

        public ColumnWidthNegotiator(List<PropertyColumnFormat> columns, int separatorLength, int tabLength = 4)
        {
            _parameters.Columns = columns;
            _parameters.SeparatorLength = separatorLength;
            _parameters.TabLength = tabLength;
            _parameters.Sizers = columns
                .Select(c => new ColumnSizerInfo(c, tabLength, _splitCache))
                .ToList();
        }

        public void AddRow(object row)
        {
            foreach (var sizer in _parameters.Sizers)
            {
                object value;
                if (sizer.PropertyColumnFormat.Property == null)
                    value = row;
                else
                    value = sizer.PropertyColumnFormat.Property.GetValue(row, null);
                sizer.Sizer.ColumnValue(value);
            }

            _rowItems.Add(row);
            ++_sizeRows;
        }

        public void AddRow<T>(CachedRow<T> row)
        {
            var dataDictionary = row.Columns.ToDictionary(c => c.Property, c => c.Value);
            foreach (var sizer in _parameters.Sizers)
            {
                object value;
                if (dataDictionary.TryGetValue(sizer.PropertyColumnFormat.Property, out value))
                    sizer.Sizer.ColumnValue(value);
                else
                    sizer.Sizer.ColumnValue(null);
            }

            _rowItems.Add(row.RowItem);
            ++_sizeRows;
        }

        public void CalculateWidths(int width, ReportFormattingOptions options = ReportFormattingOptions.Default)
        {
            BaseWidth = width;
            AvailableWidth = SizeColumns(width, (options & ReportFormattingOptions.UnlimitedBuffer) == 0);

            if (_parameters.ProportionalColumnSizingRequired)
            {
                SizeProportionalColumns(width);
            }
            else
            {
                var maximiseWidth = (options & ReportFormattingOptions.StretchColumns) > 0;
                StretchColumnsToFillWidth(width, maximiseWidth);
            }
        }

        private void SizeProportionalColumns(int width)
        {
            ProportionalColumnSizer.Size(width, SeparatorOverhead(), _parameters);
        }

        private int SizeColumns(int width, bool limitWidth)
        {
            var separatorOverhead = SeparatorOverhead();
            return ColumnShrinker.ShrinkColumns(width, separatorOverhead, _parameters, limitWidth);
        }

        private void StretchColumnsToFillWidth(int width, bool maximiseWidth)
        {
            var separatorOverhead = SeparatorOverhead();
            var stackedColumnWidth = _parameters.StackedColumnWidth;
            ColumnExpander.FillAvailableSpace(width, separatorOverhead, _parameters, maximiseWidth);
            _parameters.StackedColumnWidth = stackedColumnWidth;
        }

        private int SeparatorOverhead()
        {
            var separatorCount = Columns.Count - 1 + (StackedColumns.Any() ? 1 : 0);
            var separatorOverhead = separatorCount * _parameters.SeparatorLength;
            return separatorOverhead;
        }

        public void AddHeadings()
        {
            foreach (var sizer in _parameters.Sizers)
            {
                string value = null;
                var headingWords = _splitCache.Split(sizer.PropertyColumnFormat.Format.Heading, _parameters.TabLength);
                if (headingWords.Any())
                {
                    var longest = headingWords.Max(w => w.Length);
                    value = new string('X', longest);
                }

                if (value == null)
                    value = sizer.PropertyColumnFormat.Format.Heading;

                sizer.Sizer.ColumnValue(value);
            }

            _headingsRowIndex = _sizeRows++;
        }

        public IEnumerable<SizingValues> GetSizingValues()
        {
            var dataRow = 0;
            for (var row = 0; row < _sizeRows; ++row)
            {
                if (row != _headingsRowIndex)
                {
                    // ReSharper disable once AccessToModifiedClosure
                    //use of the row number within the delegate is okay because we only use it here, within the loop 
                    var sizedColumnValues = _parameters.Sizers.Select(s => s.Sizer.GetSizeValue(row));
                    var rowItem = _rowItems[dataRow++];
                    if (_parameters.StackSizer != null)
                    {
                        var intermediates = sizedColumnValues.Concat(_parameters.StackSizer.GetSizeValues(row));
                        yield return new SizingValues(rowItem, intermediates);
                    }
                    else
                        yield return new SizingValues(rowItem, sizedColumnValues);
                }
            }
        }

        public class SizingValues
        {
            private readonly IEnumerable<FormattingIntermediate> _values;

            public SizingValues(object rowItem, IEnumerable<FormattingIntermediate> values)
            {
                RowItem = rowItem;
                _values = values;
            }

            public object RowItem { get; private set; }

            public IEnumerable<FormattingIntermediate> GetValues()
            {
                return _values;
            }
        }
    }
}