using System.Collections.Generic;
using System.Text;

namespace TestConsole.OutputFormatting.Internal
{
    /// <summary>
    /// This class performs write operations on a <see cref="IConsoleInterface"/>.
    /// <seealso cref="TextControlItem"/>
    /// </summary>
    internal class ConsoleWriter
    {
        private readonly IConsoleInterface _consoleOutInterface;

        private bool _lastWriteWasPassiveNewLine;

        public ConsoleWriter(IConsoleInterface consoleOutInterface)
        {
            _consoleOutInterface = consoleOutInterface;
        }

        public Encoding Encoding {
            get { return _consoleOutInterface.Encoding; }
        }

        public string PrefixText { get; set; }

        public void Write(List<TextControlItem> components, bool limitWidth)
        {
            foreach (var colourControlItem in components)
            {
                ProcessItem(colourControlItem, limitWidth);
            }
        }

        public void NewLine()
        {
            _consoleOutInterface.NewLine();
        }

        private void ProcessItem(TextControlItem textControlItem, bool limitWidth)
        {
            if (textControlItem.Text != null)
                WriteText(textControlItem, limitWidth);
            else
            {
                foreach (var instruction in textControlItem.Instructions)
                {
                    switch (instruction)
                    {
                        case TextControlItem.ControlInstruction.NewLine:
                            if (!_lastWriteWasPassiveNewLine)
                                _consoleOutInterface.NewLine();
                            else
                                _lastWriteWasPassiveNewLine = false;
                            break;
                    }
                }
            }
        }

        private void WriteText(TextControlItem textControlItem, bool limitWidth)
        {
            if (PrefixText != null)
                WriteTextWithPrefix(textControlItem, limitWidth);
            else
                _consoleOutInterface.Write(textControlItem.Text, limitWidth);

            _lastWriteWasPassiveNewLine = (textControlItem.Text.Length > 0 &&
                                           _consoleOutInterface.CursorLeft == 0);
        }

        private void WriteTextWithPrefix(TextControlItem textControlItem, bool limitWidth)
        {
            var text = textControlItem.Text;
            var remaining = text.Length;
            var textPos = 0;
            do
            {
                if (_consoleOutInterface.CursorLeft == 0)
                {
                    _consoleOutInterface.Write(PrefixText, limitWidth);
                }

                var available = limitWidth
                    ? _consoleOutInterface.BufferWidth - _consoleOutInterface.CursorLeft
                    : int.MaxValue;
                var section = remaining > available ? text.Substring(textPos, available) : text.Substring(textPos);

                _consoleOutInterface.Write(section, limitWidth);
                textPos += section.Length;
                remaining -= section.Length;
            } while (remaining > 0);
        }
    }
}