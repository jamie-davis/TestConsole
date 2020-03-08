using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestConsole.OutputFormatting
{
    /// <summary>
    /// This class implements the <see cref="IConsoleOutInterface"/> and captures the console output in a format that facilitates
    /// examination of console output in a unit test.
    /// </summary>
    public class OutputBuffer : IConsoleInterface
    {
        private readonly List<string> _buffer = new List<string>();

        /// <summary>
        /// The current cursor position.
        /// </summary>
        private int _cursorTop;

        /// <summary>
        /// The current cursor position.
        /// </summary>
        private int _cursorLeft;

        /// <summary>
        /// The console encoding.
        /// </summary>
        private Encoding _encoding;

        /// <summary>
        /// The input stream supplying input for the console.
        /// </summary>
        private TextReader _inputStream;

        /// <summary>
        /// The maximum number of lines the buffer may contain. Zero or less means no limit.
        /// </summary>
        private int _lengthLimit;

        /// <summary>
        /// The width of the buffer.
        /// </summary>
        public int BufferWidth { get; set; }

        /// <summary>
        /// The current cursor position.
        /// </summary>
        public int CursorLeft
        {
            get { return _cursorLeft; }
            set { _cursorLeft = value; }
        }

        /// <summary>
        /// The current cursor position.
        /// </summary>
        public int CursorTop
        {
            get { return _cursorTop; }
            set { _cursorTop = value; }
        }

        public Encoding Encoding { get { return _encoding; } }

        /// <summary>
        /// The constructor sets default values for various console properties and allows an encoding to be specified.
        /// </summary>
        public OutputBuffer(Encoding encoding = null)
        {
            _encoding = encoding ?? Encoding.Default;
            BufferWidth = 95;
        }

        /// <summary>
        /// Write some text to the console buffer. Does not add a line feed.
        /// </summary>
        /// <param name="data">The text data to write. This must not contain colour instructions.</param>
        /// <param name="limitWidth">True indicates that the buffer width should be respected.</param>
        public void Write(string data, bool limitWidth = true)
        {
            while (data.Contains(Environment.NewLine) || (data.Length + _cursorLeft > BufferWidth && limitWidth))
            {
                string nextData;
                var usableLength = limitWidth ? BufferWidth - _cursorLeft : data.Length; 
                var newlinePos = data.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                if (newlinePos >= 0 && newlinePos < usableLength)
                {
                    nextData = data.Substring(0, newlinePos);
                    data = data.Substring(newlinePos + Environment.NewLine.Length);
                    Write(nextData);
                    NewLine();
                }
                else
                {
                    nextData = data.Substring(0, usableLength);
                    data = data.Substring(usableLength);
                    Write(nextData, limitWidth);
                }

            }

            CreateBufferTo(_cursorTop);

            OverWrite(_buffer, _cursorTop, _cursorLeft, data, limitWidth);

            _cursorLeft += data.Length;

            if (_cursorLeft >= BufferWidth && limitWidth)
            {
                _cursorTop++;
                _cursorLeft = 0;
                CreateBufferTo(_cursorTop);
            }

        }

        /// <summary>
        /// Overlay some text in an existing buffer. The method will discard any data that would overflow the buffer width.
        /// </summary>
        /// <param name="buffer">The buffer line array.</param>
        /// <param name="lineIndex">The index of the line to overwrite</param>
        /// <param name="overwritePosition">The position within the line to overwrite.</param>
        /// <param name="data">The text to place in the buffer at the specified position.</param>
        /// <param name="limitWidth">True indicates that the buffer width should be respected.</param>
        private void OverWrite(IList<string> buffer, int lineIndex, int overwritePosition, string data, bool limitWidth)
        {
            var line = buffer[lineIndex];

            if (overwritePosition >= line.Length)
                return;

            var newLine = overwritePosition > 0 ? line.Substring(0, overwritePosition) : string.Empty;
            newLine += data;

            if (newLine.Length < line.Length)
                newLine += line.Substring(newLine.Length); //copy the remainder of the line from the original data

            if (newLine.Length > BufferWidth && limitWidth)
                newLine = newLine.Substring(0, BufferWidth);
            else if (newLine.Length < BufferWidth)
                newLine += new string(' ', BufferWidth - newLine.Length);

            buffer[lineIndex] = newLine;
        }

        /// <summary>
        /// Ensure that the buffer contains the specified line.
        /// </summary>
        /// <param name="ix">The zero based index of the line that must exist.</param>
        private void CreateBufferTo(int ix)
        {
// ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (ix >= _buffer.Count)
            {
                _buffer.Add(new string(' ', BufferWidth));
            }

            if (_lengthLimit > 0)
            {
                while (_buffer.Count > _lengthLimit)
                {
                    _buffer.RemoveAt(0);
                }

                if (CursorTop >= _lengthLimit)
                    CursorTop = _lengthLimit - 1;
            }
        }

        /// <summary>
        /// Write a newline to the buffer.
        /// </summary>
        public void NewLine()
        {
            _cursorTop++;
            _cursorLeft = 0;
            CreateBufferTo(_cursorTop);
        }

        /// <summary>
        /// Return the entire buffer for testing purposes. It is possible to get just the text, just the colour information or all of the data.
        /// </summary>
        /// <returns>A large string containing the requested data.</returns>
        public string GetBuffer()
        {
            return string.Join(Environment.NewLine, _buffer);
        }

        /// <summary>
        /// Provide a text stream to provide the data for the console input stream.
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        public void SetInputStream(TextReader stream)
        {
            _inputStream = stream;
        }

        public void LimitBuffer(int maxLines)
        {
            _lengthLimit = maxLines;
        }
    }
}
