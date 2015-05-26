using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TestConsole.OutputFormatting.Internal.RecordedCommands
{
    internal class ReplayBuffer
    {
        private static readonly ColumnFormat DefaultColumnFormat = new ColumnFormat(null);
        public int Width { get; private set; }
        public static int _Buffers = 0/**/;

        /// <summary>
        /// The current cursor position.
        /// </summary>
        public int CursorLeft
        {
            get { return _currentLineLength; }
        }

        public int WordWrapLineBreakCount { get; private set; }

        private readonly List<string> _lines = new List<string>();
        private StringBuilder _currentLine;
        private int _currentLineLength;

        public ReplayBuffer(int width)
        {
            Width = width;
            /**/++_Buffers;
        }

        public IEnumerable<string> ToLines()
        {
            if (_currentLine == null)
                return _lines;

            return _lines.Concat(new[] {_currentLine.ToString()});
        }

        public void Write(string data)
        {
            Write(ControlSplitter.Split(data));
        }

        public void Write(IEnumerable<TextControlItem> items)
        {
            foreach (var item in items)
            {
                Write(item);
            }
        }

        public void Write(TextControlItem item)
        {
            if (_currentLine == null)
                StartLine();

            if (item.Text != null)
                WriteText(item.Text);
            else
            {
                foreach (var instruction in item.Instructions)
                {
                    if (instruction == TextControlItem.ControlInstruction.NewLine)
                    {
                        StartLine();
                    }
                }
            }
        }

        private void WriteText(string data)
        {
            Debug.Assert(_currentLine != null);

            var dataLength = data.Length;
            var dataPos = 0;
            while ((dataLength - dataPos) + _currentLineLength > Width)
            {
                var spaceOnLine = Width - _currentLineLength;
                Emit(data.Substring(dataPos, spaceOnLine));
                dataPos += spaceOnLine;
                StartLine();
                ++WordWrapLineBreakCount;
            }

            _currentLine.Append(data.Substring(dataPos));
            _currentLineLength += dataLength - dataPos;
        }

        private void Emit(string data)
        {
            if (_currentLine == null)
                StartLine();

            Debug.Assert(_currentLine != null);
            _currentLine.Append(data);
        }

        private void StartLine()
        {
            if (_currentLine != null)
                _lines.Add(_currentLine.ToString());

            _currentLineLength = 0;
            _currentLine = new StringBuilder();
        }

        public void NewLine()
        {
            StartLine();
        }

        public void Wrap(string data)
        {
            var lines = ColumnWrapper.WrapValue(data, DefaultColumnFormat, Width, firstLineHangingIndent: _currentLineLength);
            if (lines.Length == 0) return;

            foreach (var line in lines.Where((l, c) => c < lines.Length - 1))
            {
                Write(line);
                NewLine();
                ++WordWrapLineBreakCount;
            }
            Write(lines.Last());
        }

        /// <summary>
        /// Add additional wrapping line breaks.
        /// </summary>
        /// <param name="wrappedLineBreaks"></param>
        public void RecordWrapLineBreaks(int wrappedLineBreaks)
        {
            WordWrapLineBreakCount += wrappedLineBreaks;
        }
    }
}
