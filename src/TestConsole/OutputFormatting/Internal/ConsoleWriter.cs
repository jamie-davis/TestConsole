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

        public void Write(List<TextControlItem> components)
        {
            foreach (var colourControlItem in components)
            {
                ProcessItem(colourControlItem);
            }
        }

        public void NewLine()
        {
            _consoleOutInterface.NewLine();
        }

        private void ProcessItem(TextControlItem textControlItem)
        {
            if (textControlItem.Text != null)
                WriteText(textControlItem);
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

        private void WriteText(TextControlItem textControlItem)
        {
            if (PrefixText != null)
                WriteTextWithPrefix(textControlItem);
            else
                _consoleOutInterface.Write(textControlItem.Text);

            _lastWriteWasPassiveNewLine = (textControlItem.Text.Length > 0 &&
                                           _consoleOutInterface.CursorLeft == 0);
        }

        private void WriteTextWithPrefix(TextControlItem textControlItem)
        {
            var text = textControlItem.Text;
            var remaining = text.Length;
            var textPos = 0;
            do
            {
                if (_consoleOutInterface.CursorLeft == 0)
                {
                    _consoleOutInterface.Write(PrefixText);
                }

                var available = _consoleOutInterface.BufferWidth - _consoleOutInterface.CursorLeft;
                var section = remaining > available ? text.Substring(textPos, available) : text.Substring(textPos);

                _consoleOutInterface.Write(section);
                textPos += section.Length;
                remaining -= section.Length;
            } while (remaining > 0);
        }
    }
}