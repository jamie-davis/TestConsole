using System;

namespace TestConsole.OutputFormatting
{
    /// <summary>
    /// Options that control the formatting of a report.
    /// </summary>
    [Flags]
    public enum ReportFormattingOptions
    {
        /// <summary>
        /// Skip the headings when emitting the report. This will cause the field names or headings for each column to be disregarded.
        /// </summary>
        OmitHeadings = 1,

        /// <summary>
        /// Fill the available width
        /// </summary>
        StretchColumns = 2,

        /// <summary>
        /// If column definitions are used, include columns without a definition.
        /// </summary>
        IncludeAllColumns = 4,

        /// <summary>
        /// If child reports are present, do not repeat the report headings. By default, the report headings will be displayed again when a row has a child report. Use this option to suppress the headings repetition.
        /// </summary>
        SuppressHeadingsAfterChildReport = 8,

        /// <summary>
        /// Stretch the buffer width to fit all columns.
        /// </summary>
        UnlimitedBuffer = 16,

        Default = 0,
    }
}