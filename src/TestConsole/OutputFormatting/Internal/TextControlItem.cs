using System;
using System.Collections.Generic;
using System.Linq;

namespace TestConsole.OutputFormatting.Internal
{
    internal class TextControlItem
    {
        public enum ControlInstruction
        {
            NewLine
        }

        public string Text { get; set; }

        public List<ControlInstruction> Instructions { get; private set; }

        public TextControlItem(string text = null, IEnumerable<ControlInstruction> instructions = null)
        {
            Text = text;
            if (instructions != null)
                Instructions = instructions.ToList();

#if DEBUG
            if (text == null && instructions == null)
                throw new ArgumentException("No arguments specified.");

            if (text != null && instructions != null)
                throw new ArgumentException("Only text or instructions should be specified.");
#endif
        }

        public override string ToString()
        {
            return string.Format("ControlInstruction: Text:\"{0}\" Instructions:[{1}]", Text, 
                string.Join(",", Instructions ?? new List<ControlInstruction>()));
        }
    }
}