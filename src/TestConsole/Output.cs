using System.Collections.Generic;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.Internal;

namespace TestConsoleLib
{
    /// <summary>
    /// A class providing a buffer for recording test output. Various formatting methods are provided
    /// to create text with a readable layout.
    /// </summary>
    public sealed class Output : IConsoleOperations
    {
        private ConsoleOperationsImpl _impl;
        private OutputBuffer _buffer;

        /// <summary>
        /// Construct with a default buffer.
        /// </summary>
        public Output()
        {
            _buffer = new OutputBuffer();
            _buffer.BufferWidth = 95;
            _impl = new ConsoleOperationsImpl(_buffer);
        }

        /// <summary>
        /// Construct with a pre-defined buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use. This may be shared if required.</param>
        public Output(OutputBuffer buffer)
        {
            _buffer = buffer;
            _impl = new ConsoleOperationsImpl(_buffer);
        }

        /// <summary>
        /// Access this property to combine the buffered output into a single large string.
        /// </summary>
        public string Report
        {
            get { return _buffer.GetBuffer(); }
        }

        /// <summary>
        /// Output a formatted string at the current cursor position, and move to the beginning of the next line.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        public void WriteLine(string format, params object[] arg)
        {
            _impl.WriteLine(format, arg);
        }

        /// <summary>
        /// Output a formatted string at the current cursor position.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        public void Write(string format, params object[] arg)
        {
            _impl.Write(format, arg);
        }

        /// <summary>
        /// Output a formatted string at the current cursor position, using word wrapping. Then move to the beginning of the next line.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        public void WrapLine(string format, params object[] arg)
        {
            _impl.WrapLine(format, arg);
        }

        /// <summary>
        /// Output a formatted string at the current cursor position, but use word wrapping.
        /// </summary>
        /// <param name="format">A format string.</param>
        /// <param name="arg">Replacement args for the format string.</param>
        public void Wrap(string format, params object[] arg)
        {
            _impl.Wrap(format, arg);
        }

        /// <summary>
        /// Render a renderable object to the console.
        /// </summary>
        /// <param name="renderableData">The object to render.</param>
        public void Write(IConsoleRenderer renderableData)
        {
            _impl.Write(renderableData);
        }

        /// <summary>
        /// Render a renderable object to the console, add a newline.
        /// </summary>
        /// <param name="renderableData">The object to render.</param>
        public void WriteLine(IConsoleRenderer renderableData)
        {
            _impl.WriteLine(renderableData);
        }

        /// <summary>
        /// Format an enumerable set of rows as a tabular report.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items to be formatted.</param>
        /// <param name="options">Options that effect the way in which the table is formatted.</param>
        /// <param name="columnSeperator">The text to use to seperate columns.</param>
        public void FormatTable<T>(IEnumerable<T> items, ReportFormattingOptions options = ReportFormattingOptions.Default, string columnSeperator = null)
        {
            _impl.FormatTable(items, options, columnSeperator);
        }

        /// <summary>
        /// Format an enumerable set of rows as a tabular report, using a report definition.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="report">The report definition.</param>
        public void FormatTable<T>(Report<T> report)
        {
            _impl.FormatTable(report);
        }

        public void FormatObject<T>(T item)
        {
            _impl.FormatObject(item);
        }

        /// <summary>
        /// Output a new line.
        /// </summary>
        public void WriteLine()
        {
            _impl.WriteLine();
        }
    }
}
