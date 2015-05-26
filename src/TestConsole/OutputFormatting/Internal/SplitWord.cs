using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestConsole.OutputFormatting.Internal
{
    internal class SplitWord
    {
        private readonly List<TextControlItem.ControlInstruction> _instructions = new List<TextControlItem.ControlInstruction>();
        private Lazy<bool> _terminatesLine;
        public int Length { get; private set; }
        public int TrailingSpaces { get; private set; }
        public string WordValue { get; private set; }

        public IEnumerable<TextControlItem.ControlInstruction> Instructions { get { return _instructions; } } 

        public SplitWord(int length, int trailingSpaces, string wordValue)
        {
            Length = length;
            TrailingSpaces = trailingSpaces;
            WordValue = wordValue;
            _terminatesLine = new Lazy<bool>(() => _instructions.Any(i => i == TextControlItem.ControlInstruction.NewLine));
        }

        public string GetWordValue()
        {
            return WordValue;
        }

        public string GetTrailingSpaces(int maxWidth, out int spacesAdded)
        {
            spacesAdded = Math.Min(maxWidth, TrailingSpaces);
            return new string(' ', spacesAdded);
        }

        public bool TerminatesLine()
        {
            return _terminatesLine.Value;
        }

        public void AddInstructions(IEnumerable<TextControlItem.ControlInstruction> instructions)
        {
            _instructions.AddRange(instructions);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("SplitWord:");

            if (!string.IsNullOrEmpty(WordValue))
            {
                sb.Append("\"");
                sb.Append(WordValue);
                sb.Append("\"");
            }

            return sb.ToString();
        }
    }
}