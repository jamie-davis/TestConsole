using System;
using System.Collections.Generic;

namespace TestConsole.OutputFormatting.Internal
{
    internal static class ControlSplitter
    {
        public static List<TextControlItem> Split(string data)
        {
            var controlItems = new List<TextControlItem>();
            AddTextControlItems(controlItems, data);

            return controlItems;
        }

        private static void AddTextControlItems(List<TextControlItem> controlItems, string text)
        {
            while (text.Length > 0)
            {
                var windowsNewLine = "\r\n";
                var newLinePos = text.IndexOfAny(windowsNewLine.ToCharArray());
                if (newLinePos >= 0)
                {
                    if (newLinePos > 0)
                        controlItems.Add(new TextControlItem(text.Substring(0, newLinePos)));

                    controlItems.Add(
                        new TextControlItem(instructions: new[]
                        {
                            TextControlItem.ControlInstruction.NewLine
                        }));

                    text = text.Substring(newLinePos).StartsWith(windowsNewLine) 
                        ? text.Substring(newLinePos + windowsNewLine.Length)
                        : text.Substring(newLinePos + 1);
                }
                else
                {
                    controlItems.Add(new TextControlItem(text));
                    break;
                }
            }
        }
    }
}