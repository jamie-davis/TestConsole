using System.Text;

namespace TestConsole.OutputFormatting
{
    /// <summary>
    /// This interface describes a class that captures output data.
    /// </summary>
    public interface IConsoleInterface
    {
        /// <summary>
        /// The width of the console buffer. This may be greater than the window width.
        /// <seealso cref="BufferWidth"/>
        /// </summary>
        int BufferWidth { get; }

        /// <summary>
        /// Output a string to the console in the current cursor position.
        /// </summary>
        /// <param name="data">The text to output.</param>
        /// <param name="limitWidth">True indicates that the buffer width should be respected.</param>
        void Write(string data, bool limitWidth = true);

        /// <summary>
        /// Output a new line to the console at the current cursor position. The cursor will move the beginning of the next line.
        /// </summary>
        void NewLine();

        /// <summary>
        /// The current cursor position.
        /// </summary>
        int CursorLeft { get; set; }

        /// <summary>
        /// The current cursor position.
        /// </summary>
        int CursorTop { get; set; }

        /// <summary>
        /// The encoding being used by the console.
        /// </summary>
        Encoding Encoding { get; }
    }
}