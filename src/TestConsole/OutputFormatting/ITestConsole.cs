using System.Collections.Generic;

namespace TestConsole.OutputFormatting
{
    /// <summary>
    /// The base set of operations supported for writing data to the console.
    /// </summary>
    public interface IConsoleOperations
    {
        /// <summary>
        /// Output a formatted string at the current cursor position, and move to the beginning of the next line.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        void WriteLine(string format, params object[] arg);

        /// <summary>
        /// Output a formatted string at the current cursor position.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        void Write(string format, params object[] arg);

        /// <summary>
        /// Output a formatted string at the current cursor position, using word wrapping. Then move to the beginning of the next line.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        void WrapLine(string format, params object[] arg);

        /// <summary>
        /// Output a formatted string at the current cursor position, but use word wrapping.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        void Wrap(string format, params object[] arg);

        /// <summary>
        /// Render a renderable object to the console.
        /// </summary>
        /// <param name="renderableData">The object to render.</param>
        void Write(IConsoleRenderer renderableData);

        /// <summary>
        /// Render a renderable object to the console, add a newline.
        /// </summary>
        /// <param name="renderableData">The object to render.</param>
        void WriteLine(IConsoleRenderer renderableData);

        /// <summary>
        /// Format an enumerable set of rows as a tabular report.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items to be formatted.</param>
        /// <param name="options">Options that effect the way in which the table is formatted.</param>
        /// <param name="columnSeparator">The text to use to separate columns.</param>
        /// <param name="title">The title for the tabular report.</param>
        void FormatTable<T>(IEnumerable<T> items, ReportFormattingOptions options = ReportFormattingOptions.Default,
            string columnSeparator = null, string title = null);

        /// <summary>
        /// Format an enumerable set of rows as a tabular report, using a report definition.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="report">The report definition.</param>
        void FormatTable<T>(Report<T> report);

        /// <summary>
        /// Format an object using the ObjectReporter.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="item">The item to format.</param>
        void FormatObject<T>(T item);

        /// <summary>
        /// Output a new line.
        /// </summary>
        void WriteLine();
    }

    /// <summary>
    /// This interface defines the functionality required to support rendering recorded console activity
    /// in the various ways that a the data is used by the toolkit.
    /// </summary>
    public interface IConsoleRenderer
    {
        IEnumerable<string> Render(int width, out int wrappedLines);
        int GetFirstWordLength(int tabLength);
        int GetLongestWordLength(int tabLength);
        int CountWordWrapLineBreaks(int width);
    }
}