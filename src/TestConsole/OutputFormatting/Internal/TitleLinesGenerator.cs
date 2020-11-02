using System;
using System.Collections.Generic;

namespace TestConsole.OutputFormatting.Internal
{
    internal static class TitleLinesGenerator
    {
        internal static IEnumerable<string> Generate(string titleText, int availableWidth, SplitCache cache)
        {
            var lines = ColumnWrapper.WrapValue(titleText, ColumnFormat.DefaultFormat, availableWidth, cache);
            foreach (var line in lines)
            {
                yield return line + Environment.NewLine;
                yield return Underliner.Generate(line) + Environment.NewLine;
            }
        }
    }
}